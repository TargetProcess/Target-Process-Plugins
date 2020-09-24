using System;
using System.Security.Cryptography;
using Tp.Core.Annotations;

namespace Tp.Utils
{
    public static class UniqueTokenGenerator
    {
        [Pure]
        public static string Generate(int size)
        {
            var bytes = new byte[size];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(bytes);
            }
            return Convert.ToBase64String(bytes);
        }
    }
}
