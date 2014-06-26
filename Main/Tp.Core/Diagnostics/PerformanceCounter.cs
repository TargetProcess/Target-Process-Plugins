// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;

namespace Tp.Core.Diagnostics
{
	internal class PerformanceCounter : IPerformanceCounter
	{
		private readonly System.Diagnostics.PerformanceCounter _source;

		public PerformanceCounter(System.Diagnostics.PerformanceCounter source)
		{
			_source = source;
		}

		public void Increment()
		{
			Try(c => c.Increment());
		}

		public void IncrementBy(long value)
		{
			Try(c => c.IncrementBy(value));
		}

		public void Decrement()
		{
			Try(c => c.Decrement());
		}

		public long RawValue
		{
			get { return TryGet(c => c.RawValue); }
			set { Try(c => c.RawValue = value); }
		}

		public void Dispose()
		{
			if (_source != null)
			{
				_source.Dispose();
			}
		}

		private void Try(Action<System.Diagnostics.PerformanceCounter> action)
		{
			TryGet(c =>
				{
					action(c);
					return true;
				});
		}

		private T TryGet<T>(Func<System.Diagnostics.PerformanceCounter, T> func)
		{
			try
			{
				return func(_source);
			}
			catch
			{
				return default(T);
			}
		}
	}
}