using System;

namespace Tp.Core
{
	public class ScopedAction : IDisposable
	{
		private readonly Action _action;

		private ScopedAction(Action action)
		{
			_action = action;
		}

		public void Dispose()
		{
			_action();
		}

		public static IDisposable New(Action onEnter, Action onExit)
		{
			onEnter();
			return new ScopedAction(onExit);
		}

		public static IDisposable Defer(Action onExit)
		{
			return new ScopedAction(onExit);
		}
	}
}