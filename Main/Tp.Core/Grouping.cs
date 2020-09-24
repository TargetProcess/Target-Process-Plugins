using System.Collections;
using System.Collections.Generic;

namespace System.Linq
{
    public class Grouping<TKey, TItem> : IGrouping<TKey, TItem>
    {
        private readonly IEnumerable<TItem> _items;

        public Grouping(TKey key, IEnumerable<TItem> items)
        {
            _items = items;
            Key = key;
        }

        public IEnumerator<TItem> GetEnumerator() => _items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public TKey Key { get; }
    }
}
