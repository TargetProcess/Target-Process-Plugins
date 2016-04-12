using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using StructureMap;
using Tp.Core.Annotations;

namespace Tp.Core.Diagnostics.Time
{
	public class ProfilerProvider
	{
		private ProfilerProvider()
		{
		}

		public static readonly ProfilerProvider Instance = new ProfilerProvider();

		public Profiler GetProfiler()
		{
			return ObjectFactory.GetInstance<Profiler>();
		}
	}

	/// <summary>
	/// TpProfiler instance should be used per thread
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

		public void AddData(IEnumerable<TimeInterval> data)
		{
			_data.PushRange(data.ToArray());
		}

		public IDisposable Start(ProfilerTarget target)
		{
			var stopWatch = Stopwatch.StartNew();
			return Disposable.Create(() => _data.Push(new TimeInterval(target, stopWatch.Elapsed, Thread.CurrentThread.ManagedThreadId)));
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

		public static T Run<T>(ProfilerTarget target, Func<T> f)
		{
			using (ProfilerProvider.Instance.GetProfiler().Start(target))
				return f();
		}

		public static void Run(ProfilerTarget target, Action a)
		{
			Run<object>(target, () =>
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
