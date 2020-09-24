using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StructureMap;
using Tp.Core.Annotations;
using Tp.Core.Diagnostics.Event;

namespace Tp.Core.Diagnostics.Time
{
    /// <summary>
    /// Profiler instance should be used per thread
    /// The only cross thread interaction occurs during merging children threads profilers into parent through usage of AddData method
    /// P.S. TpProfiler usage covers only structural parallelism !
    /// </summary>
    public class Profiler : IProfilerDataCollector, IDisposable
    {
        private readonly ConcurrentStack<TimeInterval> _data;

        public Profiler(string name)
        {
            Name = name;
            _data = new ConcurrentStack<TimeInterval>();
        }

        public string Name { get; }

        // ReSharper disable once ReturnTypeCanBeEnumerable.Global
        public IReadOnlyList<TimeInterval> GetDataCopy()
        {
            return _data.ToArray();
        }

        public IEnumerable<TimeInterval> GetDataRawEnumerable()
        {
            return _data;
        }

        public void AddData(IEnumerable<TimeInterval> data)
        {
            var timeIntervals = data.ToArray();
            if (timeIntervals.Empty())
            {
                return;
            }
            _data.PushRange(timeIntervals);
        }

        public void Clear()
        {
            _data.Clear();
        }

        [UsedImplicitly]
        public void Dispose()
        {
            Clear();
        }

        public static Profiler GetProfiler()
        {
            return ObjectFactory.GetInstance<Profiler>();
        }

        public static (T, TimeSpan) Measure<T>([InstantHandle] [NotNull] Func<T> action)
        {
            var sw = new Stopwatch();
            sw.Start();
            var result = action();
            sw.Stop();

            return (result, sw.Elapsed);
        }

        public static TimeSpan Measure([NotNull] Action action)
        {
            var sw = new Stopwatch();
            sw.Start();
            action();
            sw.Stop();
            return sw.Elapsed;
        }

        public static async Task<(T, TimeSpan)> MeasureAsync<T>([NotNull] [InstantHandle] Func<Task<T>> action)
        {
            var sw = Stopwatch.StartNew();
            var result = await action();
            sw.Stop();
            return (result, sw.Elapsed);
        }

        public static async Task<TimeSpan> MeasureAsync([NotNull] [InstantHandle] Func<Task> action)
        {
            var sw = Stopwatch.StartNew();
            await action();
            sw.Stop();
            return sw.Elapsed;
        }

        public static IDisposable TimeIt(Action<TimeSpan> handleElapsed)
        {
            var w = Stopwatch.StartNew();
            return Disposable.Create(() =>
            {
                w.Stop();
                handleElapsed(w.Elapsed);
            });
        }
    }

    public static class ProfilerExtensions
    {
        public static void AddData([NotNull] this IProfilerDataCollector profiler, TimeInterval interval)
        {
            profiler.AddData(interval.Yield());
        }

        public static IDisposable Track(
            this IProfilerDataCollector profiler,
            ProfilerTarget target,
            object context = null)
        {
            var diagnosticConfiguration = ObjectFactory.GetInstance<DiagnosticConfiguration>();
            var stackTrace = diagnosticConfiguration.ShouldIncludeStackTraces ? Maybe.Return(new StackTrace().ToString()) : Maybe.Nothing;
            var resolvedContext = diagnosticConfiguration.ShouldIncludeTraceContexts ? context.NothingIfNull() : Maybe<object>.Nothing;
            return Profiler.TimeIt(elapsed => profiler.AddData(
                new TimeInterval(target, elapsed, Thread.CurrentThread.ManagedThreadId, resolvedContext, stackTrace)));
        }

        public static T Track<T>(this IProfilerDataCollector profiler, ProfilerTarget target, Func<T> f)
        {
            using (profiler.Track(target))
            {
                return f();
            }
        }

        public static void Track(this IProfilerDataCollector profiler, ProfilerTarget target, Action a)
        {
            Track(profiler, target, () =>
            {
                a();
                return Unit.Default;
            });
        }

        public static T TrackSingleThreadedOnly<T>(
            this Profiler profiler,
            Func<T> f, int threadId,
            Action<Profiler, IEnumerable<TimeInterval>> onMultipleThreadsAccess)
        {
            var result = f();
            var otherThreadsData = profiler.GetDataCopy().Where(x => x.ThreadId != threadId).ToArray();
            if (otherThreadsData.Any())
            {
                onMultipleThreadsAccess(profiler, otherThreadsData);
            }
            return result;
        }
    }

    public struct TimeInterval
    {
        public int ThreadId { get; }
        public TimeSpan Elapsed { get; }
        public ProfilerTarget Target { get; }
        /// <summary>
        /// Arbitrary target-specific payload for this tracked time interval.
        /// For example, SQL profile may include SQL command text.
        /// </summary>
        public Maybe<object> Context { get; }
        public Maybe<string> StackTrace { get; }

        public TimeInterval(
            ProfilerTarget target,
            TimeSpan elapsed,
            int threadId,
            Maybe<object> context,
            Maybe<string> stackTrace)
            : this()
        {
            Target = target;
            Elapsed = elapsed;
            ThreadId = threadId;
            Context = context;
            StackTrace = stackTrace;
        }
    }
}
