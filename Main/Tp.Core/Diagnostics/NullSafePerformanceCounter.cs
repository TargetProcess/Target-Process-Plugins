namespace Tp.Core.Diagnostics
{
	internal class NullSafePerformanceCounter : IPerformanceCounter
	{
		public void Increment()
		{
		}

		public void IncrementBy(long value)
		{
		}

		public void Decrement()
		{
		}

		public long RawValue { get; set; }

		public void Dispose()
		{
		}
	}
}
