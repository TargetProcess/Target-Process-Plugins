using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using StructureMap;
using Tp.Core.Annotations;

namespace Tp.Core.Diagnostics.Time
{
	/// <summary>
	/// Profiler instance should be used per thread
	/// The only cross thread interaction occurs during merging children threads profilers into parent through usage of AddData method
	/// P.S. TpProfiler usage covers only structural parallelism !
	/// </summary>
	public class Profiler : ITpProfilerDataCollector, IDisposable
	{
		private readonly ConcurrentStack<TimeInterval> _data;

		public Profiler()
		{
			_data = new ConcurrentStack<TimeInterval>();
		}

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
			_data.PushRange(data.ToArray());
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
		public static IDisposable Track(this Profiler profiler, ProfilerTarget target)
		{
			return Chrono.TimeIt(elapsed => profiler.AddData(new TimeInterval(target, elapsed, Thread.CurrentThread.ManagedThreadId)));
		}

		public static T Track<T>(this Profiler profiler, ProfilerTarget target, Func<T> f)
		{
			T result = default(T);
			Chrono.TimeIt(() => result = f(), elapsed => profiler.AddData(new TimeInterval(target, elapsed, Thread.CurrentThread.ManagedThreadId)));
			return result;
		}

		public static void Track(this Profiler profiler, ProfilerTarget target, Action a)
		{
			Track<object>(profiler, target, () =>
			{
				a();
				return null;
			});
		}
	}

	public struct TimeInterval
	{
		public int ThreadId { get; private set; }
		public TimeSpan Elapsed { get; private set; }
		public ProfilerTarget Target { get; private set; }

		public TimeInterval(ProfilerTarget target, TimeSpan elapsed, int threadId)
			: this()
		{
			Target = target;
			Elapsed = elapsed;
			ThreadId = threadId;
		}
	}
}
