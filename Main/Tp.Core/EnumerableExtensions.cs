using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Tp.Core.Annotations;

// ReSharper disable once CheckNamespace

namespace System.Linq
{
    public static class EnumerableExtensions
    {
        [NotNull, Pure]
        public static string ToString<T>(
            [CanBeNull] this IEnumerable<T> source,
            [NotNull, InstantHandle] Func<T, string> selector,
            [CanBeNull] string separator)
        {
            if (source == null)
                return String.Empty;

            return String.Join(separator, source.Where(x => !Equals(x, null)).Select(selector));
        }

        [NotNull, Pure]
        public static string ToString<T>(
            [CanBeNull] this IEnumerable<T> source,
            [CanBeNull] string separator)
        {
            return source.ToString(x => x.ToString(), separator);
        }

        [NotNull, Pure]
        public static string ToString(
            [CanBeNull] this IEnumerable source,
            [CanBeNull] string separator)
        {
            if (source == null)
                return String.Empty;
            return source.Cast<object>().ToString(separator);
        }

        [NotNull, Pure]
        public static string ToSqlString<T>(
            [CanBeNull] this IEnumerable<T> source)
        {
            const string empty = " ( null )";
            if (source == null)
            {
                return empty;
            }

            var values = source as IReadOnlyCollection<T> ?? source.ToList();
            if (values.Count == 0)
            {
                return empty;
            }

            return String.Format(" ({0}) ", String.Join(",", values.Select(ToSimpleSqlString)));
        }

        [NotNull, Pure]
        public static string ToSimpleSqlString<T>(
            [NotNull] this T x)
        {
            if (x is string)
            {
                return $"'{x}'";
            }
            if (x is bool)
            {
                return x.Equals(true) ? "1" : "0";
            }
            return x.ToString();
        }

        [NotNull, Pure]
        public static string ToCommaDelimitedList(
            [NotNull] this IEnumerable<string> items)
        {
            return string.Join(", ", items.Select(x => string.Concat("'", x, "'")));
        }

        [CanBeNull]
        public static T FirstOrDefault<T>(
            [NotNull] this IEnumerable<T> source,
            [CanBeNull] T defaultValue)
        {
            using (IEnumerator<T> enumerator = source.GetEnumerator())
            {
                return enumerator.MoveNext() ? enumerator.Current : defaultValue;
            }
        }

