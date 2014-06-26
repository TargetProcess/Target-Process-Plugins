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

		public string Name { get; private set; }

		public DateTimeOffset Timestamp { get; private set; }

		public long Order { get; private set; }

		public int CompareTo(TimePoint other)
		{
			var result = Timestamp.CompareTo(other.Timestamp);

			return result == 0 ? Order.CompareTo(other.Order) : result;
		}

		public override string ToString()
		{
			return "TimePoint: {0}, {1}, {2}".Fmt(Name, Timestamp, Order);
		}

		public static TimePoint UtcNow(string name)
		{
			return new TimePoint(name, DateTimeOffset.UtcNow);
		}

		public static TimePoint Empty
		{
			get { return UtcNow("Empty"); }
		}
	}
}