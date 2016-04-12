using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;

namespace Tp.Utils.Html
{
	/// <summary>
	/// 	White-list principle HTML entity encoder.
	/// </summary>
	/// <remarks>
	/// 	Unlike <see cref = "HttpUtility" />, provides stronger and more secure encoding where all characters except English alphabet letters will be encoded as HTML entities.
	/// </remarks>
	/// <seealso cref = "HttpUtility" />
	public static class Encoder
	{
		/// <summary>
		/// 	Encodes the specified <paramref name = "input" />
		/// 	and writes encoded text for use in HTML to the specified <paramref name = "output" />.
		/// </summary>
		/// <param name = "input">
		/// 	The text to encode, may be <c>null</c>.
		/// </param>
		/// <param name = "output">
		/// 	Output stream where to write encoded text.
		/// </param>
		/// <exception cref = "ArgumentNullException">
		/// 	If <paramref name = "output" /> is <c>null</c>.
		/// </exception>
		/// <seealso cref = "HttpUtility.HtmlEncode(string,System.IO.TextWriter)" />
		public static void HtmlEncode(string input, TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException(nameof(output));
			}
			if (string.IsNullOrEmpty(input))
			{
				return;
			}

			var builder = new StringBuilder();
			foreach (char ch in input)
			{
				HtmlEncode(ch, builder);
			}
			output.Write(builder);
		}

		/// <summary>
		/// 	Reads text from the specified <paramref name = "input" />
		/// 	and writes encoded text for use in HTML to the specified <paramref name = "output" />.
		/// </summary>
		/// <param name = "input">
		/// 	Input stream containing text to encode.
		/// </param>
		/// <param name = "output">
		/// 	Output stream where to write encoded text.
		/// </param>
		/// <exception cref = "ArgumentNullException">
		/// 	If <paramref name = "input" /> is <c>null</c>.
		/// </exception>
		/// <exception cref = "ArgumentNullException">
		/// 	If <paramref name = "output" /> is <c>null</c>.
		/// </exception>
		/// <seealso cref = "HttpUtility.HtmlEncode(string,System.IO.TextWriter)" />
		public static void HtmlEncode(TextReader input, TextWriter output)
		{
			if (input == null)
			{
				throw new ArgumentNullException(nameof(input));
			}
			if (output == null)
			{
				throw new ArgumentNullException(nameof(output));
			}
			int ch;
			var builder = new StringBuilder();
			while ((ch = input.Read()) != -1)
			{
				HtmlEncode(ch, builder);
			}
			output.Write(builder);
		}

