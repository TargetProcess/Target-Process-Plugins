using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Linq
{
	public static class QueryableExtensions
	{
		public static IQueryable<GroupWithCount<TKey>> CountBy<T, TKey>(this IQueryable<T> source,
		                                                                Expression<Func<T, TKey>> keySelector)
		{
			var q = source.Provider.CreateQuery<GroupWithCount<TKey>>(
				Expression.Call(
					null,
					((MethodInfo) MethodBase.GetCurrentMethod()).MakeGenericMethod(typeof (T), typeof (TKey)),
					new[] {source.Expression, Expression.Quote(keySelector)}
					));

			return q;
		}

		public static IQueryable<IGrouping<TKey, TResult>> PartitionBy<T, TKey, TResult>(this IQueryable<T> source,
		                                                                                 Expression<Func<T, TKey>> keySelector,
		                                                                                 Expression<Func<T, TResult>>
			                                                                                 elementSelector, int skip, int take)
		{
			var q = source.Provider.CreateQuery<IGrouping<TKey, TResult>>(
				Expression.Call(
					null,
					((MethodInfo) MethodBase.GetCurrentMethod()).MakeGenericMethod(typeof (T), typeof (TKey), typeof (TResult)),
					new[]
						{
							source.Expression,
							Expression.Quote(keySelector),
							Expression.Quote(elementSelector),
							Expression.Constant(skip),
							Expression.Constant(take)
						}
					));

			return q;
		}

		public static IQueryable<IGrouping<TKey, T>> PartitionBy<T, TKey>(this IQueryable<T> source,
		                                                                  Expression<Func<T, TKey>> keySelector, int skip,
		                                                                  int take)
		{
			return source.PartitionBy(keySelector, x => x, skip, take);
		}

		public static IEnumerable<T> Page<T>(this IQueryable<T> items, int? skip, int? take, out bool hasNextPage,
		                                     int defaultTake = 25)
		{
			if (take == null && skip == null)
			{
				hasNextPage = false;
				return items;
			}

			int takeValue = take.GetValueOrDefault(defaultTake);
			int skipValue = skip.GetValueOrDefault(0);


			var page = items.Skip(skipValue).Take(takeValue + 1).ToList();
			if (page.Count == takeValue + 1)
			{
				hasNextPage = true;
				page.RemoveAt(takeValue);
			}
			else
			{
				hasNextPage = false;
			}
			return page;
		}


		public static IEnumerable<GroupWithCount<TKey>> CountBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
		{
			return source.GroupBy(keySelector).Select(x => new GroupWithCount<TKey>(x.Key, x.Count()));
		}


		public static IEnumerable<IGrouping<TKey, T>> PartitionBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector, int skip, int take)
		{
			return source.PartitionBy(keySelector, x => x, skip, take);
		}

		public static IEnumerable<IGrouping<TKey, TElement>> PartitionBy<TSource, TKey, TElement>(this IEnumerable<TSource> source,
			Func<TSource, TKey> keySelector,
			Func<TSource, TElement> elementSelector,
			int skip, int take)
		{
			IEnumerable<IGrouping<TKey, TElement>> q =
				source.GroupBy(keySelector, elementSelector);

			if (typeof(TKey).GetInterface("IComparable") != null)
				q = q.OrderBy(g => g.Key);

			q = q.Select(g => (IGrouping<TKey, TElement>)new Group<TKey, TElement>(g, skip, take)).Where(g => g.Any());

			return q;
		}

		private class Group<TKey, T> : IGrouping<TKey, T>
		{
			private readonly IEnumerable<T> _grouping;
			private readonly TKey _key;

			public Group(IGrouping<TKey, T> grouping, int skip, int take)
			{
				_grouping = grouping.Skip(skip).Take(take);
				_key = grouping.Key;
			}


			public IEnumerator<T> GetEnumerator()
			{
				return _grouping.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			public TKey Key
			{
				get { return _key; }
			}

			public override string ToString()
			{
				return "{{Key:{0}, [{1}]}}".Fmt(Key, this.ToString(" ,"));
			}
		}

	}

}