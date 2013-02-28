using System;

namespace Tp.Core
{
	public class Disposable : IDisposable
	{
		private readonly Action _action;

		protected Disposable(Action action)
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

		public static IDisposable Empty
		{
			get { return DefaultDisposable.Instance; }
		}

		private class DefaultDisposable :IDisposable
		{
			public static readonly DefaultDisposable Instance = new DefaultDisposable();

			static DefaultDisposable()
			{
			}

			private DefaultDisposable()
			{
			}

			public void Dispose()
			{
			}
		}
	}
}