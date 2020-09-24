// Inspired by Microsoft.CodeAnalysis.InternalUtilities.ConcurrentLruCache

using System;
using System.Collections.Generic;
using Tp.Core.Extensions;

namespace Tp.Core
{
    /// <summary>
    /// Cache with a fixed size that evicts the least recently used members.
    /// </summary>
    public class ConcurrentLruCache<TKey, TValue>
    {
        private readonly object _lockObject = new object();
        private readonly int _capacity;
        private readonly Dictionary<TKey, CacheValue> _cache;
        private readonly LinkedList<TKey> _nodeList;

        public ConcurrentLruCache(int capacity, IEqualityComparer<TKey> keyComparer = null)
        {
            if (capacity <= 0) throw new ArgumentOutOfRangeException(nameof(capacity));
            _capacity = capacity;
            _cache = new Dictionary<TKey, CacheValue>(
                // If capacity is low, we can pre-allocate entire dictionary.
                // Otherwise, let's keep its size dynamic.
                Math.Min(capacity, 100),
                keyComparer);
            _nodeList = new LinkedList<TKey>();
        }

        public ConcurrentLruCache(IEnumerable<KeyValuePair<TKey, TValue>> items, int capacity, IEqualityComparer<TKey> keyComparer = null)
            : this(capacity, keyComparer)
        {
            foreach (var (key, value) in items)
            {
                UnsafeAdd(key, value);
            }
        }

        public int Size
        {
            get
            {
                lock (_lockObject)
                {
                    return _cache.Count;
                }
            }
        }

        [Obsolete("Do not use in production")]
        public IReadOnlyList<(TKey Key, TValue Value)> GetAllDataForTestsOnly()
        {
            lock (_lockObject)
            {
                var keyValuePairArray = new (TKey, TValue)[_cache.Count];
                var num = 0;
                foreach (var node in _nodeList)
                {
                    keyValuePairArray[num++] = (node, _cache[node].Value);
                }
                return keyValuePairArray;
            }
        }

        public void Clear()
        {
            lock (_lockObject)
            {
                _cache.Clear();
                _nodeList.Clear();
            }
        }

        public bool TryAdd(TKey key, TValue value)
        {
            lock (_lockObject)
            {
                return UnsafeAdd(key, value);
            }
        }

        private void MoveNodeToTop(LinkedListNode<TKey> node)
        {
            if (_nodeList.First == node)
            {
                return;
            }

            _nodeList.Remove(node);
            _nodeList.AddFirst(node);
        }

        private void UnsafeEvictLastNode()
        {
            var last = _nodeList.Last;
            _nodeList.Remove(last);
            _cache.Remove(last.Value);
        }

        private void UnsafeAddNodeToTop(TKey key, TValue value)
        {
            var node = new LinkedListNode<TKey>(key);
            _cache.Add(key, new CacheValue()
            {
                Node = node,
                Value = value
            });
            _nodeList.AddFirst(node);
        }

        private bool UnsafeAdd(TKey key, TValue value)
        {
            if (_cache.TryGetValue(key, out _))
            {
                return false;
            }

            if (_cache.Count == _capacity)
            {
                UnsafeEvictLastNode();
            }

            UnsafeAddNodeToTop(key, value);
            return true;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            lock (_lockObject)
            {
                return UnsafeTryGetValue(key, out value);
            }
        }

        private bool UnsafeTryGetValue(TKey key, out TValue value)
        {
            if (_cache.TryGetValue(key, out var cacheValue))
            {
                MoveNodeToTop(cacheValue.Node);
                value = cacheValue.Value;
                return true;
            }

            value = default;
            return false;
        }

        public TValue GetOrAdd(TKey key, TValue value)
        {
            lock (_lockObject)
            {
                if (UnsafeTryGetValue(key, out var v))
                {
                    return v;
                }
                UnsafeAdd(key, value);
                return value;
            }
        }

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> creator)
        {
            lock (_lockObject)
            {
                if (UnsafeTryGetValue(key, out var existing))
                {
                    return existing;
                }
                var created = creator(key);
                UnsafeAdd(key, created);
                return created;
            }
        }

        private struct CacheValue
        {
            public TValue Value;
            public LinkedListNode<TKey> Node;
        }
    }
}
