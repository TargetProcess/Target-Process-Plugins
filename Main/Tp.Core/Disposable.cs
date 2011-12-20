using System;

namespace Tp.Core
{
	public class Disposable : IDisposable
	{
		private readonly Action _action;

		private Disposable(Action action)
		{
			_action = action;
		}

		public void Dispose()
		{
			_action();
		}
		
		public static IDisposable Create(Action onExit)
		{
			return new Disposable(onExit);
		}
	}
}