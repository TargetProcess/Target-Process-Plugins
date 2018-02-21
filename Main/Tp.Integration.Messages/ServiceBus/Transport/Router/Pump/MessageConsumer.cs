﻿using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Tp.Core;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Exceptions;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Interfaces;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Log;

namespace Tp.Integration.Messages.ServiceBus.Transport.Router.Pump
{
    public class MessageConsumer<TMessage> : IMessageConsumer<TMessage> where TMessage : class
    {
        private readonly IMessageSource<TMessage> _messageSource;
        private readonly IScheduler _scheduler;
        private readonly ILoggerContextSensitive _log;
        private IObservable<TMessage> _observable;
        private IDisposable _subscription;
        private volatile bool _isRunning;
        private Predicate<TMessage> _while;
        private readonly List<IObserver<TMessage>> _observers;

        public MessageConsumer(IMessageSource<TMessage> messageSource, IScheduler scheduler, ILoggerContextSensitive log)
        {
            _messageSource = messageSource;
            _scheduler = scheduler;
            _log = log;
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

        public virtual void Dispose(string childTag)
        {
        }

        protected virtual void ConsumeCore(Action<TMessage> handleMessage)
        {
            var @while = While ?? (_ => true);
            Func<IObservable<TMessage>> takeWhile =
                () =>
                    _messageSource.Iterate(handleMessage, _scheduler, s => _log.Info(LoggerContext.New(Name), s))
                        .TakeWhile(m => @while(m));
            _observable = takeWhile.ToSelfRepairingHotObservable(e => { });
            _subscription = _observable.Subscribe(new CompositeObserver(_observers));
        }

        public void Dispose()
        {
            if (_subscription != null)
            {
                _subscription.Dispose();
            }
            OnDispose();
            _isRunning = false;
            _log.Info(LoggerContext.New(Name), "disposed.");
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

        private void ThrowIfRunning()
        {
            if (IsRunning)
            {
                throw new MessageConsumerException(string.Format("Cannot change already running MessageConsumer"));
            }
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
