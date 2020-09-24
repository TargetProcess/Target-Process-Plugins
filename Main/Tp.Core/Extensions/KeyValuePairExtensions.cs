using System.Collections.Generic;

namespace Tp.Core.Extensions
{
    public static class KeyValuePairExtensions
    {
        // ReSharper disable once UseDeconstructionOnParameter
        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> keyValuePair, out TKey key, out TValue value)
        {
            key = keyValuePair.Key;
            value = keyValuePair.Value;
        }
    }
}
