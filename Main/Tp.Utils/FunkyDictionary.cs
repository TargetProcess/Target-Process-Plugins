using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Tp.Components
{
	/// <summary>
	/// Overrides indexer getter to return <c>null</c> instead of throwing exception
	/// if key not found.
	/// </summary>
	/// <typeparam name="K">Key type.</typeparam>
	/// <typeparam name="V">Value type.</typeparam>
	[Serializable]
	public class FunkyDictionary<K, V> : IDictionary<K, V> where V : class
	{
		private readonly IDictionary<K, V> _data;

		public FunkyDictionary()
		{
			_data = new Dictionary<K, V>();
		}

		public FunkyDictionary(IDictionary<K, V> data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			_data = data;
		}

		[System.Diagnostics.DebuggerStepThrough]
		public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
		{
			return _data.GetEnumerator();
		}

		[System.Diagnostics.DebuggerStepThrough]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		[System.Diagnostics.DebuggerStepThrough]
		public void Add(KeyValuePair<K, V> item)
		{
			_data.Add(item);
		}

		[System.Diagnostics.DebuggerStepThrough]
		public void Clear()
		{
			_data.Clear();
		}

		[System.Diagnostics.DebuggerStepThrough]
		public bool Contains(KeyValuePair<K, V> item)
		{
			return _data.Contains(item);
		}

		[System.Diagnostics.DebuggerStepThrough]
		public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
		{
			_data.CopyTo(array, arrayIndex);
		}

		[System.Diagnostics.DebuggerStepThrough]
		public bool Remove(KeyValuePair<K, V> item)
		{
			return _data.Remove(item);
		}

		public int Count
		{
			[System.Diagnostics.DebuggerStepThrough]
			get { return _data.Count; }
		}

		public bool IsReadOnly
		{
			[System.Diagnostics.DebuggerStepThrough]
			get { return _data.IsReadOnly; }
		}

		[System.Diagnostics.DebuggerStepThrough]
		public bool ContainsKey(K key)
		{
			return _data.ContainsKey(key);
		}

		[System.Diagnostics.DebuggerStepThrough]
		public void Add(K key, V value)
		{
			_data.Add(key, value);
		}

		[System.Diagnostics.DebuggerStepThrough]
		public bool Remove(K key)
		{
			return _data.Remove(key);
		}

		[System.Diagnostics.DebuggerStepThrough]
		public bool TryGetValue(K key, out V value)
		{
			return _data.TryGetValue(key, out value);
		}

		/// <summary>
		/// Instead of throwing exception, will return <c>null</c> in getter 
		/// if key not found.
		/// </summary>
		/// <param name="key">Key whose value to look up.</param>
		/// <returns>Value for the specified key, or <c>null</c> if key not found.</returns>
		public V this[K key]
		{
			[System.Diagnostics.DebuggerStepThrough]
			get { return _data.ContainsKey(key) ? _data[key] : null; }
			[System.Diagnostics.DebuggerStepThrough]
			set { _data[key] = value; }
		}

		public ICollection<K> Keys
		{
			[System.Diagnostics.DebuggerStepThrough]
			get { return _data.Keys; }
		}

		public ICollection<V> Values
		{
			[System.Diagnostics.DebuggerStepThrough]
			get { return _data.Values; }
		}
	}
}