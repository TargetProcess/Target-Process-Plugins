using System;
using System.Collections.Generic;

namespace Tp.Core.PropertyBag
{
	public class ErasedTypedKeyStorage<T>
	{
		private readonly Dictionary<Type, T> _items;

		public ErasedTypedKeyStorage()
		{
			_items = new Dictionary<Type, T>();
		}

		public void AddItem(TypedKey key, T item)
		{
			_items[key.Type] = item;
		}

		public void RemoveItem(TypedKey key)
		{
			_items.Remove(key.Type);
		}

		public Maybe<T> MaybeGetItem(TypedKey key)
		{
			return _items.GetValue(key.Type);
		}
	}
}
