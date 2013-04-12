using System.Globalization;
using System.Linq;

namespace Mercurial
{
    /// <summary>
    /// This class implements IsNullOrWhiteSpace for .NET 3.5.
    /// </summary>
    internal static class StringEx
    {
        /// <summary>
        /// Compares the <paramref name="value"/> against <c>null</c> and checks if the
        /// string contains only whitespace.
        /// </summary>
        /// <param name="value">
        /// The string value to check.
        /// </param>
        /// <returns>
        /// <c>true</c> if the string <paramref name="value"/> is <c>null</c>, <see cref="string.Empty"/>,
        /// or contains only whitespace; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrWhiteSpace(string value)
        {
            return !(value != null && value.Trim().Length > 0);
        }

        /// <summary>
        /// Scans the string for any character categorized as whitespace.
        /// </summary>
        /// <param name="value">
        /// The string to scan for whitespace.
        /// </param>
        /// <returns>
        /// <c>true</c> if the string contains any character categorized as whitespace;
        /// otherwise, <c>false</c>.
        /// </returns>
        public static bool ContainsWhiteSpace(string value)
        {
            if (value == null)
                return false;

            return value.Any(c => char.IsWhiteSpace(c));
        }

        /// <summary>
        /// Add encapsulating double quotes to the value if it contains whitespace.
        /// </summary>
        /// <param name="value">
        /// The value to optionally add double quotes around.
        /// </param>
        /// <returns>
        /// The value; with double quotes on both ends if it contains whitespace.
        /// </returns>
        public static string EncapsulateInQuotesIfWhitespace(string value)
        {
            if (IsNullOrWhiteSpace(value))
                return string.Empty;

            value = value.Trim();
            if (ContainsWhiteSpace(value))
                return string.Format(CultureInfo.InvariantCulture, "\"{0}\"", value);

            return value;
        }
    }
}