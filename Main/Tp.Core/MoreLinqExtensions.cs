using System.Collections.Generic;
using Tp.Core;

// ReSharper disable once CheckNamespace

namespace System.Linq
{
	public static class MoreLinqExtensions
	{
		private const string SequenceContainsNoElements = "Sequence contains no elements";

		private class Criterion<TItem, TKey>
		{
			public TItem Item { get; set; }
			public TKey Key { get; set; }
		}

		public static TItem MinBy<TItem, TKey>(this IEnumerable<TItem> items, Func<TItem, TKey> projection, IComparer<TKey> comparer = null)
			where TItem : class
		{
			comparer = comparer ?? Comparer<TKey>.Default;
			return items.ClosestBy(projection, (newKey, closestKey) => comparer.Compare(newKey, closestKey) < 0);
		}

		public static TItem MaxBy<TItem, TKey>(this IEnumerable<TItem> items, Func<TItem, TKey> projection, IComparer<TKey> comparer = null)
			where TItem : class
		{
			comparer = comparer ?? Comparer<TKey>.Default;
			return items.ClosestBy(projection, (newKey, closestKey) => comparer.Compare(newKey, closestKey) > 0);
		}

		private static TItem ClosestBy<TItem, TKey>(this IEnumerable<TItem> items, Func<TItem, TKey> projection, Func<TKey, TKey, bool> isCloser)
		{
			AssertNotNull(items, "items");
			AssertNotNull(projection, "projection");
			return items
				.Aggregate<TItem, Criterion<TItem, TKey>>(null, (closest, item) =>
				{
					var key = projection(item);
					return closest == null || isCloser(key, closest.Key)
						? new Criterion<TItem, TKey> { Item = item, Key = key }
						: closest;
				})
				.NothingIfNull()
				.Select(c => c.Item)
				.GetOrThrow(() => new InvalidOperationException(SequenceContainsNoElements));
		}

		public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> items, Func<TSource, TKey> projection,
			IEqualityComparer<TKey> comparer = null)
		{
			AssertNotNull(items, "items");
			AssertNotNull(projection, "projection");
			var knownKeys = new HashSet<TKey>(comparer);
			foreach (var element in items)
			{
				if (knownKeys.Add(projection(element)))
				{
					yield return element;
				}
			}
		}

		public static IEnumerable<TResult> Generate<TResult>(TResult initial, Func<TResult, TResult> generator)
		{
			AssertNotNull(generator, "generator");
			var current = initial;
			while (true)
			{
				yield return current;
				current = generator(current);
			}
		}

		public static IEnumerable<T> Concat<T>(this T item, IEnumerable<T> items)
		{
			return item.Yield().Concat(items);
		}

		public static IEnumerable<TItem> TakeUntil<TItem>(this IEnumerable<TItem> items, Func<TItem, bool> projection)
		{
			AssertNotNull(items, "items");
			AssertNotNull(projection, "projection");
			foreach (var item in items)
			{
				yield return item;
				if (projection(item))
				{
					yield break;
				}
			}
		}

		public static HashSet<TItem> ToHashSet<TItem>(this IEnumerable<TItem> items, IEqualityComparer<TItem> comparer = null)
		{
			AssertNotNull(items, "items");
			return new HashSet<TItem>(items, comparer);
		}

		private static void AssertNotNull(object value, string paramName)
		{
			if (value == null)
			{
				throw new ArgumentNullException(paramName);
			}
		}
	}
}
