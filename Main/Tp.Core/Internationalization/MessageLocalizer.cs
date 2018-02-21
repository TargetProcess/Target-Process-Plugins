using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Tp.Core.Annotations;
using Tp.I18n;

namespace Tp.Core.Internationalization
{
    /// <summary>
    ///     Provides a possibility for localization of external messages. For example messages gotten from db.
    ///     Should be used only if there is no control on message provider.
    /// </summary>
    public class MessageLocalizer
    {
        public delegate IFormattedMessage FormattedMessageCreator(IDictionary<string, string> data);

        private static readonly Dictionary<string, string> _emptyData = new Dictionary<string, string>();
        private readonly IDictionary<string, FormattedMessageCreator> _valueLocalizations;
        private readonly IDictionary<string, Tuple<Regex, FormattedMessageCreator>> _regexLocalizations;

        public MessageLocalizer()
        {
            _valueLocalizations = new Dictionary<string, FormattedMessageCreator>();
            _regexLocalizations = new Dictionary<string, Tuple<Regex, FormattedMessageCreator>>();
        }

        public void RegisterValueLocalization([NotNull] IFormattedMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            _valueLocalizations.Add(message.Value, _ => message);
        }

        public void RegisterRegexLocalization([NotNull] string regexPattern, [NotNull] FormattedMessageCreator creator)
        {
            if (regexPattern == null)
            {
                throw new ArgumentNullException(nameof(regexPattern));
            }

            if (creator == null)
            {
                throw new ArgumentNullException(nameof(creator));
            }

            _regexLocalizations.Add(regexPattern, Tuple.Create(new Regex(regexPattern), creator));
        }

        public Maybe<IFormattedMessage> TryLocalize([NotNull] string message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            FormattedMessageCreator creator;
            if (_valueLocalizations.TryGetValue(message, out creator))
            {
                return Maybe.Return(creator(_emptyData));
            }

            foreach (var localization in _regexLocalizations.Values)
            {
                var messagePattern = localization.Item1;
                var messageCreator = localization.Item2;
                var matchResult = MatchPattern(messagePattern, message);

                if (matchResult.HasValue)
                {
                    return matchResult.Select(v => messageCreator(v));
                }
            }

            return Maybe.Nothing;
        }

        private static Maybe<Dictionary<string, string>> MatchPattern(Regex messagePattern, string message)
        {
            var matchResult = messagePattern.Match(message);

            if (!matchResult.Success)
            {
                return Maybe.Nothing;
            }

            var groups = messagePattern.GetGroupNames();

            // Skipping first group with name "0", which is pattern itself 
            var data =
                groups.Skip(1).ToDictionary(
                    name => name,
                    name => matchResult.Groups[name].Value);

            return Maybe.Return(data);
        }
    }
}
