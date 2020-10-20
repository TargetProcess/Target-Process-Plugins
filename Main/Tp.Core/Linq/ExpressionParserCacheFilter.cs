using System.Linq;
using Tp.Core.Annotations;
using Tp.Core.Configuration;

namespace Tp.Core.Linq
{
    public delegate bool ExpressionParserCacheFilter([NotNull] string input);

    public static class ExpressionParserCacheFilters
    {
        /// <summary>
        /// Input strings shorter than this value won't be put into cache
        /// if they also contain literals like numbers or strings.
        /// </summary>
        private static readonly int _shortInputLength = AppSettingsReader.ReadInt32(
            "ExpressionParser:LruCache:ShortInputLength",
            // There is a common pattern of client storage access filters like `((key=="user43322-5560931232390684936"))`
            // It doesn't make much sense to cache such expressions.
            45);

        public static readonly ExpressionParserCacheFilter SkipShort = input =>
        {
            if (input.Length >= _shortInputLength)
            {
                return true;
            }

            var containsLiterals = input.Any(c => c == '"' || c == '\'' || char.IsDigit(c));
            if (containsLiterals)
            {
                // It's a short string containing literals,
                // most likely it's something like `id == 2320` or `x in ['A', 'B']`.
                // No sense to put such expressions in cache - they are fast enough to parse,
                // and there is a high chance they will be evicted from cache soon anyway.
                return false;
            }

            return true;
        };
    }
}
