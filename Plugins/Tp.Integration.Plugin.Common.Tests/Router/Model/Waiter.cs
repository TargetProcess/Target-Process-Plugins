using System;
using System.Threading;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Interfaces;

namespace Tp.Integration.Plugin.Common.Tests.Router.Model
{
	class Waiter
	{
		private int _waitablesCount;
		private readonly ManualResetEvent _waitEvent;

		public Waiter(int waitablesCount)
		{
			_waitablesCount = waitablesCount;
			_waitEvent = new ManualResetEvent(false);
		}

		public void Register(IMessageConsumer<TestMessage> messageConsumer)
		{
			messageConsumer.AddObserver(new Observer<TestMessage>(() =>
			                                                      	{
			                                                      		if (Interlocked.Decrement(ref _waitablesCount) == 0)
			                                                      		{
			                                                      			_waitEvent.Set();
			                                                      		}
			                                                      	}));
		}

		public bool Wait(TimeSpan waitTimeout)
		{
			return _waitEvent.WaitOne(waitTimeout);
		}

		class Observer<T> : IObserver<T>
		{
			private readonly Action<T> _onNext;
			private readonly Action<Exception> _onError;
			private readonly Action _onComplete;

			public Observer(Action onComplete):this(m => {}, e => {}, onComplete)
			{
			}

			public Observer(Action<T> onNext, Action<Exception> onError, Action onComplete)
			{
				_onNext = onNext;
				_onError = onError;
				_onComplete = onComplete;
			}

			public void OnNext(T value)
			{
				_onNext(value);
			}

			public void OnError(Exception error)
			{
				_onError(error);
			}

			public void OnCompleted()
			{
				_onComplete();
			}
		}
	}
}