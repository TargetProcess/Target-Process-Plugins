using System.Runtime.CompilerServices;

namespace Tp.Core
{
	public static class ExtensionsProvider
	{
		private static readonly ConditionalWeakTable<object, object> _table = new ConditionalWeakTable<object, object>();
		public static Maybe<T> GetValue<T>(object key)
		{
			object value;
			return _table.TryGetValue(key, out value) ? Maybe.Return((T)value) : Maybe.Nothing;
		}

		public static void SetValue<T>(object key, T value)
		{
			_table.Add(key, value);
		}
	}
}
