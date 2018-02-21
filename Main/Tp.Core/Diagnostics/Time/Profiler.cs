using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
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

        public IEnumerable<TimeInterval> GetData()
        {
            return _data.ToArray();
        }

        public void AddData(TimeInterval data)
        {
            _data.Push(data);
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
    }

    public static class ProfilerExtensions
    {
        public static IDisposable Track(this Profiler profiler, ProfilerTarget target, object context = null)
        {
            var diagnosticConfiguration = ObjectFactory.GetInstance<DiagnosticConfiguration>();
            var stackTrace = diagnosticConfiguration.ShouldIncludeStackTraces ? Maybe.Return(new StackTrace().ToString()) : Maybe.Nothing;
            var resolvedContext = diagnosticConfiguration.ShouldIncludeTraceContexts ? context.NothingIfNull() : Maybe<object>.Nothing;
            return
                Chrono.TimeIt(
                    elapsed =>
                        profiler.AddData(new TimeInterval(target, elapsed, Thread.CurrentThread.ManagedThreadId, resolvedContext,
                            stackTrace)));
        }

        public static T Track<T>(this Profiler profiler, ProfilerTarget target, Func<T> f)
        {
            using (profiler.Track(target))
            {
                return f();
            }
        }

        public static void Track(this Profiler profiler, ProfilerTarget target, Action a)
        {
            Track(profiler, target, () =>
            {
                a();
                return Unit.Default;
            });
        }

        public static T TrackSingleThreadedOnly<T>(this Profiler profiler, Func<T> f, int threadId,
            Action<Profiler, IEnumerable<TimeInterval>> onMultipleThreadsAccess)
        {
            var result = f();
            var otherThreadsData = profiler.GetData().Where(x => x.ThreadId != threadId).ToArray();
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
        public Maybe<object> Context { get; }
        public Maybe<string> StackTrace { get; }

        public TimeInterval(ProfilerTarget target, TimeSpan elapsed, int threadId, Maybe<object> context, Maybe<string> stackTrace)
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
