using System;
using System.Text;
using Tp.Core.Annotations;

namespace Tp.Core.Web
{
    public static class ETagCalculator
    {
        private static readonly Encoding _encoding = new UTF8Encoding(false);

        [Pure]
        [NotNull]
        public static string CalculateETag([NotNull] string content)
        {
            var bytes = _encoding.GetBytes(content);
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var etagBytes = sha256.ComputeHash(bytes);
                var etag = Convert.ToBase64String(etagBytes);
                return etag;
            }
        }
    }
}
