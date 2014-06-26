using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace System
{
	public static class NullableExtensions
	{
		public static TTo? Bind<T, TTo>(this T? value, Func<T, TTo?> func) where T : struct where TTo : struct
		{
			return value.HasValue ? func(value.Value) : null;
		}

		public static TTo Extract<T, TTo>(this T? value, Func<T, TTo> func) where T : struct where TTo : class
		{
			return value.HasValue ? func(value.Value) : null;
		}
		
		public static TTo? Map<T, TTo>(this T? value, Func<T, TTo> func) where T : struct where TTo : struct
		{
			return value.HasValue ? func(value.Value) : (TTo?) null;
		}
		
		public static IEnumerable<TTo> Choose<T, TTo>(this IEnumerable<T> xs, Func<T, TTo?> map) where TTo : struct 
		{
			return xs
				.Select(map)
				.Where(x => x.HasValue)
// ReSharper disable once PossibleInvalidOperationException
				.Select(x => x.Value);
		}
	}
}