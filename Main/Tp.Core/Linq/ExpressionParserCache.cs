using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Threading;
using log4net;
using Tp.Core.Annotations;
using Tp.Core.Configuration;

namespace Tp.Core.Linq
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public class ExpressionParserCache
    {
        public static readonly ILog Log = TpLogManager.Instance.GetLog(typeof(ExpressionParserCache));

        private readonly ConcurrentLruCache<string, LambdaExpression> _parsedExpressions =
            new ConcurrentLruCache<string, LambdaExpression>(
                // How many parsed expressions will be stored per account in LRU cache across all accounts.
                // Note that storing keys and values is somewhat expensive in terms of RAM (rough estimate is 1 Kb per entry),
                // so don't set it too high.
                AppSettingsReader.ReadInt32("ExpressionParser:LruCache:Size", 250000),
                StringComparer.Ordinal);

        /// <summary>
        /// Input strings shorter than this value won't be put into cache
        /// if they also contain literals like numbers or strings.
        /// See <see cref="ShouldCache"/> for details.
        /// </summary>
        private static readonly int _shortInputLength = AppSettingsReader.ReadInt32(
            "ExpressionParser:LruCache:ShortInputLength",
            // There is a common pattern of client storage access filters like `((key=="user43322-5560931232390684936"))`
            // It doesn't make much sense to cache such expressions.
            // It would be more elegant to use non-caching parser from ClientStorageController,
            // but it requires much more effort than this dumb and naive length filter.
            // Let's use it and see how many false positive rejections we get with such length.
            45);

        private readonly ConcurrentLruCache<string, (IReadOnlyList<DynamicOrdering>, ParameterExpression Parameter)> _parsedOrderings =
            new ConcurrentLruCache<string, (IReadOnlyList<DynamicOrdering>, ParameterExpression Parameter)>(
                // Ordering inputs don't have such variety as regular lambda expressions,
                // so we can keep the cache size quite low.
                AppSettingsReader.ReadInt32("ExpressionParser:OrderingLruCache:Size", 1000));

        private long _totalGetLambda;

        public long TotalGetLambda => _totalGetLambda;

        private long _nonCachedGetLambda;

        public long NonCachedGetLambda => _nonCachedGetLambda;

        private long _totalGetOrdering;

        public long TotalGetOrdering => _totalGetOrdering;

        private long _nonCachedGetOrdering;

        public long NonCachedGetOrdering => _nonCachedGetOrdering;

        public int Size => _parsedExpressions.Size;

        public int OrderingSize => _parsedOrderings.Size;

        public LambdaExpression GetOrAdd(
            [NotNull] string[] cacheKeyParts, [NotNull] Func<LambdaExpression> get)
        {
            Interlocked.Increment(ref _totalGetLambda);

            var cacheKey = BuildCacheKey(cacheKeyParts);

            return _parsedExpressions.GetOrAdd(cacheKey, _ =>
            {
                Interlocked.Increment(ref _nonCachedGetLambda);
                return get();
            });
        }

        /// <remarks>
        /// Text input for orderings don't have such variety as regular lambda expressions,
        /// so we can use a naive small cache for them.
        ///
        /// If for some reason this cache starts growing in RAM size or starts evicting entries too often,
        /// we will need to make it behave similar to <see cref="_parsedExpressions"/>.
        /// </remarks>
        public (IReadOnlyList<DynamicOrdering>, ParameterExpression Parameter) GetOrAdd(
            string key,
            Func<(IReadOnlyList<DynamicOrdering>, ParameterExpression Parameter)> get)
        {
            Interlocked.Increment(ref _totalGetOrdering);
            return _parsedOrderings.GetOrAdd(key, _ =>
            {
                Interlocked.Increment(ref _nonCachedGetOrdering);
                return get();
            });
        }

        public void Clear()
        {
            _parsedExpressions.Clear();
            _parsedOrderings.Clear();
        }

        /// <remarks>
        /// We expect this cache to contain a lot of entries (see <see cref="_parsedExpressions"/> init for capacity).
        /// Turns out that string used as cache keys can easily become quite large, and start consuming a lot of RAM.
        /// Assuming a rough 2x multiplier, a typical cache key of 350 symbols uses about 700 bytes.
        /// Keeping 250.000 such keys in memory thus requires ~167 MB.
        /// And we also need to add memory for stored expressions, and LRU internal structure overhead.
        ///
        /// So we need to make cache keys as short as possible.
        ///
        /// Every cache key contains environment prefix, and the cached expression as well:
        ///
        /// |----------------- prefix -------------------------|---------- user input ----------------|
        /// account;customfields;extendabledomain;parseroptions;{id,name,effort,bug_count:Bugs.Count()}
        ///
        /// Even if we can minimize the prefix length, we can't control the size of user input.
        /// Thus, we have to use a hash function like SHA256 to produce a cache key of a limited size.
        /// This kills debuggability, but it's good for RAM.
        ///
        /// This method also does a minor optimization and avoids computing hash when original cache key is short enough.
        /// </remarks>
        /// <param name="cacheKeyParts">
        /// Array of information bits which should be joined to build a cache key: environment info (custom field versions, parser options),
        /// and the input expression as well.
        /// </param>
        private static string BuildCacheKey(string[] cacheKeyParts)
        {
            var unhashedLength = cacheKeyParts.Sum(s => s.Length) + (cacheKeyParts.Length - 1);
            if (unhashedLength < ExpressionStringHasher.HashLength)
            {
                return string.Join(";", cacheKeyParts);
            }

            return ExpressionStringHasher.CalculateHash(cacheKeyParts);
        }

        [Pure]
        public static bool ShouldCache([NotNull] string input)
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
        }
    }
}
