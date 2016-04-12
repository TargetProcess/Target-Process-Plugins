using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Tp.Core.Diagnostics.Time
{
	public class TpProfilerStatistics
	{
		private readonly Dictionary<ProfilerTarget, TpProfilerStatisticRecord> _data;

		private TpProfilerStatistics(Dictionary<ProfilerTarget, string> data)
		{
			_data = data.ToDictionary(x => x.Key, x => new TpProfilerStatisticRecord
			{
				Target = x.Key,
				Name = x.Key.GetMetadata<ProfilerTarget, HttpHeaderAttribute>(),
				Data = x.Value
			});
		}

		public Maybe<TpProfilerStatisticRecord> this[ProfilerTarget target] => _data.GetValue(target);

		public IEnumerable<TpProfilerStatisticRecord> All => _data.Values;

		public static TpProfilerStatistics Build(IEnumerable<TimeInterval> timeIntervals)
		{
			var data = Enum.GetValues(typeof(ProfilerTarget)).Cast<ProfilerTarget>().ToDictionary(x => x, x => BuildCore(timeIntervals, x));
			return new TpProfilerStatistics(data);
		}

		private static string BuildCore(IEnumerable<TimeInterval> timeIntervals, ProfilerTarget target)
		{
			return timeIntervals.Where(x => x.Target == target)
				.GroupBy(x => x.ThreadId)
				.OrderBy(x => x.Key == Thread.CurrentThread.ManagedThreadId ? 0 : 1)
				.Select(x => x.Sum(t => (int) t.Elapsed.TotalMilliseconds))
				.ToList()
				.ToString(";");
		}
	}
}