        [NotNull, Pure, LinqTunnel]
        public static IEnumerable<T> TakeAtMost<T>(
            [NotNull] this IEnumerable<T> source, int count)
        {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (count < 2) { throw new ArgumentOutOfRangeException(nameof(count)); }

            var list = source.ToList();

            var actualCount = list.Count;

            if (actualCount == 0)
            {
                yield break;
            }

            if (actualCount <= count)
            {
                foreach (var item in list)
                {
                    yield return item;
                }
            }
            else
            {
                var frequency = (actualCount - 1) / (double) (count - 1);

                var sourceWithNumbers = list.Select((x, i) => new { x, i });

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

        [NotNull]
        [Pure]
        [LinqTunnel]
        public static IEnumerable<T> Prepend<T>(
            [NotNull] this IEnumerable<T> items,
            T value)
        {
            yield return value;

            foreach (var item in items)
            {
                yield return item;
            }
        }

        [NotNull, Pure, LinqTunnel]
        public static IEnumerable<T> Concat<T>(
            [NotNull] this IEnumerable<T> items,
            [NotNull] params T[] additional)
        {
            return Enumerable.Concat(items, additional);
        }

#if DEBUG
        [NotNull, Pure, LinqTunnel]
        public static IEnumerable<T> TapDebug<T>(
            [NotNull] this IEnumerable<T> items,
            [NotNull] Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
                yield return item;
            }
        }
#endif

        [Pure]
        public static bool Empty<T>([NotNull] this IEnumerable<T> enumerable)
        {
            return !enumerable.Any();
        }

        [Pure]
        public static bool Empty<T>([NotNull] this ICollection<T> collection)
        {
            return collection.Count == 0;
        }

        [Pure]
        [ContractAnnotation("null => true")]
        public static bool IsNullOrEmpty<T>([CanBeNull] this IEnumerable<T> enumerable)
        {
            return enumerable == null || !enumerable.Any();
        }

        [Pure, NotNull]
        public static IEnumerable<T> EmptyIfNull<T>([CanBeNull] this IEnumerable<T> enumerable)
        {
            return enumerable ?? Enumerable.Empty<T>();
        }

        public static void ForEach<T>(
            [CanBeNull] this IEnumerable<T> source,
            [NotNull, InstantHandle] Action<T> action)
        {
            source.ForEach((x, i) => action(x));
        }

        public static void ForEach<T>(
            [CanBeNull] this IEnumerable<T> source,
            [NotNull, InstantHandle] Action<T, int> action)
        {
            if (source == null)
                return;

            var index = 0;
            foreach (var elem in source)
            {
                action(elem, index++);
            }
        }

        [NotNull, Pure, LinqTunnel]
        public static IEnumerable<IEnumerable<T>> Split<T>(
            [NotNull] this IReadOnlyCollection<T> source, int partSize)
        {
            return source.Where((x, i) => i % partSize == 0).Select((x, i) => source.Skip(i * partSize).Take(partSize));
        }

        [NotNull, Pure, LinqTunnel]
        public static IEnumerable<IList<T>> SplitArray<T>(
            [NotNull] this T[] source, int partSize)
        {
            if (partSize == 0)
            {
                yield return source;
                yield break;
            }
            var arrayLength = source.Length;
            var fullPartsCount = arrayLength / partSize;
            for (int i = 0; i < fullPartsCount; ++i)
            {
                yield return new ArraySegment<T>(source, i * partSize, partSize);
            }
            var lastPartSize = arrayLength % partSize;
            if (lastPartSize != 0)
            {
                yield return new ArraySegment<T>(source, fullPartsCount * partSize, lastPartSize);
            }
        }

        [NotNull, Pure, LinqTunnel]
        public static IEnumerable<T> SafeConcat<T>([CanBeNull] this IEnumerable<T> first, [CanBeNull] IEnumerable<T> second)
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
        public static T SingleOrDefaultRelax<T>(
            [NotNull] this IEnumerable<T> source,
            [InstantHandle] Func<T, bool> predicate = null)
        {
            if (predicate == null)
                predicate = x => true;

            using (var enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                    return default;

                T value = enumerator.Current;

                return !enumerator.MoveNext() && predicate(value) ? value : default;
            }
        }

        public static void Times(
            this int count,
            [NotNull, InstantHandle] Action<int> @do)
        {
            for (int i = 0; i < count; i++)
            {
                @do(i);
            }
        }

        [NotNull, Pure, LinqTunnel]
        public static IEnumerable<int> Times(this int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return i;
            }
        }

        public static void Times(
            this int count,
            [NotNull, InstantHandle] Action @do)
        {
            count.Times(_ => @do());
        }

        [NotNull, Pure, LinqTunnel]
        public static IEnumerable<int> To(this int from, int uninclusiveTo)
        {
            for (int i = from; i < uninclusiveTo; i++)
            {
                yield return i;
            }
        }

        [NotNull, Pure, LinqTunnel]
        public static IEnumerable<TTo> Unfold<TFrom, TTo>(this TFrom seed,
            [NotNull] Func<TFrom, bool> canGenerate,
            [NotNull] Func<TFrom, TTo> generateNextValue,
            [NotNull] Func<TFrom, TFrom> generateNextState)
        {
            var state = seed;
            while (canGenerate(state))
            {
                yield return generateNextValue(state);
                state = generateNextState(state);
            }
        }

        [NotNull, Pure, LinqTunnel]
        public static IEnumerable<TTo> Unfold<TFrom, TTo>(this TFrom seed,
            [NotNull] Func<TFrom, TTo> generateNextValue,
            [NotNull] Func<TFrom, TFrom> generateNextState)
        {
            return Unfold(seed, x => true, generateNextValue, generateNextState);
        }

        [Pure]
        public static PartitionResult<T> Partition<T>(
            [NotNull] this IEnumerable<T> sequence,
            [NotNull, InstantHandle] Func<T, bool> predicate)
        {
            return sequence.Partition(predicate, (matching, notMatching) => new PartitionResult<T>(matching, notMatching));
        }

        [Pure]
        public static TResult Partition<T, TResult>(
            [NotNull] this IEnumerable<T> sequence,
            [NotNull, InstantHandle] Func<T, bool> predicate,
            [NotNull, InstantHandle] Func<IEnumerable<T>, IEnumerable<T>, TResult> resultSelector)
        {
            var groups = sequence.GroupBy(predicate).ToArray();
            var matches = groups.FirstOrDefault(x => x.Key);
            var doesNotMatch = groups.FirstOrDefault(x => !x.Key);

            return resultSelector(
                matches ?? Enumerable.Empty<T>(),
                doesNotMatch ?? Enumerable.Empty<T>());
        }

