// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using Tp.Core;
using Tp.Core.Annotations;

namespace System
{
	public static class StringExtensions
	{

		private static readonly Dictionary<char, char> SpecialCharacters = new Dictionary<char, char>
			{
				{'0', '\0'},
				{'a', '\a'},
				{'b', '\b'},
				{'f', '\f'},
				{'n', '\n'},
				{'r', '\r'},
				{'t', '\t'},
				{'v', '\v'},
				{'\\', '\\'},
				{'"', '\"'},
			};

		private static readonly Dictionary<char, char> SpecialCharactersInverted;

		static StringExtensions()
		{
			SpecialCharactersInverted = SpecialCharacters.ToDictionary(x => x.Value, x => x.Key);
		}

		[StringFormatMethod("format")]
		public static string Fmt(this string format, params object[] args)
		{
			return String.Format(format, args);
		}

		[StringFormatMethod("format")]
		public static string Fmt(this string format, object arg1)
		{
			return String.Format(format, arg1);
		}

		[StringFormatMethod("format")]
		public static string Fmt(this string format, object arg1, object arg2)
		{
			return String.Format(format, arg1, arg2);
		}

		[StringFormatMethod("format")]
		public static string Fmt(this string format, object arg1, object arg2, object arg3)
		{
			return String.Format(format, arg1, arg2, arg3);
		}

		public static bool IsNullOrEmpty(this string format)
		{
			return String.IsNullOrEmpty(format);
		}

		public static bool IsNullOrWhitespace(this string value)
		{
			return value == null || value.Trim().Length == 0;
		}

		[StringFormatMethod("format")]
		public static StringBuilder AppendLine(this StringBuilder stringBuilder, string format, params object[] args)
		{
			return stringBuilder.AppendFormat(format, args).AppendLine();
		}

		public static bool Contains(this string source, string value, StringComparison comparisonType)
		{
			return source.IndexOf(value, comparisonType) >= 0;
		}

		public static string ToStringSafe(this object s)
		{
			if (s == null)
				return null;
			return s.ToString();
		}

		public static string TrimSafe(this string s, params char[] chars)
		{
			if (s == null)
				return null;
			return s.Trim(chars);
		}

		public static bool EqualsIgnoreCase(this string a, string b)
		{
			return string.Equals(a, b, StringComparison.InvariantCultureIgnoreCase);
		}


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


		public static string Escape(this string s)
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

		public static string Unescape(this string s)
		{
			var result = new StringBuilder(s.Length);

			foreach (var c in NormalizeChars(s))
			{
				result.Append(c);
			}

			return result.ToString();
		}

		private static IEnumerable<char> NormalizeChars(string s)
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
							throw new ArgumentException(Res.InvalidCharacterLiteral);
						}
						yield return SpecialCharacters.GetValue(enumerator.Current).GetOrThrow(() => new ArgumentException(Res.InvalidCharacter.Fmt(enumerator.Current)));
					}
					else
					{
						yield return current;
					}
				}
			}
		}

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
	}
}