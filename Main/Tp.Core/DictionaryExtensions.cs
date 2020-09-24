using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using Tp.Core;
using Tp.Core.Annotations;

namespace System.Collections.Generic
{
    [PerformanceCritical]
    public static class DictionaryExtensions
    {
        private class DefaultDictionary<TKey, TValue> : IDictionary<TKey, TValue>
        {
            private readonly IDictionary<TKey, TValue> _dictionary;
            private readonly TValue _defaultValue;

            public DefaultDictionary(IDictionary<TKey, TValue> dictionary, TValue defaultValue)
            {
                _dictionary = dictionary;
                _defaultValue = defaultValue;
            }

            public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
            {
                return _dictionary.GetEnumerator();
            }

            public void Add(KeyValuePair<TKey, TValue> item)
            {
                _dictionary.Add(item);
            }

            public void Clear()
            {
                _dictionary.Clear();
            }

            public bool Contains(KeyValuePair<TKey, TValue> item)
            {
                return _dictionary.Contains(item);
            }

            public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
            {
                _dictionary.CopyTo(array, arrayIndex);
            }

            public bool Remove(KeyValuePair<TKey, TValue> item)
            {
                return _dictionary.Remove(item);
            }

            public int Count => _dictionary.Count;

            public bool IsReadOnly => _dictionary.IsReadOnly;

            public bool ContainsKey(TKey key)
            {
                return _dictionary.ContainsKey(key);
            }

            public void Add(TKey key, TValue value)
            {
                _dictionary.Add(key, value);
            }

            public bool Remove(TKey key)
            {
                return _dictionary.Remove(key);
            }

            public bool TryGetValue(TKey key, out TValue value)
            {
                value = this[key];
                return true;
            }

            public TValue this[TKey key]
            {
                get => _dictionary.GetValue(key).GetOrDefault(_defaultValue);
                set => _dictionary[key] = value;
            }

            public ICollection<TKey> Keys => _dictionary.Keys;

            public ICollection<TValue> Values => _dictionary.Values;

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public static Maybe<TVal> GetValue<TKey, TVal>(this Dictionary<TKey, TVal> d, TKey k)
        {
            IDictionary<TKey, TVal> typed = d;
            return typed.GetValue(k);
        }

        public static Maybe<TVal> GetValue<TKey, TVal>(this ConcurrentDictionary<TKey, TVal> d, TKey k)
        {
            IDictionary<TKey, TVal> typed = d;
            return typed.GetValue(k);
        }

        public static Maybe<TVal> GetValue<TKey, TVal>(this IReadOnlyDictionary<TKey, TVal> d, TKey k)
        {
            return k == null
                ? Maybe.Nothing
                : !d.TryGetValue(k, out var fetched) ? Maybe.Nothing : Maybe.Just(fetched);
        }

        public static TVal GetValueFailingVerbose<TKey, TVal>(this IDictionary<TKey, TVal> d, TKey k)
        {
            if (!d.TryGetValue(k, out var fetched))
            {
                throw new KeyNotFoundException("Key {0} was not found in map".Fmt(k));
            }
            return fetched;
        }

        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> valueRetriever)
        {
            if (!dictionary.TryGetValue(key, out var value))
            {
                value = valueRetriever(key);
                dictionary.Add(key, value);
            }
            return value;
        }

        public static IDictionary<TKey, TValue> WithDefaultValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
            TValue defaultValue)
        {
            return new DefaultDictionary<TKey, TValue>(dictionary, defaultValue);
        }

        public static IReadOnlyDictionary<TKey, TValue> ToReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            return new ReadOnlyDictionary<TKey, TValue>(dictionary);
        }

        public static void AddOrThrow<TKey, TValue, TException>([NotNull] this IDictionary<TKey, TValue> dictionary, TKey key, TValue value,
            [NotNull] Func<ArgumentException, TException> converter) where TKey : class where TException : Exception
        {
            try
            {
                dictionary.Add(key, value);
            }
            catch (ArgumentException ex)
            {
                throw converter(ex);
            }
        }
    }
}
