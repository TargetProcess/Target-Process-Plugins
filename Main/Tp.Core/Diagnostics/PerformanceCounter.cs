using System;
using log4net;

namespace Tp.Core.Diagnostics
{
	internal class PerformanceCounter : IPerformanceCounter
	{
		private readonly System.Diagnostics.PerformanceCounter _source;
		private readonly ILog _log;

		public PerformanceCounter(System.Diagnostics.PerformanceCounter source, ILog log)
		{
			_source = source;
			_log = log;
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
			catch (Exception ex)
			{
				_log.Error("Error occured", ex);
				return default(T);
			}
		}
	}
}
