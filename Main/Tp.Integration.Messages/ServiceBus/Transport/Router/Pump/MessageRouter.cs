using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Threading;
using NServiceBus.Utils;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Interfaces;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Log;

namespace Tp.Integration.Messages.ServiceBus.Transport.Router.Pump
{
	public abstract class MessageRouter<TMessage> : MessageConsumer<TMessage> where TMessage : class
	{
		private readonly ILoggerContextSensitive _log;
		private readonly Func<TMessage, string> _tagMessageProvider;
		private readonly IProducerConsumerFactory<TMessage> _producerConsumerFactory;
		private readonly ConcurrentDictionary<string, Lazy<Child>> _routerItems;
		private Action<TMessage> _handleMessage;
		private int _childrenCount;

		protected MessageRouter(IMessageSource<TMessage> messageSource, IProducerConsumerFactory<TMessage> producerConsumerFactory,
			Func<TMessage, string> tagMessageProvider, IScheduler scheduler, ILoggerContextSensitive log)
			: base(messageSource, scheduler, log)
		{
			_log = log;
			_producerConsumerFactory = producerConsumerFactory;
			_tagMessageProvider = tagMessageProvider;
			_routerItems = new ConcurrentDictionary<string, Lazy<Child>>();
		}

		private void InitializeChildren(IEnumerable<string> childrenTags)
		{
			foreach (var childTag in childrenTags)
			{
				GetOrCreateRouterItem(childTag);
			}
		}

		public override string Name
		{
			get { return MessageSource.Name + "~router"; }
		}

		protected override void ConsumeCore(Action<TMessage> handleMessage)
		{
			_handleMessage = handleMessage;
			ThreadPool.QueueUserWorkItem(x => InitializeChildren(GetChildTags()));
			base.ConsumeCore(m =>
			{
				if (IsTransactional)
				{
					new TransactionWrapper().RunInTransaction(() => Process(m), IsolationLevel, TransactionTimeout);
				}
				else
				{
					Process(m);
				}
			});
		}

		protected abstract IEnumerable<string> GetChildTags();

		protected abstract void Receive(TMessage message);

		protected abstract void Preprocess(TMessage message);

		public override void Dispose(string childTag)
		{
			Lazy<Child> child;
			var hasValue = _routerItems.TryGetValue(childTag, out child);
			if (hasValue)
			{
				child.Value.Consumer.Dispose();
			}
		}

		protected override void OnDispose()
		{
			base.OnDispose();
			foreach (var consumer in _routerItems)
			{
				consumer.Value.Value.Consumer.Dispose();
			}
		}

		private void Process(TMessage message)
		{
			_log.Debug(LoggerContext.New(Name), "thread pool info.");
			string messageTag = _tagMessageProvider(message);
			if (string.IsNullOrEmpty(messageTag))
			{
				_handleMessage(message);
				return;
			}

			Receive(message);

			if (!NeedToHandle(message))
			{
				return;
			}

			Preprocess(message);
			Child child = GetOrCreateRouterItem(messageTag);
			child.Producer.Produce(message);
		}

		protected abstract bool NeedToHandle(TMessage messageTag);

		private Child GetOrCreateRouterItem(string tag)
		{
			return _routerItems.GetOrAdd(tag, t => Lazy.Create(() => Create(t))).Value;
		}

		private Child Create(string sourceName)
		{
			Interlocked.Increment(ref _childrenCount);
			IMessageSource<TMessage> source = _producerConsumerFactory.CreateSource(sourceName, true);
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

		private struct Child
		{
			public IMessageProducer<TMessage> Producer { get; set; }
			public IMessageSource<TMessage> Source { get; set; }
			public IMessageConsumer<TMessage> Consumer { get; set; }
		}

		private class DisposeProducerOnCompleteObserver : IObserver<TMessage>
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
