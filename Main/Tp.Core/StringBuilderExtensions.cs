using System.Collections.Generic;

namespace System.Text
{
	public static class StringBuilderExtensions
	{
		public static StringBuilder Dump<T>(this StringBuilder builder, IEnumerable<T> items, Action<StringBuilder, T> itemMessageProvider)
		{
			foreach (T item in items)
			{
				itemMessageProvider(builder, item);
			}
			return builder;
		}
	}
}
