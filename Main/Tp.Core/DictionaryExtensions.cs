using Tp.Core;

namespace System.Collections.Generic
{
	public static class DictionaryExtensions
	{
		public static TValue GetValueOrDefault<TKey,TValue>(this IDictionary<TKey,TValue> map, TKey key, TValue defaultValue)
		{
			TValue maybeValue;
			return !map.TryGetValue(key, out maybeValue)
					? defaultValue 
					: maybeValue;
		}

		public static Maybe<TValue> GetValue<TKey, TValue>(this IDictionary<TKey, TValue> map, TKey key)
		{
			TValue maybeValue;
			return map.TryGetValue(key, out maybeValue)
					? Maybe.Just(maybeValue)
					: Maybe.Nothing;
		}
	}
}
