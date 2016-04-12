using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Tp.Core.Annotations;

namespace Tp.Core
{
	public static class ExtensionsProvider
	{
		private static readonly ConditionalWeakTable<object, ConcurrentDictionary<string, object>> _table =
			new ConditionalWeakTable<object, ConcurrentDictionary<string, object>>();

		public static Maybe<T> GetValue<T>([NotNull] object root, [NotNull] string key)
		{
			if (root == null) throw new ArgumentNullException(nameof(root));
			if (key == null) throw new ArgumentNullException(nameof(key));

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

		public static void SetValue<T>([NotNull] object root, [NotNull] string key, T value)
		{
			if (root == null) throw new ArgumentNullException(nameof(root));
			if (key == null) throw new ArgumentNullException(nameof(key));
			var dictionary = _table.GetOrCreateValue(root);
			dictionary.AddOrUpdate(key, value, (x, y) => value);
		}

		public static T GetOrCreateValue<T>([NotNull] object root, [NotNull] string key, Func<T> valueFactory)
		{
			if (root == null) throw new ArgumentNullException(nameof(root));
			if (key == null) throw new ArgumentNullException(nameof(key));

			var bag = _table.GetOrCreateValue(root);
			return (T) bag.GetOrAdd(key, _ => valueFactory());
		}

		public static void SetValue<T>([NotNull] object root, T value)
		{
			if (root == null) throw new ArgumentNullException(nameof(root));
			SetValue(root, typeof(T).Name, value);
		}

		public static Maybe<T> GetValue<T>([NotNull] object root)
		{
			if (root == null) throw new ArgumentNullException(nameof(root));
			return GetValue<T>(root, typeof(T).Name);
		}
	}
}
