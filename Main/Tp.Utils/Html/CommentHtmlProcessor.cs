using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Tp.Utils.Html
{
    public class CommentHtmlProcessor : Sanitizer, IMentionsExtractor
    {
        private class UserMention
        {
            public UserMention(string raw, string loginOrEmail, string name)
            {
                Raw = raw;
                LoginOrEmail = loginOrEmail;
                Name = name;
            }

            public string Raw { get; }
            public string LoginOrEmail { get; }
            public string Name { get; }
        }

        private class TeamMention
        {
            public TeamMention(string raw, int id, string name)
            {
                Raw = raw;
                Id = id;
                Name = name;
            }

            public string Raw { get; }
            public int Id { get; }
            public string Name { get; }
        }

        private class KeepAttributeRule
        {
            public KeepAttributeRule(string tagName, Predicate<string> needKeep)
            {
                TagName = tagName;
                NeedKeep = needKeep;
            }

            public string TagName { get; }
            public Predicate<string> NeedKeep { get; }
        }

        private HtmlNodeType _prevNodeType = HtmlNodeType.None;

        protected readonly ISet<string> KeepAttributeElements = new HashSet<string>
        {
            "img",
            "a",
            "span",
            "table",
            "td"
        };

        protected readonly ISet<string> KeepElements = new HashSet<string>
        {
            "p",
            "br",
            "a",
            "img",
            "div",
            "span",
            "ul",
            "li",
            "ol",
            "b",
            "i",
            "em",
            "strong",
            "strike",
            "u",
            "s",
            "span",
            "pre",
            "table",
            "thead",
            "tbody",
            "td",
            "th",
            "tr",
            "caption",
            "colgroup",
            "col",
            "tfoot",
            "code"
        };

        private static readonly ISet<string> _keepImgStyleProperties = new HashSet<string>
        {
            "border-width",
            "border-style",
            "float",
            "height",
            "margin",
            "width"
        };

        private readonly IDictionary<string, KeepAttributeRule[]> _keepAttributeRules = new Dictionary<string, KeepAttributeRule[]>
        {
            {
                "class", new []
                {
                    // For mentions?
                    new KeepAttributeRule("span", _ => true)
                }
            },
            {
                "style", new []
                {
                    // CkEditor has special styling (ex. images), so allow to pass the gate only if all styles match.
                    new KeepAttributeRule("img", style =>
                    {
                        if (style.IsNullOrEmpty()) return false;

                        var imgStyleProperties = style.Split(';')
                            .Select(property => property.Split(':').FirstOrDefault()?.Trim())
                            .Where(name => !name.IsNullOrEmpty())
                            .ToArray();

                        return imgStyleProperties.All(property => _keepImgStyleProperties.Contains(property));
                    }),
                    // For mentions?
                    new KeepAttributeRule("span", _ => true)
                }
            }
        };

        private static readonly Regex CkEditorMentionedEmailsRegexObsolete = new Regex(@"data-mention=[""'](?<login>\S+?)['""]", RegexOptions.Compiled);
        private static readonly Regex MarkdownMentionedUserEmailsLoginsRegexObsolete = new Regex(@"\B(?>@|&#64;)(?!team|user)(?<login>[^\[\s]+)\b", RegexOptions.Compiled);
        private static readonly Regex CkEditorMentionedUserLoginsRegex = new Regex(@"&#64;user&#58;(?<login>.+?)&#91;(?<name>.+?)&#93;\B", RegexOptions.Compiled);
        private static readonly Regex MarkdownMentionedUserLoginsRegex = new Regex(@"@user:(?<login>.+?)\[(?<name>.+?)\]\B", RegexOptions.Compiled);

        private static readonly Regex CkEditorMentionedTeamIdsRegex = new Regex(@"&#64;team&#58;(?<id>\d{1,9})&#91;(?<name>.+?)&#93;\B", RegexOptions.Compiled);
        private static readonly Regex MarkdownMentionedTeamIdsRegex = new Regex(@"@team:(?<id>\d{1,9})\[(?<name>.+?)\]\B", RegexOptions.Compiled);

        public CommentHtmlProcessor() : this(false) { }

        public CommentHtmlProcessor(bool requiredHtmlEncode)
        {
            RequiredHtmlEncode = requiredHtmlEncode;
        }

        public IEnumerable<int> GetMentionedTeamsIds(string input)
        {
            return
                from match in CkEditorMentionedTeamIdsRegex.Matches(input).OfType<Match>().Union(MarkdownMentionedTeamIdsRegex.Matches(input).OfType<Match>())
                select match.Groups["id"]
                into gr
                where gr.Success
                select int.Parse(gr.Value);
        }

        public IEnumerable<string> GetMentionedUserLoginsOrEmails(string input)
        {
            return
                from match in CkEditorMentionedUserLoginsRegex.Matches(input).OfType<Match>()
                    .Union(MarkdownMentionedUserLoginsRegex.Matches(input).OfType<Match>())
                select match.Groups["login"]
                into gr
                where gr.Success
                select gr.Value.Replace("&#64;", "@");
        }

        public IEnumerable<string> GetMentionedUserLoginsOrEmailsObsolete(string input)
        {
            return
                from match in CkEditorMentionedEmailsRegexObsolete.Matches(input).OfType<Match>()
                    .Union(MarkdownMentionedUserEmailsLoginsRegexObsolete.Matches(input).OfType<Match>())
                select match.Groups["login"]
                into gr
                where gr.Success
                select gr.Value.Replace("&#64;", "@");
        }

        protected override bool IsValidAttribute(string name, string key, string value)
        {
            return _keepAttributeRules.TryGetValue(key, out var rules)
                ? rules.Any(rule => rule.TagName == name && rule.NeedKeep(value))
                : base.IsValidAttribute(name, key, value);
        }

        protected override void WriteElement(TextWriter result, string name, Dictionary<string, string> attributes, bool empty)
        {
            if (!KeepElements.Contains(name))
                return;

            if (KeepAttributeElements.Contains(name))
            {
                _prevNodeType = HtmlNodeType.Element;
                base.WriteElement(result, name, attributes, empty);
                return;
            }

            _prevNodeType = HtmlNodeType.Element;
            base.WriteElement(result, name, new Dictionary<string, string>(), empty);
        }

        protected override void WriteEndElement(TextWriter result, string name)
        {
            if (!KeepElements.Contains(name))
                return;

            _prevNodeType = HtmlNodeType.None;
            base.WriteEndElement(result, name);
        }

        protected override void WriteText(TextWriter result, string value)
        {
            if (_prevNodeType == HtmlNodeType.Element && !string.IsNullOrEmpty(value))
            {
                value = value.TrimStart('\r', '\n');
            }

            if (TopTag != null && TopTag.ToLowerInvariant() == "pre")
            {
                var text = RewritePreText(RequiredHtmlEncode ? value.HtmlEncode() : value);
                _prevNodeType = HtmlNodeType.Text;
                result.Write(text);
                return;
            }

            _prevNodeType = HtmlNodeType.Text;
            base.WriteText(result, value);
        }

        protected override void WriteCData(TextWriter result, string value)
        {
            _prevNodeType = HtmlNodeType.CDATA;
            base.WriteCData(result, value);
        }

        protected override void HtmlEncode(string value, TextWriter writer)
        {
            if (RequiredHtmlEncode)
                base.HtmlEncode(value, writer);
            else
                writer.Write(value);
        }

        protected override void HtmlAttributeEncode(string value, TextWriter writer)
        {
            if (RequiredHtmlEncode)
                base.HtmlAttributeEncode(value, writer);
            else
                writer.Write(value);
        }

        private static string RewritePreText(string preText)
        {
            return preText.Replace("\r\n", "\n").Replace("\n", "<br />");
        }

        public bool RequiredHtmlEncode { get; set; }

        public static string ToPlainCleanText(string input)
        {
            if (input.IsNullOrEmpty()) return string.Empty;

            var clean = TransformMarkdown(input);
            clean = ReplaceAllMentions(clean);
            clean = PlainTextRenderer.RenderToPlainText(clean);

            return clean;
        }

        private static IEnumerable<UserMention> GetUserMentions(string input)
        {
            var matches = CkEditorMentionedUserLoginsRegex.Matches(input).OfType<Match>()
                .Union(MarkdownMentionedUserLoginsRegex.Matches(input).OfType<Match>())
                .Union(CkEditorMentionedEmailsRegexObsolete.Matches(input).OfType<Match>())
                .Union(MarkdownMentionedUserEmailsLoginsRegexObsolete.Matches(input).OfType<Match>());

            return matches.Select(match => new UserMention(match.Groups[0].Value, match.Groups["login"].Value.Replace("&#64;", "@"), match.Groups["name"].Value));
        }

        private static IEnumerable<TeamMention> GetTeamMentions(string input)
        {
            var matches = MarkdownMentionedTeamIdsRegex.Matches(input).OfType<Match>()
                 .Union(CkEditorMentionedTeamIdsRegex.Matches(input).OfType<Match>());

            return matches.Select(match =>
            {
                int.TryParse(match.Groups["id"].Value, out var matchedTeamId);
                return new TeamMention(match.Groups[0].Value, matchedTeamId, match.Groups["name"].Value);
            });
        }

        public static string ReplaceAllMentions(string input)
        {
            GetUserMentions(input).ForEach(mention => input = input.Replace(mention.Raw, mention.Name));
            GetTeamMentions(input).ForEach(mention => input = input.Replace(mention.Raw, mention.Name));
            return input;
        }

        public string ReplaceMentionedUserLoginsEmails(string input, IDictionary<string, string> users, Func<string, string> replaceUserIdPattern)
        {
            if (!users.IsNullOrEmpty())
            {
                GetUserMentions(input)
                    .Where(mention => users.ContainsKey(mention.LoginOrEmail))
                    .ForEach(mention => input = input.Replace(mention.Raw, replaceUserIdPattern(users[mention.LoginOrEmail])));
            }

            return input;
        }

        public string ReplaceMentionedTeamIds(string input, IDictionary<int, string> teams, Func<string, string> replaceTeamIdPattern)
        {
            if (!teams.IsNullOrEmpty())
            {
                GetTeamMentions(input)
                    .Where(mention => teams.ContainsKey(mention.Id))
                    .ForEach(mention => input = input.Replace(mention.Raw, replaceTeamIdPattern(teams[mention.Id])));
            }

            return input;
        }

        public string GetUserNotifiedTemplateValue(string firstName, string lastName, string login, string email)
        {
            if (!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName))
            {
                return $"{firstName} {lastName}";
            }
            if (!string.IsNullOrEmpty(login))
            {
                return login;
            }
            return email;
        }

        public static string HighlightLegacyEmailsLogins(string html, string ckEditorStyle, string markdownStyle)
        {
            html = CkEditorMentionedEmailsRegexObsolete.Replace(html, ckEditorStyle);
            html = MarkdownMentionedUserEmailsLoginsRegexObsolete.Replace(html, markdownStyle);
            return html;
        }
    }

    public interface IMentionsExtractor
    {
        IEnumerable<int> GetMentionedTeamsIds(string input);
        IEnumerable<string> GetMentionedUserLoginsOrEmails(string input);
        IEnumerable<string> GetMentionedUserLoginsOrEmailsObsolete(string input);

        string ReplaceMentionedUserLoginsEmails(string input, IDictionary<string, string> users, Func<string, string> replaceUserIdPattern);
        string ReplaceMentionedTeamIds(string input, IDictionary<int, string> teams, Func<string, string> replaceTeamIdPattern);
        string GetUserNotifiedTemplateValue(string firstName, string lastName, string login, string email);
    }
}
