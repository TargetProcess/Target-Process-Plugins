// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using JetBrains.Annotations;
using Tp.Core;

namespace System.Linq
{
	public static class EnumerableExtensions
	{
		// for generic interface IEnumerable<T>
		public static string ToString<T>(this IEnumerable<T> source,[NotNull] Func<T, string> selector, string separator)
		{
			if (source == null)
				return String.Empty;

			if (String.IsNullOrEmpty(separator))
				throw new ArgumentException("Parameter separator can not be null or empty.");

			return source.Where(x => !Equals(x, null)).Aggregate(new StringBuilder(),
																	 (sb, x) => sb.Append(separator).Append(selector(x)),
																	 x => (x.Length > 0 ? x.Remove(0, separator.Length) : x).ToString());
		}

		public static string ToString<T>(this IEnumerable<T> source, string separator)
		{
			return source.ToString(x => x.ToString(), separator);
		}

		// for interface IEnumerable
		public static string ToString(this IEnumerable source, string separator)
		{
			if (source == null)
				return String.Empty;

			if (String.IsNullOrEmpty(separator))
				throw new ArgumentException("Parameter separator can not be null or empty.");

			return source.Cast<object>().ToString(separator);
		}

		
		public static string ToSqlString<T>(this IEnumerable<T> ids)
		{
			if (ids == null || !ids.Any())
				return " ( null )";

			return String.Format(" ({0}) ", String.Join(",", ids.Select(x => x.ToString()).ToArray()));
		}

		public static bool IsOrdered<T>(this IEnumerable<T> items)
		{
			return IsOrdered(items, Comparer<T>.Default);
		}

		public static bool IsOrdered<T>(this IEnumerable<T> items, Comparer<T> comparer)
		{
			return IsOrdered(items, comparer.Compare);
		}
		public static bool IsOrdered<T>(this IEnumerable<T> items, Comparison<T> comparison)
		{
			using (var enumerator = items.GetEnumerator())
			{
				if (!enumerator.MoveNext())
					return true;
				var previousItem = enumerator.Current;
				while (enumerator.MoveNext())
				{
					var currentItem = enumerator.Current;
					if (comparison(previousItem, currentItem) > 0)
						return false;
					previousItem = currentItem;
				}
			}
			return true;
		}
	
		
		public static T FirstOrDefault<T>(this IEnumerable<T> source, T defaultValue)
		{
			using (IEnumerator<T> enumerator = source.GetEnumerator())
			{
				return enumerator.MoveNext() ? enumerator.Current : defaultValue;
			}
		}

		public static IEnumerable<T> TakeAtMost<T>(this IEnumerable<T> source, int count)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			if (count < 2)
			{
				throw new ArgumentOutOfRangeException("count");
			}

			var list = source.ToList();

			var actualCount = list.Count;

			if (actualCount == 0)
			{
				yield break;
			}

			if (actualCount <= count)
			{
				foreach (var item in list)
					yield return item;
			}
			else
			{
				var frequency = (actualCount - 1)/(double) (count - 1);

				var sourceWithNumbers = list.Select((x, i) => new {x, i});

				double currentNumber = 0;

				foreach (var sourceWithNumber in sourceWithNumbers)
				{
					if ((int) Math.Round(currentNumber) == sourceWithNumber.i)
					{
						yield return sourceWithNumber.x;
						currentNumber += frequency;
					}
				}
			}
		}


		public static HashSet<T> ToHashSet<T>(this IEnumerable<T> items)
		{
			return new HashSet<T>(items);
		}

		public static IEnumerable<T> Concat<T>(this IEnumerable<T> items, params T[] additional)
		{
			return Enumerable.Concat(items, additional);
		}

		public static bool Empty<T>(this IEnumerable<T> enumerable)
		{
			return !enumerable.Any();
		}

		public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
		{
			return enumerable == null || !enumerable.Any();
		}

		public static IEnumerable<TItem> CollectDuplicates<TItem, TKey>(this IEnumerable<TItem> items, Func<TItem, TKey> itemKeyProvider)
		{
			var map = new Dictionary<TKey, List<TItem>>();
			foreach (TItem item in items)
			{
				TKey key = itemKeyProvider(item);
				Maybe<List<TItem>> maybeList = map.GetValue(key);
				if (!maybeList.HasValue)
				{
					map.Add(key, new List<TItem> { item });
				}
				else
				{
					maybeList.Value.Add(item);
				}
			}
			return map.Where(i => i.Value.Count > 1).SelectMany(i => i.Value);
		}

			
		public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
		{
			if (source != null)
			{
				foreach (var elem in source)
				{
					action(elem);
				}
			}
		}

		public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> source, int chunkSize)
		{
			return source.Where((x, i) => i % chunkSize == 0).Select((x, i) => source.Skip(i * chunkSize).Take(chunkSize));
		}

		public static IEnumerable<Tuple<string,object>> GetObjectValues(this object values)
		{
			if (values == null) 
				yield break;
			foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(values))
			{
				object obj2 = descriptor.GetValue(values);
				yield return Tuple.Create(descriptor.Name, obj2);
			}
		}
	}
}
