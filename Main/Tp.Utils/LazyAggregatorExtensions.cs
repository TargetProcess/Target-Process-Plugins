using System.Linq;

namespace System.Collections.Generic
{
    public static class LazyAggregatorExtensions
    {
        public static LazyAggregator<T> LazyLastOrDefault<TRecord, T>(this IEnumerable<TRecord> records,
            Func<TRecord, T> selector)
        {
            var enumerable = records.Select(selector);
            return new LazyAggregator<T>(enumerable, items => items.LastOrDefault(x => !Equals(x, null)));
        }
    }
}
