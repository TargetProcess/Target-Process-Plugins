using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Tp.Core.Diagnostics.Time
{
	public class ProfilerStatistics
	{
		private readonly Dictionary<ProfilerTarget, TpProfilerStatisticRecord> _data;

		private ProfilerStatistics(Dictionary<ProfilerTarget, string> data)
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

		public static ProfilerStatistics Build(IEnumerable<TimeInterval> timeIntervals)
		{
			var data = Enum.GetValues(typeof(ProfilerTarget)).Cast<ProfilerTarget>().ToDictionary(x => x, x => Calculate(timeIntervals, x).Select(x1 => x1.ElapsedMs).ToString(";"));
			return new ProfilerStatistics(data);
		}

		public static IReadOnlyCollection<ProfilerTargetData> Calculate(IEnumerable<TimeInterval> timeIntervals, params ProfilerTarget[] targets)
		{
			return timeIntervals.Where(x => targets.Contains(x.Target))
				.GroupBy(x => x.ThreadId)
				.OrderBy(x => x.Key == Thread.CurrentThread.ManagedThreadId ? 0 : 1)
				.Select(x => new ProfilerTargetData { ThreadId = x.Key, ElapsedMs = x.Sum(t => (int)t.Elapsed.TotalMilliseconds)}).ToList();
		}

		public struct ProfilerTargetData
		{
			public int ThreadId { get; set; }
			public double ElapsedMs { get; set; }
		}
	}
}