		/// <summary>
		/// 	Encodes the specified input string for use in HTML.
		/// </summary>
		/// <remarks>
		/// 	This method is <c>null</c> safe.
		/// </remarks>
		/// <param name = "input">
		/// 	The string to encode, may be <c>null</c>.
		/// </param>
		/// <returns>
		/// 	<c>null</c> if <paramref name = "input" /> is <c>null</c>, encoded string otherwise.
		/// </returns>
		/// <seealso cref = "HttpUtility.HtmlEncode(string)" />
		public static string HtmlEncode(this string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return input;
			}
			var output = new StringWriter();
			output.GetStringBuilder().EnsureCapacity(input.Length * 2);
			HtmlEncode(input, output);
			return output.ToString();
		}

		/// <summary>
		/// 	Encodes the specified input string for use in HTML tag attributes.
		/// </summary>
		/// <param name = "input">
		/// 	The string to encode, may be <c>null</c>.
		/// </param>
		/// <param name = "output">
		/// 	Output stream where to write encoded text.
		/// </param>
		/// <exception cref = "ArgumentNullException">
		/// 	If <paramref name = "output" /> is <c>null</c>.
		/// </exception>
		/// <seealso cref = "HttpUtility.HtmlAttributeEncode(string,System.IO.TextWriter)" />
		public static void HtmlAttributeEncode(string input, TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException(nameof(output));
			}
			if (string.IsNullOrEmpty(input))
			{
				return;
			}
			foreach (char ch in input)
			{
				HtmlAttributeEncode(ch, output);
			}
		}

		/// <summary>
		/// 	Encodes the specified input string for use in HTML tag attributes.
		/// </summary>
		/// <param name = "input">
		/// 	Input stream containing text to encode.
		/// </param>
		/// <param name = "output">
		/// 	Output stream where to write encoded text.
		/// </param>
		/// <exception cref = "ArgumentNullException">
		/// 	If <paramref name = "input" /> is <c>null</c>.
		/// </exception>
		/// <exception cref = "ArgumentNullException">
		/// 	If <paramref name = "output" /> is <c>null</c>.
		/// </exception>
		/// <seealso cref = "HttpUtility.HtmlAttributeEncode(string,System.IO.TextWriter)" />
		public static void HtmlAttributeEncode(TextReader input, TextWriter output)
		{
			if (input == null)
			{
				throw new ArgumentNullException(nameof(input));
			}
			if (output == null)
			{
				throw new ArgumentNullException(nameof(output));
			}
			int ch;
			while ((ch = input.Read()) != -1)
			{
				HtmlAttributeEncode(ch, output);
			}
		}

		/// <summary>
		/// 	Encodes the specified input string for use in HTML tag attributes.
		/// </summary>
		/// <param name = "input">
		/// 	The string to encode, may be <c>null</c>.
		/// </param>
		/// <returns>
		/// 	<c>null</c> if <paramref name = "input" /> is <c>null</c>, encoded string otherwise.
		/// </returns>
		/// <seealso cref = "HttpUtility.HtmlAttributeEncode(string)" />
		public static string HtmlAttributeEncode(string input)
		{
			if (input == null)
			{
				return null;
			}
			if (input.Length == 0)
			{
				return "";
			}
			var output = new StringWriter();
			output.GetStringBuilder().EnsureCapacity(input.Length * 2);
			HtmlAttributeEncode(input, output);
			return output.ToString();
		}

		private static void HtmlEncode(int ch, StringBuilder builder)
		{
			if (Char.IsWhiteSpace((char) ch))
			{
				var unicodeCategory = Char.GetUnicodeCategory((char) ch);
				if (unicodeCategory != UnicodeCategory.LineSeparator && unicodeCategory != UnicodeCategory.ParagraphSeparator)
				{
					builder.Append((char) ch);
				}
			}
			else if (Char.IsLetterOrDigit((char) ch) || Char.GetUnicodeCategory((char) ch) == UnicodeCategory.CurrencySymbol)
			{
				builder.Append((char) ch);
			}
			else if (ch == '<')
			{
				builder.Append("&lt;");
			}
			else if (ch == '>')
			{
				builder.Append("&gt;");
			}
			else if (ch == '"')
			{
				builder.Append("&quot;");
			}
			else if (
				(ch > '`' && ch < '{')
					|| (ch > '@' && ch < '[')
					|| (ch > '/' && ch < ':')
					|| (ch == '.' || ch == '-' || ch == '_')
				)
			{
				builder.Append((char) ch);
			}
			else
			{
				builder.Append("&#");
				builder.Append(ch);
				builder.Append(";");
			}
		}

		private static void HtmlAttributeEncode(int ch, TextWriter output)
		{
			if (Char.IsWhiteSpace((char) ch))
			{
				output.Write((char) ch);
			}
			else if (ch == '<')
			{
				output.Write("&lt;");
			}
			else if (ch == '>')
			{
				output.Write("&gt;");
			}
			else if (ch == '"')
			{
				output.Write("&quot;");
			}
			else if ((ch > '`' && ch < '{')
				|| (ch > '@' && ch < '[')
				|| (ch > '/' && ch < ':')
				|| (ch == '.' || ch == ',' || ch == '-' || ch == '_')
				)
			{
				output.Write((char) ch);
			}
			else
			{
				output.Write("&#");
				output.Write(ch);
				output.Write(";");
			}
		}
	}
}
