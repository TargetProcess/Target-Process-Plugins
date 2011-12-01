// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Interfaces;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Log;

namespace Tp.Integration.Messages.ServiceBus.Transport.Router.Pump
{
	public abstract class MessageRouter<TMessage> : MessageConsumer<TMessage>
	{
		private readonly ILoggerContextSensitive _log;
		private readonly Func<TMessage, string> _tagMessageProvider;
		private readonly IProducerConsumerFactory<TMessage> _producerConsumerFactory;
		private readonly Dictionary<string, Child> _routerItems;
		private Action<TMessage> _handleMessage;
		private int _childrenCount;

		protected MessageRouter(IMessageSource<TMessage> messageSource, IProducerConsumerFactory<TMessage> producerConsumerFactory, Func<TMessage, string> tagMessageProvider, IScheduler scheduler, ILoggerContextSensitive log)
			: base(messageSource, scheduler)
		{
			_log = log;
			_producerConsumerFactory = producerConsumerFactory;
			_tagMessageProvider = tagMessageProvider;
			_routerItems = new Dictionary<string, Child>();
		}

		public override string Name
		{
			get { return MessageSource.Name + "~router"; }
		}
		
		protected override void ConsumeCore(Action<TMessage> handleMessage)
		{
			_handleMessage = handleMessage;
			base.ConsumeCore(Receive);
		}

		protected abstract void Preprocess(TMessage message);

		protected override void OnDispose()
		{
			base.OnDispose();
			foreach (IMessageConsumer<TMessage> consumer in _routerItems.Values.Select(r => r.Consumer))
			{
				consumer.Dispose();
			}
		}

		private void Receive(TMessage message)
		{
			_log.Debug(LoggerContext.New(Name), "thread pool info.");
			string messageTag = _tagMessageProvider(message);
			if (string.IsNullOrEmpty(messageTag))
			{
				_handleMessage(message);
				return;
			}
			Preprocess(message);
			Child child = GetOrCreateRouterItem(messageTag);
			child.Producer.Produce(message);
		}

		private Child GetOrCreateRouterItem(string tag)
		{
			Child child;
			if (!_routerItems.TryGetValue(tag, out child))
			{
				child = Create(tag);
				_routerItems.Add(tag, child);
			}
			return child;
		}

		private Child Create(string sourceName)
		{
			IMessageSource<TMessage> source = _producerConsumerFactory.CreateSource(sourceName);
			return Create(source);
		}

		private Child Create(IMessageSource<TMessage> source)
		{
			Interlocked.Increment(ref _childrenCount);
			IMessageConsumer<TMessage> consumer = _producerConsumerFactory.CreateConsumer(source);
			IMessageProducer<TMessage> producer = _producerConsumerFactory.CreateProducer(source);
			consumer.AddObserver(new DisposeProducerOnCompleteObserver(producer, _log));
			consumer.Consume(_handleMessage);
			return new Child
			{
				Source = source,
				Consumer = consumer,
				Producer = producer
			};
		}

		struct Child
		{
			public IMessageProducer<TMessage> Producer { get; set; }
			public IMessageSource<TMessage> Source { get; set; }
			public IMessageConsumer<TMessage> Consumer { get; set; }
		}

		class DisposeProducerOnCompleteObserver : IObserver<TMessage>
		{
			private readonly IMessageProducer<TMessage> _producer;
			private readonly ILoggerContextSensitive _log;

			public DisposeProducerOnCompleteObserver(IMessageProducer<TMessage> producer, ILoggerContextSensitive log)
			{
				_producer = producer;
				_log = log;
			}

			public void OnNext(TMessage value)
			{
			}

			public void OnError(Exception error)
			{
			}

			public void OnCompleted()
			{
				string name = _producer.Name;
				_producer.Dispose();
				_log.Debug(LoggerContext.New(name), "Killed.");
			}
		}
	}
}