using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace Tp.Utils.Html
{
    /// <summary>
    /// Rewrites image urls to make them absolute.
    /// </summary>
    public class ImgSanitizer : Sanitizer
    {
        private readonly string _baseUri;

        public ImgSanitizer(string baseUri)
        {
            if (baseUri == null)
            {
                throw new ArgumentNullException(nameof(baseUri));
            }
            _baseUri = baseUri.TrimEnd('/');
            var regex = new Regex(@"((?#protocol)https?://)((?#domain)[-\w.]+(?:(?#port):\d{1,5})?)(/.*)?", RegexOptions.IgnoreCase);
            _baseUri = regex.Replace(_baseUri, "$1$2");
        }

        /// <summary>
        /// Sanitize input HTML using default settings.
        /// </summary>
        /// <param name="baseUri"></param>
        /// <param name="input">Input HTML. May be <c>null</c>.</param>
        /// <returns>Sanitized HTML.</returns>
        public static string Sanitize(string baseUri, string input)
        {
            if (input == null)
            {
                return null;
            }

            var inputReader = new StringReader(input);
            var resultWriter = new StringWriter();
            resultWriter.GetStringBuilder().EnsureCapacity(input.Length + 32);
            resultWriter.NewLine = "\n";
            var sanitizer = new ImgSanitizer(baseUri);
            sanitizer.Sanitize(inputReader, resultWriter);
            return resultWriter.ToString();
        }

        protected override void WriteElement(TextWriter result, string name, Dictionary<string, string> attributes, bool empty)
        {
            if (string.Compare(name, "img", StringComparison.OrdinalIgnoreCase) == 0)
            {
                if (attributes.ContainsKey("src"))
                {
                    attributes["src"] = RewriteUrl(attributes["src"]);
                }
            }
            base.WriteElement(result, name, attributes, empty);
        }

        private string RewriteUrl(string src)
        {
            var result = src;

            if (!src.StartsWith("http://", true, CultureInfo.InvariantCulture) &&
                !src.StartsWith("https://", true, CultureInfo.InvariantCulture))
            {
                if (!src.StartsWith("/"))
                {
                    src = "/" + src;
                }
                result = _baseUri + src;
            }

            return result;
        }
    }
}
