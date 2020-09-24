using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Text.RegularExpressions;
using Tp.Core;
using Tp.Core.Annotations;

// ReSharper disable once CheckNamespace

namespace System
{
    public static class StringExtensions
    {
        private static readonly Dictionary<char, char> SpecialCharacters = new Dictionary<char, char>
        {
            { '0', '\0' },
            { 'a', '\a' },
            { 'b', '\b' },
            { 'f', '\f' },
            { 'n', '\n' },
            { 'r', '\r' },
            { 't', '\t' },
            { 'v', '\v' },
            { '\\', '\\' },
            { '"', '\"' },
        };

        private static readonly Dictionary<char, char> SpecialCharactersInverted;

        static StringExtensions()
        {
            SpecialCharactersInverted = SpecialCharacters.ToDictionary(x => x.Value, x => x.Key);
        }

        [StringFormatMethod("format"), Pure]
        public static string Fmt(this string format, params object[] args) =>
            string.Format(format, args);

        [StringFormatMethod("format"), Pure]
        public static string Fmt(this string format, object arg1) =>
            string.Format(format, arg1);

        [StringFormatMethod("format"), Pure]
        public static string Fmt(this string format, object arg1, object arg2) =>
            string.Format(format, arg1, arg2);

        [StringFormatMethod("format"), Pure]
        public static string Fmt(this string format, object arg1, object arg2, object arg3) =>
            string.Format(format, arg1, arg2, arg3);

        [Pure]
        [ContractAnnotation("format:null => true")]
        public static bool IsNullOrEmpty(this string format) =>
            string.IsNullOrEmpty(format);

        [Pure]
        [ContractAnnotation("value:null => true")]
        public static bool IsNullOrWhitespace(this string value) =>
            string.IsNullOrWhiteSpace(value);

        [StringFormatMethod("format")]
        public static StringBuilder AppendLine(
            [NotNull] this StringBuilder stringBuilder, string format, params object[] args)
        {
            return stringBuilder.AppendFormat(format, args).AppendLine();
        }

        [Pure]
        [ContractAnnotation("source:null => false")]
        public static bool Contains(this string source, string value, StringComparison comparisonType) =>
            source != null && source.IndexOf(value, comparisonType) >= 0;

        [Pure]
        public static string ToStringSafe([CanBeNull] this object s) =>
            s?.ToString();

        [Pure]
        [ContractAnnotation("s:null => null")]
        public static string TrimSafe(this string s, params char[] chars) =>
            s?.Trim(chars);

        [Pure]
        public static bool EqualsIgnoreCase(this string a, string b) =>
            string.Equals(a, b, StringComparison.InvariantCultureIgnoreCase);

        [Pure]
        public static bool OrdinalEqualsIgnoreCase(this string a, string b) =>
            string.Equals(a, b, StringComparison.OrdinalIgnoreCase);

        [Pure]
        [ContractAnnotation("value:null => null")]
        public static string FilterInvalidXmlCharacters(this string value)
        {
            if (value.IsNullOrEmpty())
                return value;
            var sb = new StringBuilder(value.Length);
            foreach (var c in value.Where(c => (c == 0x9) ||
                (c == 0xA) ||
                (c == 0xD) ||
                ((c >= 0x20) && (c <= 0xD7FF)) ||
                ((c >= 0xE000) && (c <= 0xFFFD))))
            {
                sb.Append(c);
            }
            return sb.ToString();
        }

        [Pure, NotNull]
        public static string Escape([NotNull] this string s)
        {
            var result = new StringBuilder(s.Length);
            foreach (char c in s)
            {
                char converted;
                if (SpecialCharactersInverted.TryGetValue(c, out converted))
                {
                    result.Append('\\');
                    result.Append(converted);
                }
                else
                {
                    result.Append(c);
                }
            }
            return result.ToString();
        }

        [Pure, NotNull]
        public static string Unescape([NotNull] this string s)
        {
            var result = new StringBuilder(s.Length);

            foreach (var c in NormalizeChars(s))
            {
                result.Append(c);
            }

            return result.ToString();
        }

        [NotNull, Pure, LinqTunnel]
        private static IEnumerable<char> NormalizeChars([NotNull] string s)
        {
            using (var enumerator = s.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var current = enumerator.Current;

                    if (current == '\\')
                    {
                        if (!enumerator.MoveNext())
                        {
                            throw new ArgumentException(Res.InvalidCharacterLiteral.Value);
                        }
                        yield return
                            SpecialCharacters.GetValue(enumerator.Current)
                                .GetOrThrow(() => new ArgumentException(Res.InvalidCharacter(enumerator.Current).Value));
                    }
                    else
                    {
                        yield return current;
                    }
                }
            }
        }

        [Pure]
        [ContractAnnotation("value:null => null")]
        public static string CamelCase(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            var sb = new StringBuilder(value);
            sb[0] = char.ToLowerInvariant(sb[0]);
            return sb.ToString();
        }

        [Pure]
        [TableValuedSqlFunction("f_SplitByComma", "Value", DbType.String)]
        public static IEnumerable<string> SplitByComma(this string value)
        {
            return value.IsNullOrEmpty()
                ? Enumerable.Empty<string>()
                : value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        [Pure]
        [CanBeNull]
        [SqlFunctionAttribute("[dbo].[f_GetUrlCustomFieldPart]", DbType.String)]
        public static string GetUrlCustomFieldPart([CanBeNull] this string urlCustomFieldValue, UrlCustomFieldPart urlPart)
        {
            if (urlCustomFieldValue == null)
            {
                return urlCustomFieldValue;
            }

            var urlPartsSeparator = urlCustomFieldValue.IndexOf("\n", StringComparison.Ordinal);

            if (urlPart == UrlCustomFieldPart.Description)
            {
                return urlPartsSeparator >= 0 ? urlCustomFieldValue.Substring(0, urlPartsSeparator) : null;
            }

            if (urlPart == UrlCustomFieldPart.Uri)
            {
                return urlPartsSeparator >= 0
                    ? urlCustomFieldValue.Substring(urlPartsSeparator + 1,
                        urlCustomFieldValue.Length - urlPartsSeparator - 1)
                    : urlCustomFieldValue;
            }

            return urlCustomFieldValue;
        }

        [Pure]
        public static string NormalizeLineEndings(this string value)
        {
            return Regex.Replace(value, @"\r\n|\n\r|\n|\r", Environment.NewLine);
        }

        [Pure]
        [ContractAnnotation("value:null => null")]
        public static string FirstLetterToUpper(this string value)
        {
            return value.IsNullOrEmpty() ? value : value.First().ToString().ToUpper() + value.Substring(1);
        }
    }
}
