using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Tp.Core.Annotations;

namespace Tp.Core.Diagnostics.Time
{
    public class ProfilerStatistics
    {
        public static readonly ProfilerStatistics Empty;

        private static readonly ProfilerTarget[] ProfilerTargets;

        private readonly Dictionary<ProfilerTarget, TpProfilerStatisticRecord> _data;

        static ProfilerStatistics()
        {
            ProfilerTargets = Enum.GetValues(typeof(ProfilerTarget)).Cast<ProfilerTarget>().ToArray();
            Empty = Build(Enumerable.Empty<TimeInterval>());
        }

        [PerformanceCritical]
        private ProfilerStatistics(Dictionary<ProfilerTarget, (string, int, int, string)> data)
        {
            _data = data
                .Select(x =>
                {
                    var target = x.Key;
                    var headerName = target.GetMetadata<ProfilerTarget, HttpHeaderAttribute>();
                    return new TpProfilerStatisticRecord
                    {
                        Target = target,
                        Name = ResolverHeaderName(headerName),
                        ElapsedTimeRaw = x.Value.Item1,
                        ElapsedTimeTotal = x.Value.Item2,
                        HitCount = x.Value.Item3,
                        Context = x.Value.Item4
                    };
                })
                .ToDictionaryOfKnownCapacity(data.Count, x => x.Target, x => x);
        }

        private string ResolverHeaderName(string headerName)
        {
#if DEBUG
            return headerName.StartsWith("X-") ? headerName : string.Empty;
#else
				return (headerName.StartsWith("X-") && !headerName.Contains("-Debug-")) ? headerName : string.Empty;
			#endif
        }

        public Maybe<TpProfilerStatisticRecord> this[ProfilerTarget target] => _data.GetValue(target);

        public IEnumerable<TpProfilerStatisticRecord> All => _data.Values;

        [PerformanceCritical]
        public static ProfilerStatistics Build(IEnumerable<TimeInterval> timeIntervals)
        {
            var data = ProfilerTargets.ToDictionaryOfKnownCapacity(x => x, x =>
            {
                var profilerDatas = Calculate(timeIntervals, x);
                var elapsedTimes = profilerDatas.Select(x1 => x1.ElapsedMs).ToArray();
                var hitCounts = profilerDatas.Select(x1 => x1.HitCount).ToArray();
                var contexts = profilerDatas.Select(x1 => x1.Context).ToArray();
                return (
                    elapsedTimes.ToString(";"),
                    elapsedTimes.Any() ? (int) elapsedTimes.Sum() : -1,
                    hitCounts.Any() ? hitCounts.Sum() : 0,
                    contexts.ToString(";"));
            });
            return new ProfilerStatistics(data);
        }

        public static IReadOnlyCollection<ProfilerTargetData> Calculate(
            IEnumerable<TimeInterval> timeIntervals,
            params ProfilerTarget[] targets)
        {
            return timeIntervals
                .Where(x => targets.Contains(x.Target))
                .GroupBy(x => x.ThreadId)
                .OrderBy(x => x.Key == Thread.CurrentThread.ManagedThreadId ? 0 : 1)
                .Select(x => new ProfilerTargetData
                {
                    ThreadId = x.Key,
                    ElapsedMs = x.Sum(t => (int) t.Elapsed.TotalMilliseconds),
                    HitCount = x.Count(),
                    Context = x
                        .Aggregate(
                            new StringBuilder(1024),
                            (acc, interval) => acc
                                .AppendLine($"[Thread:{x.Key}, ElapsedMs:{interval.Elapsed.TotalMilliseconds}]{{")
                                .AppendLine("Context:" + interval.Context.Select(c => c.ToString()).GetOrDefault(string.Empty))
                                .AppendLine("StackTrace:" + interval.StackTrace.Select(c => c.ToString()).GetOrDefault(string.Empty))
                                .AppendLine("}}"))
                        .ToString()
                })
                .ToList();
        }

        public struct ProfilerTargetData
        {
            public int ThreadId { get; set; }
            public double ElapsedMs { get; set; }
            public int HitCount { get; set; }
            public string Context { get; set; }
        }
    }
}
