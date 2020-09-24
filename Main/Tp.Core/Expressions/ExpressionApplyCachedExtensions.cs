using System;
using System.Linq.Expressions;
using Tp.Core.Configuration;

namespace Tp.Core.Expressions
{
    /// <summary>
    /// Provides cached implementation of <see cref="ExpressionExtensions.Apply"/>.
    /// Note that there is no cache eviction, so you should only call these methods for static expressions,
    /// and not dynamically generated ones.
    /// </summary>
    /// <remarks>
    /// `ApplyCached` should only be called when you expect to get the delegate, and not in the context of another expression.
    ///
    /// For example, this won't work:
    /// <code>
    /// Expression&lt;Func&lt;GeneralDto, string&gt;&gt; exp = general => SomeExpr.ApplyCached(general);
    /// </code>
    ///
    /// This is fine though:
    /// <code>
    /// var lambda1 = SomeExpr.ApplyCached(general);
    /// </code>
    /// </remarks>
    public static class ExpressionApplyCachedExtensions
    {
        // It's quite easy to accidentally call .ApplyCached() on dynamically created expression,
        // so we must use LRU cache here to avoid uncontrollable out-of-memory errors.
        // The size of this cache should be monitored in production though.
        // If it ever reaches its limit, it is a sign of bug in code.
        private static readonly ConcurrentLruCache<LambdaExpression, Delegate> _compiledDelegateCache =
            new ConcurrentLruCache<LambdaExpression, Delegate>(
                AppSettingsReader.ReadInt32(
                    "Expressions:Apply:CompiledDelegateCache:Size",
                    10000));

        public static int CacheSize => _compiledDelegateCache.Size;

        public static TResult ApplyCached<T1, TResult>(this Expression<Func<T1, TResult>> e, T1 t1)
        {
            var f = (Func<T1, TResult>) _compiledDelegateCache.GetOrAdd(e, x => x.Compile());
            return f(t1);
        }

        public static TResult ApplyCached<T1, T2, TResult>(this Expression<Func<T1, T2, TResult>> e, T1 t1, T2 t2)
        {
            var f = (Func<T1, T2, TResult>) _compiledDelegateCache.GetOrAdd(e, x => x.Compile());
            return f(t1, t2);
        }

        public static TResult ApplyCached<T1, T2, T3, TResult>(this Expression<Func<T1, T2, T3, TResult>> e, T1 t1, T2 t2, T3 t3)
        {
            var f = (Func<T1, T2, T3, TResult>) _compiledDelegateCache.GetOrAdd(e, x => x.Compile());
            return f(t1, t2, t3);
        }
    }
}
