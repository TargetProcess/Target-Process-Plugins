using System;

namespace Tp.Core
{
	public class EmptyObserver<T> : IObserver<T>
	{
		public void OnNext(T value)
		{
		}

		public void OnError(Exception error)
		{
		}

		public void OnCompleted()
		{
		}
	}
}
