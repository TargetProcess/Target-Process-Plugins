using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Exceptions;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Extensions;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Interfaces;

namespace Tp.Integration.Messages.ServiceBus.Transport.Router.Pump
{
	public class MessageConsumer<TMessage> : IMessageConsumer<TMessage>
	{
		private readonly IMessageSource<TMessage> _messageSource;
		private readonly IScheduler _scheduler;
		private IObservable<TMessage> _observable;
		private IDisposable _subscription;
		private volatile bool _isRunning;
		private volatile bool _disposed;
		private Predicate<TMessage> _while;
		private readonly List<IObserver<TMessage>> _observers;

		public MessageConsumer(IMessageSource<TMessage> messageSource, IScheduler scheduler)
		{
			_messageSource = messageSource;
			_scheduler = scheduler;
			_observers = new List<IObserver<TMessage>>();
		}

		public virtual string Name
		{
			get { return _messageSource.Name + "~consumer"; }
		}

		public void AddObserver(IObserver<TMessage> observer)
		{
			ThrowIfRunning();
			_observers.Add(observer);
		}

		public void Consume(Action<TMessage> handleMessage)
		{
			ThrowIfRunning();
			ConsumeCore(handleMessage);
			_isRunning = true;
		}

		protected virtual void ConsumeCore(Action<TMessage> handleMessage)
		{
			_observable = _messageSource.Until(ResolveWhile(), handleMessage, Finally, _scheduler);
			_subscription = _observable.Subscribe(new CompositeObserver(_observers));
		}

		public void Dispose()
		{
			_disposed = true;
			if (_subscription != null)
			{
				_subscription.Dispose();
			}
			OnDispose();
			_isRunning = false;
		}

		public Predicate<TMessage> While
		{
			get { return _while; }
			set
			{
				ThrowIfRunning();
				_while = value;
			}
		}

		public bool IsRunning
		{
			get { return _isRunning; }
		}

		protected IMessageSource<TMessage> MessageSource
		{
			get { return _messageSource; }
		}

		protected void ThrowIfRunning()
		{
			if (IsRunning)
			{
				throw new MessageConsumerException(string.Format("Cannot change already running MessageConsumer"));
			}
		}

		private void Finally()
		{
			if(_disposed)
			{
				OnDispose();
			}
		}

		private Predicate<TMessage> ResolveWhile()
		{
			return While != null ? (Predicate<TMessage>)CustomWhile : IsNotDisposed;
		}

		private bool IsNotDisposed(TMessage _)
		{
			return !_disposed;
		}

		private bool CustomWhile(TMessage m)
		{
			return While(m) && IsNotDisposed(m);
		}

		protected virtual void OnDispose()
		{
		}
		
		private class CompositeObserver : IObserver<TMessage>
		{
			private readonly List<IObserver<TMessage>> _observers;

			public CompositeObserver(List<IObserver<TMessage>> observers)
			{
				_observers = observers;
			}

			public void OnNext(TMessage value)
			{
				_observers.ForEach(o => o.OnNext(value));
			}

			public void OnError(Exception error)
			{
				_observers.ForEach(o => o.OnError(error));
			}

			public void OnCompleted()
			{
				_observers.ForEach(o => o.OnCompleted());
			}
		}
	}
}