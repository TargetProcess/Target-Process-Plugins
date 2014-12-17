using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Tp.Core
{
	public class MaybeDictionary<TKey, TValue> : Dictionary<TKey,TValue>, IDictionary<TKey, Maybe<TValue>>
	{
		public MaybeDictionary(IEqualityComparer<TKey> comparer=null):base(comparer??EqualityComparer<TKey>.Default)
		{
		}

		public MaybeDictionary(IDictionary<TKey,TValue> other) : base(other)
		{
			
		}


		IEnumerator<KeyValuePair<TKey, Maybe<TValue>>> IEnumerable<KeyValuePair<TKey, Maybe<TValue>>>.GetEnumerator()
		{
			return
				((IEnumerable<KeyValuePair<TKey, TValue>>) this).Select(
					x => new KeyValuePair<TKey, Maybe<TValue>>(x.Key, Maybe.Just(x.Value))).GetEnumerator();
		}

		public void Add(KeyValuePair<TKey, Maybe<TValue>> item)
		{
			item.Value.Do(value => Add(item.Key, value));
		}

		public bool Contains(KeyValuePair<TKey, Maybe<TValue>> item)
		{
			if (item.Value == Maybe.Nothing)
				return false;

			return ((IDictionary<TKey, TValue>) this).Contains(new KeyValuePair<TKey, TValue>(item.Key, item.Value.Value));
		}

		public void CopyTo(KeyValuePair<TKey, Maybe<TValue>>[] array, int arrayIndex)
		{
			throw new System.NotImplementedException();
		}

		public bool Remove(KeyValuePair<TKey, Maybe<TValue>> item)
		{
			if (item.Value == Maybe.Nothing)
				return false;
			return ((IDictionary<TKey, TValue>) this).Remove(new KeyValuePair<TKey, TValue>(item.Key, item.Value.Value));
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		#region Implementation of IDictionary<TKey,Maybe<TValue>>

		public void Add(TKey key, Maybe<TValue> value)
		{
			if (value!=Maybe.Nothing)
			{
				base.Add(key,value.Value);
			}
		}

		public bool TryGetValue(TKey key, out Maybe<TValue> maybeValue)
		{
			TValue value;
			bool result = TryGetValue(key, out value);
			maybeValue = result ? Maybe.Just(value) : Maybe.Nothing;
			return result;
		}

		public new Maybe<TValue> this[TKey key]
		{
			get { return this.GetValue<TKey, TValue>(key); }
			set
			{
				if (value!=Maybe.Nothing)
				{
					base[key] = value.Value;
				}
				else
				{
					if (ContainsKey(key))
						Remove(key);
				}

			}
		}

		ICollection<TKey> IDictionary<TKey, Maybe<TValue>>.Keys
		{
			get { return Keys; }
		}

		public new ICollection<Maybe<TValue>> Values
		{
			get { return base.Values.Select(x => Maybe.Just(x)).ToList().AsReadOnly(); }
		}

		#endregion
	}
}