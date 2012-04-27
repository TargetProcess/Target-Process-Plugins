using Tp.Core;

namespace System.Collections.Generic
{
	public static class DictionaryExtensions
	{
		public static TVal GetValueOrDefault<TKey,TVal>(this IDictionary<TKey,TVal> d, TKey k, TVal defVal)
		{
			return GetValue(d, k, v => v, _ => defVal);
		}

		public static Maybe<TVal> GetValue<TKey, TVal>(this IDictionary<TKey, TVal> d, TKey k)
		{
			return GetValue(d, k, Maybe.Just, _ => Maybe.Nothing);
		}

		public static TVal GetValueFailingVerbose<TKey, TVal>(this IDictionary<TKey, TVal> d, TKey k)
		{
			return GetValue(d, k, v => v, keyNotFound => { throw new KeyNotFoundException("Key {0} was not found in map".Fmt(k)); });
		}

		private static TValReturn GetValue<TKey, TVal, TValReturn>(this IDictionary<TKey, TVal> d, TKey k, Func<TVal, TValReturn> success, Func<TKey, TValReturn> fail)
		{
			TVal v;
			return d.TryGetValue(k, out v) ? success(v) : fail(k);
		}
	}
}