        [NotNull, ItemNotNull, Pure, LinqTunnel]
        public static IEnumerable<Tuple<T, T>> Pairwise<T>(
            [NotNull] this IEnumerable<T> xs)
        {
            using (var enumerator = xs.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    yield break;
                }

                var left = enumerator.Current;

                while (enumerator.MoveNext())
                {
                    var right = enumerator.Current;
                    yield return Tuple.Create(left, right);
                    left = right;
                }
            }
        }

        [NotNull, Pure]
        public static ReadOnlyCollection<T> ToReadOnlyCollection<T>(
            [NotNull] this IEnumerable<T> xs)
        {
            return new ReadOnlyCollection<T>(xs.ToList());
        }

        [NotNull, Pure]
        public static IEnumerable<T> Yield<T>(this T value)
        {
            yield return value;
        }

        public static int MaxOrDefault<TSource>(
            [NotNull] this IEnumerable<TSource> source,
            [NotNull, InstantHandle] Func<TSource, int> selector,
            int defaultValue = default)
        {
            return source.Select(selector).DefaultIfEmpty(defaultValue).Max();
        }

        /// <summary>
        /// Drops last n elements from the sequence. Returns empty sequence if source contains less than n elements.
        /// </summary>
        [NotNull, Pure, LinqTunnel]
        public static IEnumerable<T> DropLast<T>(
            [NotNull] this IEnumerable<T> xs, int n = 1)
        {
            if (n < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(n));
            }

            Queue<T> buffer = new Queue<T>(n + 1);
            foreach (T x in xs)
            {
                buffer.Enqueue(x);
                if (buffer.Count == n + 1)
                {
                    yield return buffer.Dequeue();
                }
            }
        }

        /// <summary>
        /// Batches the source sequence into sized buckets and applies a projection to each bucket.
        /// </summary>
        /// <typeparam name="T">Type of elements in <paramref name="source"/> sequence.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="size">Size of buckets.</param>
        /// <returns>Splits source enumerable on enumerables of provides size.</returns>
        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int size)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size));

            T[] bucket = null;
            var count = 0;

            foreach (var item in source)
            {
                if (bucket == null)
                {
                    bucket = new T[size];
                }

                bucket[count++] = item;

                if (count != size)
                {
                    continue;
                }

                yield return bucket;

                bucket = null;
                count = 0;
            }

            if (bucket != null && count > 0)
            {
                yield return bucket.Take(count);
            }
        }

        [Pure]
        public static bool Contains<T>(
            [NotNull] this IEnumerable<T> set,
            [NotNull] IEnumerable<T> subSet)
        {
            return subSet.All(set.Contains);
        }

        [Pure]
        public static bool Intersects<T>(
            [NotNull] this IEnumerable<T> left,
            [NotNull] IEnumerable<T> right)
        {
            return left.Any(right.Contains);
        }
    }

    public struct PartitionResult<T>
    {
        public readonly IEnumerable<T> Matching;
        public readonly IEnumerable<T> NotMatching;

        public PartitionResult(IEnumerable<T> matching, IEnumerable<T> notMatching)
        {
            Matching = matching;
            NotMatching = notMatching;
        }
    }

    public class GroupWithCount
    {
        [NotNull]
        public static GroupWithCount<TKey> New<TKey>(TKey key, int count) =>
            new GroupWithCount<TKey>(key, count);
    }

    public class GroupWithCount<TKey> : IEquatable<GroupWithCount<TKey>>
    {
        public bool Equals(GroupWithCount<TKey> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EqualityComparer<TKey>.Default.Equals(Key, other.Key) && Count == other.Count;
        }

        public static bool operator ==(GroupWithCount<TKey> left, GroupWithCount<TKey> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(GroupWithCount<TKey> left, GroupWithCount<TKey> right)
        {
            return !Equals(left, right);
        }

        public TKey Key { get; }

        public int Count { get; }

        public GroupWithCount(TKey key, int count)
        {
            Key = key;
            Count = count;
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
            return (type != null) && EqualityComparer<TKey>.Default.Equals(type.Key, Key)
                && EqualityComparer<int>.Default.Equals(type.Count, Count);
        }

        public override int GetHashCode()
        {
            int num = 0x7a2f0b42;
            num = (-1521134295 * num) + EqualityComparer<TKey>.Default.GetHashCode(Key);
            return (-1521134295 * num) + EqualityComparer<int>.Default.GetHashCode(Count);
        }
    }
}
