// 
// Copyright (c) 2005-2014 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using StructureMap;

namespace Tp.Core.Diagnostics.Time
{
	public class TpProfilerProvider
	{
		private TpProfilerProvider() { }

		public static readonly TpProfilerProvider Instance = new TpProfilerProvider();
		public TpProfiler GetProfiler()
		{
			return ObjectFactory.GetInstance<TpProfiler>();
		}
	}
	/// <summary>
	/// TpProfiler instance should be used per thread
	/// The only cross thread interaction occurs during merging children threads profilers into parent through usage of AddData method
	/// P.S. TpProfiler usage covers only structural parallelism !
	/// </summary>
	public class TpProfiler : ITpProfilerDataCollector
	{
		private readonly ConcurrentStack<TimeInterval> _data;
		public TpProfiler()
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
