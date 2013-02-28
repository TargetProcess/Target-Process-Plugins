using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Tp.Core
{
	public class MultiDictionary<T> : IEnumerable<KeyValuePair<T, IEnumerable<T>>>
	{
		private readonly IEqualityComparer<T> _comparer;

		public MultiDictionary(IEqualityComparer<T> comparer)
		{
			_comparer = comparer;
		}

		public MultiDictionary() : this(EqualityComparer<T>.Default)
		{
		}

		readonly Dictionary<T, HashSet<T>> _dictionary = new Dictionary<T, HashSet<T>>();

		#region Implementation of IEnumerable

		public IEnumerator<KeyValuePair<T, IEnumerable<T>>> GetEnumerator()
		{
			return _dictionary.Select(x => new KeyValuePair<T, IEnumerable<T>>(x.Key,x.Value)).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		public void Add(Tuple<T, T> pair)
		{
			AddToDictionary(pair.Item1, pair.Item2);
			AddToDictionary(pair.Item2, pair.Item1);
		}

		public void AddRange(IEnumerable<Tuple<T, T>> pairs)
		{
			foreach (var pair in pairs)
			{
				AddToDictionary(pair.Item1, pair.Item2);
				AddToDictionary(pair.Item2, pair.Item1);
			}
		}

		private void AddToDictionary(T key, T value)
		{
			var hashSet = _dictionary.GetOrAdd(key, _ => new HashSet<T>(_comparer));
			hashSet.Add(value);
		}

		public Maybe<IEnumerable<T>> MaybeGetValue(T key)
		{
			return _dictionary.GetValue(key).Bind(hashset => hashset.AsEnumerable());
		}
	}
}