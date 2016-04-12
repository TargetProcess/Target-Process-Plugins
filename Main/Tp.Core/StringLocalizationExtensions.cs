using System.Collections.Generic;
using Jeffijoe.MessageFormat;
using Tp.Core.Annotations;
using Tp.I18n;

// ReSharper disable once CheckNamespace

namespace System
{
	public static class StringLocalizationExtensions
	{
		public static IIntl Intl => new Intl(new MessageFormatter(locale: "en", useCache: false));

		[Pure]
		public static IFormattedMessage Localize(this string token) =>
			Intl.GetFormattedMessage(token);

		[Pure]
		public static IFormattedMessage Localize(this string token, object data) =>
			Intl.GetFormattedMessage(token, data);

		[Pure]
		public static IFormattedMessage AsLocalizable(this string token) =>
			new FormattedMessage(token, new Dictionary<string, object>(), token);

		[Pure]
		public static IFormattedMessage AsLocalized(this string message) =>
			"{message}".Localize(new { message });

		private const string CombineToken = "{part1}{part2}";

		[Pure]
		public static IFormattedMessage Combine(this IFormattedMessage part1, IFormattedMessage part2)
		{
			var parts = new { part1, part2 };
			return CombineToken.Localize(parts);
		}

		[Pure]
		public static IFormattedMessage Combine(this IFormattedMessage part1, string part2)
		{
			var parts = new { part1, part2 };
			return CombineToken.Localize(parts);
		}
	}
}
