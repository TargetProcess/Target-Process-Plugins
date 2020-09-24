using System.Text.RegularExpressions;

namespace Tp.Core.Web
{
    public static class WeakETag
    {
        private static readonly Regex _weakETagPattern = new Regex("^W\\/\"(.+)\"$", RegexOptions.Compiled);

        public static string Create(string value) => $"W/\"{value}\"";

        public static bool TryParse(string weakValue, out string etagBody)
        {
            var match = _weakETagPattern.Match(weakValue);
            etagBody = match.Success ? match.Groups[1].Value : default;
            return match.Success;
        }
    }
}
