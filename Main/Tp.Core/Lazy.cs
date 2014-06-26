// ReSharper disable CheckNamespace

using System.Threading;

namespace System
// ReSharper restore CheckNamespace
{
	public static class Lazy
	{
		public static Lazy<T> Create<T>(Func<T> valueFactory, LazyThreadSafetyMode threadSafetyMode = LazyThreadSafetyMode.ExecutionAndPublication)
		{
			return new Lazy<T>(valueFactory, threadSafetyMode);
		}

		public static Lazy<T> Create<T>(T value, LazyThreadSafetyMode threadSafetyMode = LazyThreadSafetyMode.ExecutionAndPublication)
		{
			return new Lazy<T>(()=>value, threadSafetyMode);
		}
	}
}