// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Tp.Core;
using Tp.Core.Annotations;

namespace System.Linq
{
	public class GroupWithCount<TKey> : IEquatable<GroupWithCount<TKey>>
	{
		public bool Equals(GroupWithCount<TKey> other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return EqualityComparer<TKey>.Default.Equals(_key, other._key) && _count == other._count;
		}

		public static bool operator ==(GroupWithCount<TKey> left, GroupWithCount<TKey> right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(GroupWithCount<TKey> left, GroupWithCount<TKey> right)
		{
			return !Equals(left, right);
		}

		private readonly TKey _key;
		private readonly int _count;

		public TKey Key
		{
			get { return _key; }
		}

		public int Count
		{
			get { return _count; }
		}

		public GroupWithCount(TKey key, int count)
		{
			_key = key;
			_count = count;
		}

		public override string ToString()
		{
			var builder = new StringBuilder();
			builder.Append("{ Key = ");
			builder.Append(Key);
			builder.Append(", Count = ");
			builder.Append(Count);
			builder.Append(" }");
			return builder.ToString();
		}

		public override bool Equals(object value)
		{
			var type = value as GroupWithCount<TKey>;
			return (type != null) && EqualityComparer<TKey>.Default.Equals(type.Key, Key) && EqualityComparer<int>.Default.Equals(type.Count, Count);
		}

		public override int GetHashCode()
		{
			int num = 0x7a2f0b42;
			num = (-1521134295*num) + EqualityComparer<TKey>.Default.GetHashCode(Key);
			return (-1521134295*num) + EqualityComparer<int>.Default.GetHashCode(Count);
		}
	}

	public static class EnumerableExtensions
	{
		// for generic interface IEnumerable<T>
		public static string ToString<T>(this IEnumerable<T> source,[NotNull] Func<T, string> selector, string separator)
		{
			if (source == null)
				return String.Empty;

			if (String.IsNullOrEmpty(separator))
				throw new ArgumentException("Parameter separator can not be null or empty.");

			return String.Join(separator,source.Where(x => !Equals(x, null)).Select(selector));
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

		
		public static string ToSqlString<T>(this IEnumerable<T> values)
		{
			if (values == null || !values.Any())
				return " ( null )";

			return String.Format(" ({0}) ", String.Join(",", values.Select(ToSimpleSqlString)));
		}

		public static string ToSimpleSqlString<T>(this T x)
		{
			if (x is string)
			{
				return string.Format("'{0}'", x);
			}
			if (x is bool)
			{
				return x.Equals(true) ? "1" : "0";
			}
			return x.ToString();
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
			if (source == null)
				return;
			foreach (var elem in source)
			{
				action(elem);
			}
		}


		public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> source, int chunkSize)
		{
			return source.Where((x, i) => i % chunkSize == 0).Select((x, i) => source.Skip(i * chunkSize).Take(chunkSize));
		}

		public static IEnumerable<T> SafeConcat<T>([CanBeNull] this IEnumerable<T> first, [CanBeNull] IEnumerable<T> second  )
		{
			if (first == null)
				first = Enumerable.Empty<T>();
			if (second == null)
				second = Enumerable.Empty<T>();

			return first.Concat(second);
		}

		/// <summary>
		/// Return first element, if a <param name="source"></param> contains one element, otherwise - default(T)
		/// </summary>
		public static T SingleOrDefaultRelax<T>(this IEnumerable<T> source, Func<T, bool> predicate = null)
		{
			if (predicate == null)
				predicate = x => true;

			using (var enumerator = source.GetEnumerator())
			{
				if (!enumerator.MoveNext())
					return default(T);

				T value = enumerator.Current;

				return !enumerator.MoveNext() && predicate(value) ? value : default(T);
			}
		}


		public static void Times(this int count, Action<int> @do)
		{
			for (int i = 0; i < count; i++)
			{
				@do(i);
			}
		}

		public static IEnumerable<int> Times(this int count)
		{
			for (int i = 0; i < count; i++)
			{
				yield return i;
			}
		}

		public static void Times(this int count, Action @do)
		{
			count.Times(_ => @do());
		}

		public static IEnumerable<int> To(this int from, int uninclusiveTo)
		{
			for (int i = from; i < uninclusiveTo; i++)
			{
				yield return i;
			}
		}
	
	}
}
