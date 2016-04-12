using System;

namespace Tp.Core.Diagnostics
{
	public interface IPerformanceCounter : IDisposable
	{
		void Increment();
		void IncrementBy(long value);
		void Decrement();
		long RawValue { get; set; }
	}
}
