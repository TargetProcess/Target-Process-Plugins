// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Tp.Core
{
	/// <summary>
	/// Basic string manipulation routines.
	/// </summary>
	public static class StringUtils
	{
		public static readonly string[] LineBreaks = new[] { "\r\n", "\r", "\n" };


		/// <summary>
		/// A regular expression to validate email addresses.
		/// </summary>
		public static readonly Regex EmailRegex = new Regex(@"^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$", RegexOptions.IgnoreCase);
		/// <summary>
		/// Verify whether string is a valid email address.
		/// </summary>
		/// <param name="value">A string.</param>
		/// <returns><code>true</code> if valid email address, <code>false</code> otherwise.</returns>
		public static bool IsEmail(this string value)
		{
			return !String.IsNullOrEmpty(value) && EmailRegex.IsMatch(value);
		}

		/// <summary>
		/// Verifies whether the specified string is null or white space only. This method is null-safe.
		/// </summary>
		/// <remarks>
		/// Here is how it works:
		/// <example>
		/// <code>
		/// Assert.IsTrue(StringUtils.IsBlank(null));
		/// Assert.IsTrue(StringUtils.IsBlank(""));
		/// Assert.IsTrue(StringUtils.IsBlank("    "));
		/// Assert.IsFalse(StringUtils.IsBlank("  hello  "));
		/// </code>
		/// </example>
		/// </remarks>
		/// <param name="value">The string to test.</param>
		/// <returns>
		/// <c>true</c> if the specified string is <c>null</c> or contains white-space only; <c>false</c> otherwise.
		/// </returns>
		public static bool IsBlank(this string value)
		{
			return String.IsNullOrEmpty(value) || value.All(Char.IsWhiteSpace);
		}

		/// <summary>
		/// Inverse of <see cref="IsBlank"/>.
		/// </summary>
		/// <param name="value">The string to test.</param>
		/// <returns>
		/// <c>false</c> if the specified string is <c>null</c> or contains white-space only; <c>true</c> otherwise.
		/// </returns>
		public static bool IsNotBlank(this string value)
		{
			return !IsBlank(value);
		}

		/// <summary>
		/// Trims the specified string. If the result is empty string, then <c>null</c> is returned. This method is null-safe.
		/// </summary>
		/// <remarks>
		/// Here is how it works:
		/// <example>
		/// <code>
		/// Assert.AreEqual(null, StringUtils.TrimToNull(null));
		/// Assert.AreEqual(null, StringUtils.TrimToNull(""));
		/// Assert.AreEqual(null, StringUtils.TrimToNull("    "));
		/// Assert.AreEqual("hello", StringUtils.TrimToNull("  hello  "));
		/// </code>
		/// </example>
		/// </remarks>
		/// <param name="value">The string to trim.</param>
		/// <returns>
		/// Trimming result, or <c>null</c> if result is empty string.
		/// </returns>
		[SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
		public static string TrimToNull(this string value)
		{
			if (IsBlank(value))
			{
				return null;
			}
			value = value.Trim();
			return value.Length == 0 ? null : value;
		}

		/// <summary>
		/// Trims the specified string. This method is null-safe.
		/// </summary>
		/// <remarks>
		/// <example>
		/// Here is how it works:
		/// <code>
		/// Assert.AreEqual("", StringUtils.TrimToEmpty(null));
		/// Assert.AreEqual("", StringUtils.TrimToEmpty(""));
		/// Assert.AreEqual("", StringUtils.TrimToEmpty("    "));
		/// Assert.AreEqual("hello", StringUtils.TrimToEmpty("  hello  "));
		/// </code>
		/// </example>
		/// </remarks>
		/// <param name="value">The string to trim.</param>
		/// <returns>
		/// Trimming result, or <c>null</c> if result is empty string.
		/// </returns>
		[SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
		public static string TrimToEmpty(this string value)
		{
			return IsBlank(value) ? String.Empty : value.Trim();
		}

		public static List<int> ToIntList(this IList<string> ids)
		{
			if (ids == null)
			{
				throw new ArgumentNullException("ids");
			}
			var result = new List<int>(ids.Count);
			foreach (string id in ids)
			{
				int n;
				if (Int32.TryParse(id, out n))
				{
					result.Add(n);
				}
			}
			return result;
		}

		public static List<string> ToStringList(this IList<int> ids)
		{
			if (ids == null)
			{
				throw new ArgumentNullException("ids");
			}
			var result = new List<string>(ids.Count);
			result.AddRange(ids.Select(id => Convert.ToString(id, CultureInfo.InvariantCulture)));
			return result;
		}

		public static string GetLastWord(string name)
		{
			if (String.IsNullOrEmpty(name))
			{
				return String.Empty;
			}

			int whiteSpaceIndex = name.LastIndexOf(" ");

			return whiteSpaceIndex < 0 ? name : name.Substring(whiteSpaceIndex + 1);
		}

		public static string ToStringArray(this object[] tags)
		{
			if (tags == null)
			{
				throw new ArgumentNullException("tags");
			}
			var result = new StringBuilder();
			for (int i = 0; i < tags.Length; i++)
			{
				if (i > 0)
				{
					result.Append(",");
				}
				result.Append(tags[i]);
			}
			return result.ToString();
		}

		/// <summary>
		/// Remove any whitespace, punctuation and graphics from the specified string, convert result to lower case.
		/// </summary>
		/// <remarks>
		/// Here is how it works:
		/// <example>
		/// <code>
		/// Assert.IsNull(StringUtils.Simplify(null));
		/// Assert.IsNull(StringUtils.Simplify(""));
		/// Assert.IsNull(StringUtils.Simplify("    "));
		/// Assert.AreEqual("helloworld", StringUtils.Simplify("  Hello, World!  "));
		/// Assert.AreEqual("iam99yearsold", StringUtils.Simplify("  I  am  99  years  old!  "));
		/// </code>
		/// </example>
		/// </remarks>
		/// <param name="value">Original value.</param>
		/// <returns>Mangled value for comparison.</returns>
		public static string Simplify(this string value)
		{
			if (value == null)
			{
				return null;
			}

			var result = new char[value.Length];
			int off = 0;
			foreach (char c in value.Where(Char.IsLetterOrDigit))
			{
				result[off++] = Char.ToLowerInvariant(c);
			}
			return off > 0 ? new string(result, 0, off) : null;
		}

		public static string GetGroupMatch(this string value, string pattern)
		{
			var regex = new Regex(pattern);
			var match = regex.Match(value);
			if (!match.Success)
				return null;
			if (match.Groups.Count < 2)
				throw new ApplicationException("Group brackets not found in specified regex patern");
			return match.Groups[1].Value;
		}

		public static string GetGroupMatch(this string value, string pattern, int groupId)
		{
			var regex = new Regex(pattern);
			var match = regex.Match(value);
			if (!match.Success)
				return null;
			if (match.Groups.Count <= groupId)
				throw new ApplicationException("Specified group not found in regex pattern");
			return regex.Match(value).Groups[groupId].Value;
		}

		public static string DecodeFromBase64(this string value)
		{
			return Encoding.Default.GetString(Convert.FromBase64String(value));
		}

		public static string EncodeToBase64(this string value)
		{
			return Convert.ToBase64String(Encoding.Default.GetBytes(value));
		}

		public static string ComputeMD5Hash(this string value)
		{
			return ComputeMD5Hash(value, Encoding.Default);
		}

		public static string ComputeMD5Hash(this string value, Encoding encoding)
		{
			using (MD5 md5 = MD5.Create())
			{
				var encodedBytes = md5.ComputeHash(encoding.GetBytes(value));
				var hash = BitConverter.ToString(encodedBytes).Replace("-", "");
				return hash;
			}
		}

		public static Image Base64ToImage(this string base64String)
		{
			// Convert Base64 String to byte[]
			var imageBytes = Convert.FromBase64String(base64String);
			var ms = new MemoryStream(imageBytes, 0,
									  imageBytes.Length);

			// Convert byte[] to Image
			ms.Write(imageBytes, 0, imageBytes.Length);
			var image = Image.FromStream(ms, true);
			return image;
		}
		public static bool IsManyWords(string text, char delimiter = ' ')
		{
			return !String.IsNullOrEmpty(text) && text.Contains(delimiter);
		}

		public static string[] GetWords(string text, char delimiter = ' ')
		{
			if (IsManyWords(text, delimiter))
			{
				return text.Split(delimiter).Where(c => !string.IsNullOrEmpty(c)).ToArray();
			}
			return new[] { text };
		}

		public static string GetAbbreviation(string text)
		{
			if (IsManyWords(text))
			{
				var strArray = GetWords(text);
				var chars = strArray.Select(c => c[0].ToString().ToUpper());
				return string.Join("", chars.Take(2));
			}
			else
			{
				var result = GetLastWord(text);
				return result == String.Empty ? String.Empty : result[0].ToString();
			}
		}

		public static bool EqualWithoutSpaces(this string a, string b, StringComparison comparison = StringComparison.CurrentCulture)
		{
			if (a == null && b == null)
			{
				return true;
			}
			if (a == null || b == null)
			{
				return false;
			}
			return String.Equals(a.Replace(" ", ""), b.Replace(" ", ""), comparison);
		}
	}
}