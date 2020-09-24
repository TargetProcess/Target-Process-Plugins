using System;
using System.Threading;

namespace Tp.Core.Diagnostics.Time
{
    public struct TimePoint : IComparable<TimePoint>
    {
        private static long _globalOrder;

        private TimePoint(string name, DateTimeOffset timestamp) : this(name, timestamp, Interlocked.Increment(ref _globalOrder))
        {
        }

        public TimePoint(string name, DateTimeOffset timestamp, long order) : this()
        {
            Order = order;
            Name = name;
            Timestamp = timestamp;
        }

        public string Name { get; }

        public DateTimeOffset Timestamp { get; }

        public long Order { get; }

        public int CompareTo(TimePoint other)
        {
            var result = Timestamp.CompareTo(other.Timestamp);

            return result == 0 ? Order.CompareTo(other.Order) : result;
        }

        public override string ToString()
        {
            return "TimePoint: {0}, {1}, {2}".Fmt(Name, Timestamp, Order);
        }

        /// <summary>
        /// How much time passed since <see cref="Timestamp"/> to <see cref="point"/>.
        /// </summary>
        public TimeSpan DiffTo(DateTimeOffset point) => point - Timestamp;

        public static TimePoint UtcNow(string name)
        {
            return new TimePoint(name, CurrentDate.UtcValue);
        }

        public static TimePoint Create(string name, DateTimeOffset point)
        {
            return new TimePoint(name, point);
        }
    }
}
