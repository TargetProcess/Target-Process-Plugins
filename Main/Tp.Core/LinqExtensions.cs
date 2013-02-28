using System.Collections.Generic;
using Tp.Core;

namespace System.Linq
{
	public static class LinqExtensions
	{
		public static IEnumerable<T> Union<T, TItem> (this IEnumerable<T> first, IEnumerable<T> second, Func<T,TItem> itemToCompare)
		{
			return first.Union(second, LambdaComparer<T>.Equality(itemToCompare));
		}
	}
}