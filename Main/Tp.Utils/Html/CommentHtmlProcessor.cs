using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Iesi.Collections.Generic;

namespace Tp.Utils.Html
{
	public class CommentHtmlProcessor : Sanitizer
	{
		private HtmlNodeType _prevNodeType = HtmlNodeType.None;

		protected readonly Iesi.Collections.Generic.ISet<string> KeepAttributeElements = new HashedSet<string>
		{
			"img",
			"a",
			"span",
			"table",
			"td"
		};

		protected readonly Iesi.Collections.Generic.ISet<string> KeepElements = new HashedSet<string>
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
			"tfoot"
		};


		public static readonly Regex HtmlMentionRegex = new Regex(@"data-mention=[""'](?<email>\S+?)['""]", RegexOptions.Compiled);
		public static readonly Regex MarkdownMentionRegex = new Regex(@"\B(@|&#64;)(?<email>\S+)", RegexOptions.Compiled);

		public CommentHtmlProcessor()
			: this(false)
		{
		}

		public CommentHtmlProcessor(bool requiredHtmlEncode)
		{
			RequiredHtmlEncode = requiredHtmlEncode;
		}

		public IEnumerable<string> GetMentions(string input)
		{
			return from match in HtmlMentionRegex.Matches(input).OfType<Match>().Union(MarkdownMentionRegex.Matches(input).OfType<Match>())
				select match.Groups["email"]
				into gr
				where gr.Success
				select gr.Value.Replace("&#64;", "@");
		}

		protected override bool IsValidAttribute(string name, string key, string value)
		{
			if ((key == "style" || key == "class") && name != "span")
				return false;

			return base.IsValidAttribute(name, key, value);
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
				value = value.TrimStart(new[] { '\r', '\n' });
			}

			if (TopTag != null)
			{
				switch (TopTag.ToLowerInvariant())
				{
					case "pre":
						string text = RewritePreText(RequiredHtmlEncode ? value.HtmlEncode() : value);
						_prevNodeType = HtmlNodeType.Text;
						result.Write(text);
						return;
				}
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
	}
}
