using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Tp.Core.Annotations;

namespace Tp.Core.Linq
{
    public static class ExpressionStringHasher
    {
        // SHA256 implementation is not thread-safe.
        // Profiling shows that using thread-local values is a little bit faster than creating new SHA256 instances every time,
        // and it also allocates less memory.
        private static ThreadLocal<SHA256> _hasher = new ThreadLocal<SHA256>(SHA256.Create);

        public static readonly int HashLength;

        static ExpressionStringHasher()
        {
            HashLength = CalculateHash(new[] { "test" }).Length;
        }

        [Pure]
        [NotNull]
        public static string CalculateHash([NotNull] [ItemNotNull] IEnumerable<string> cacheKeyParts)
        {
            var encodedBytes = Encoding.UTF8.GetBytes(string.Join(";", cacheKeyParts));
            var computed = _hasher.Value.ComputeHash(encodedBytes);
            return ToString(computed);
        }

        /// <summary>
        /// Extracted from <see cref="BitConverter.ToString(byte[],int,int)"/>.
        /// Doesn't include "-" separators between hex values.
        /// </summary>
        private static string ToString(byte[] value)
        {
            var length1 = value.Length * 2;
            var chArray = new char[length1];
            var num1 = 0;
            for (var index = 0; index < length1; index += 2)
            {
                var num2 = value[num1++];
                chArray[index] = GetHexValue(num2 / 16);
                chArray[index + 1] = GetHexValue(num2 % 16);
            }
            return new string(chArray, 0, chArray.Length - 1);
        }

        private static char GetHexValue(int i)
        {
            return i < 10 ? (char) (i + 48) : (char) (i - 10 + 65);
        }
    }
}
