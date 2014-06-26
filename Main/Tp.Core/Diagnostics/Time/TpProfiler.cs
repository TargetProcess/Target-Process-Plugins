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

namespace Tp.Core.Diagnostics.Time
{
	public class TpProfiler
	{
		private readonly ConcurrentBag<TimeInterval> _timeIntervals;
		private readonly int _threadId;

		public TpProfiler()
		{
			_threadId = Thread.CurrentThread.ManagedThreadId;
			_timeIntervals = new ConcurrentBag<TimeInterval>();
		}

		public IEnumerable<TimeInterval> GetTimeIntervals()
		{
			return _timeIntervals.ToArray();
		}

		public TimeIntervalTracker Start(string key)
		{
			return new TimeIntervalTracker(timeElapsed => _timeIntervals.Add(new TimeInterval(key, timeElapsed, _threadId)));
		}

		public void Merge(TpProfiler other)
		{
			other._timeIntervals.ForEach(timeInterval => _timeIntervals.Add(timeInterval));
		}
	}

	public class TimeIntervalTracker : IDisposable
	{
		private readonly Action<TimeSpan> _onStop;
		private readonly Stopwatch _stopWatch;

		public TimeIntervalTracker(Action<TimeSpan> onStop)
		{
			_onStop = onStop;
			_stopWatch = new Stopwatch();
			_stopWatch.Start();
		}

		public void Dispose()
		{
			_onStop(_stopWatch.Elapsed);
		}
	}

	public struct TimeInterval
	{
		public TimeSpan Elapsed { get; private set; }
		public int ThreadId { get; private set; }
		public string Key { get; private set; }

		public TimeInterval(string key, TimeSpan elapsed, int threadId)
			: this()
		{
			Key = key;
			Elapsed = elapsed;
			ThreadId = threadId;
		}
	}
}
