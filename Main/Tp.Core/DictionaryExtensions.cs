using Tp.Core;
using Tp.Core.Annotations;

namespace System.Collections.Generic
{
	[PerformanceCritical]
	public static class DictionaryExtensions
	{
		public static IDictionary<TKey,TValue> WithDefault<TKey,TValue>(this IDictionary<TKey,TValue> d, Func<TKey,TValue> @default)
		{
			return new DictWithDefault<TKey, TValue>(d, @default);
		}

		public class DictWithDefault<TKey, TValue> : IDictionary<TKey, TValue>
		{
			private readonly IDictionary<TKey, TValue> _dictionary;
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

			public int Count
			{
				get { return _dictionary.Count; }
			}

			public bool IsReadOnly
			{
				get { return _dictionary.IsReadOnly; }
			}

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
				if (!_dictionary.TryGetValue(key, out value))
					value = _default(key);
				return true;
			}

			public TValue this[TKey key]
			{
				get
				{
					return _dictionary.GetValue(key).Anyway(()=>_default(key)); }
				set { _dictionary[key] = value; }
			}

			public ICollection<TKey> Keys
			{
				get { return _dictionary.Keys; }
			}

			public ICollection<TValue> Values
			{
				get { return _dictionary.Values; }
			}

			private readonly Func<TKey, TValue> _default;

			public DictWithDefault(IDictionary<TKey, TValue> dictionary, Func<TKey, TValue> @default)
			{
				_dictionary = dictionary;
				_default = @default;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}

		public static TVal GetValueOrDefault<TKey,TVal>(this IDictionary<TKey,TVal> d, TKey k, TVal defVal=default(TVal))
		{
			TVal fetched;
			if(d.TryGetValue(k, out fetched))
			{
				return fetched;
			}
			return defVal;
		}

		public static Maybe<TVal> GetValue<TKey, TVal>(this IDictionary<TKey, TVal> d, TKey k)
		{
			TVal fetched;
			if ((object)k == null)
				return Maybe.Nothing;
			return !d.TryGetValue(k, out fetched) ? Maybe.Nothing : Maybe.Just(fetched);
		}

		public static TVal GetValueFailingVerbose<TKey, TVal>(this IDictionary<TKey, TVal> d, TKey k)
		{
			TVal fetched;
			if (!d.TryGetValue(k, out fetched))
			{
				throw new KeyNotFoundException("Key {0} was not found in map".Fmt(k));
			}
			return fetched;
		}

		public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> valueRetreiver)
		{
			TValue value;
			if (!dictionary.TryGetValue(key, out value))
			{
				value = valueRetreiver(key);
				dictionary.Add(key, value);
			}
			return value;
		}
	}
}
