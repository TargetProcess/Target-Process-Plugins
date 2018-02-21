using System.Collections.Generic;
using System.Linq.Dynamic;
using Jeffijoe.MessageFormat;
using Tp.Core.Annotations;
using Tp.I18n;

// ReSharper disable once CheckNamespace

namespace System
{
    public static class StringLocalizationExtensions
    {
        [NotNull]
        public static IIntl Intl => new Intl(new MessageFormatter(locale: "en", useCache: false));

        [Pure]
        [NotNull]
        public static IFormattedMessage Localize([NotNull] this string token) =>
            Intl.GetFormattedMessage(token);

        [Pure]
        [NotNull]
        public static IFormattedMessage Localize([NotNull] this string token, [NotNull] object data) =>
            Intl.GetFormattedMessage(token, data);

        [Pure]
        [NotNull]
        [PublicApiMethod]
        public static IFormattedMessage AsLocalizable(this string token) =>
            new FormattedMessage(token, new Dictionary<string, object>(), token);

        [Pure]
        [NotNull]
        public static IFormattedMessage AsLocalized(this string message) =>
            "{message}".Localize(new { message });

        private const string CombineToken = "{part1}{part2}";

        [Pure]
        [NotNull]
        public static IFormattedMessage Combine(
            [NotNull] this IFormattedMessage part1,
            [NotNull] IFormattedMessage part2)
        {
            var parts = new { part1, part2 };
            return CombineToken.Localize(parts);
        }

        [Pure]
        [NotNull]
        public static IFormattedMessage Combine(
            [NotNull] this IFormattedMessage part1,
            [NotNull] string part2)
        {
            var parts = new { part1, part2 };
            return CombineToken.Localize(parts);
        }
    }
}
