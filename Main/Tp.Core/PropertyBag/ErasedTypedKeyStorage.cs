using System;
using System.Collections.Generic;

namespace Tp.Core.PropertyBag
{
    public class ErasedTypedKeyStorage<T>
    {
        private readonly Dictionary<Tuple<Type,string>, T> _items;

        public ErasedTypedKeyStorage()
        {
            _items = new Dictionary<Tuple<Type,string>, T>();
        }

        public void AddItem(TypedKey key, T item)
        {
            _items[BuildKey(key)] = item;
        }

        public void RemoveItem(TypedKey key)
        {
            _items.Remove(BuildKey(key));
        }

        public Maybe<T> MaybeGetItem(TypedKey key)
        {
            return _items.GetValue(BuildKey(key));
        }

        private Tuple<Type, string> BuildKey(TypedKey key)
        {
            return Tuple.Create(key.Type, key.Name);
        }
    }
}
