using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace Tp.Core
{
	public static class ExtensionsProvider
	{
		private static readonly ConditionalWeakTable<object, ConcurrentDictionary<string, object>> _table = new ConditionalWeakTable<object, ConcurrentDictionary<string, object>>();

		public static Maybe<T> GetValue<T>(object root, string key)
		{
			ConcurrentDictionary<string, object> bag;
			if (_table.TryGetValue(root, out bag))
			{
				object value;
				if (bag.TryGetValue(key, out value))
				{
					return value.MaybeAs<T>();
				} 
			}
			return Maybe.Nothing;
		}

		public static void SetValue<T>(object root, string key, T value)
		{
			var dictionary = _table.GetOrCreateValue(root);
			dictionary.AddOrUpdate(key, value, (x, y) => value);
		}

		public static T GetOrCreateValue<T>(object root, string key, Func<T> valueFactory)
		{
			var bag = _table.GetOrCreateValue(root);
			return (T) bag.GetOrAdd(key, _ => valueFactory());
		}

		public static void SetValue<T>(object root, T value)
		{
			SetValue(root, typeof(T).Name, value);
		}
		
		public static Maybe<T> GetValue<T>(object root)
		{
			return GetValue<T>(root, typeof (T).Name);
		}
	}
}
