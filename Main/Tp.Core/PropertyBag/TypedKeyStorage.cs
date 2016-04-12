using Tp.Core.Annotations;

namespace Tp.Core.PropertyBag
{
	public class TypedKeyStorage<TItemBase> : ErasedTypedKeyStorage<TItemBase>
	{
		public void AddItem<TItem>(TypedKey<TItem> key, TItem item) where TItem : TItemBase
		{
			TypedKey erasedKey = key;
			AddItem(erasedKey, item);
		}

		public void RemoveItem<TItem>(TypedKey<TItem> key)
		{
			TypedKey erasedKey = key;
			RemoveItem(erasedKey);
		}

		[PerformanceCritical]
		public Maybe<TItem> MaybeGetItem<TItem>(TypedKey<TItem> key) where TItem : TItemBase
		{
			TypedKey erasedKey = key;
			Maybe<TItemBase> result = MaybeGetItem(erasedKey);
			if (result.HasValue)
			{
				return Maybe.Return((TItem) result.Value);
			}
			return Maybe.Nothing;
		}
	}
}
