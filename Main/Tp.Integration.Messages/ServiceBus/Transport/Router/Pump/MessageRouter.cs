using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Messaging;
using System.Reactive.Concurrency;
using System.Threading;
using System.Transactions;
using NServiceBus.Utils;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Interfaces;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Log;
using Tp.Integration.Messages.ServiceBus.Transport.Router.MsmqRx;

namespace Tp.Integration.Messages.ServiceBus.Transport.Router.Pump
{
    public abstract class MessageRouter<TMessage> : MessageConsumer<TMessage> where TMessage : class, IMessageWithId
    {
        private readonly ILoggerContextSensitive _log;
        private readonly TransportTransactionMode _transactionMode;
        private readonly IsolationLevel _isolationLevel;
        private readonly TimeSpan _transactionTimeout;
        private readonly MessageQueueTransactionType _sendTransactionType;
        private readonly MessageQueueTransactionType _receiveTransactionType;
        private readonly Func<TMessage, string> _tagMessageProvider;
        private readonly IProducerConsumerFactory<TMessage> _producerConsumerFactory;
        private readonly ConcurrentDictionary<string, Lazy<Child>> _routerItems;
        private Action<TMessage> _handleMessage;
        private int _childrenCount;
        private readonly RetryCountPerMessageKeeper _retryCountPerMessageKeeper = new RetryCountPerMessageKeeper();

        protected MessageRouter(IMessageSource<TMessage> messageSource, IProducerConsumerFactory<TMessage> producerConsumerFactory,
            Func<TMessage, string> tagMessageProvider, IScheduler scheduler, ILoggerContextSensitive log,
            TransportTransactionMode transactionMode, IsolationLevel isolationLevel, TimeSpan transactionTimeout,
            MessageQueueTransactionType sendTransactionType, MessageQueueTransactionType receiveTransactionType)
            : base(messageSource, scheduler, log)
        {
            _log = log;
            _transactionMode = transactionMode;
            _isolationLevel = isolationLevel;
            _transactionTimeout = transactionTimeout;
            _sendTransactionType = sendTransactionType;
            _receiveTransactionType = receiveTransactionType;
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

                switch (_transactionMode)
                {
                    case TransportTransactionMode.QueueOnly:
                        ProcessIfMessageTagIsProvided(m, ProcessInMessageQueueTransaction);
                        break;
                    case TransportTransactionMode.TransactionScope:
                        new TransactionWrapper().RunInTransaction(() => ProcessIfMessageTagIsProvided(m, Process), _isolationLevel, _transactionTimeout);
                        break;
                    default:
                        ProcessIfMessageTagIsProvided(m, Process);
                        break;
                }
            });
        }

        protected abstract IEnumerable<string> GetChildTags();

        protected abstract void Receive(TMessage message, MessageQueueTransaction messageQueueTransaction);
        protected abstract void Receive(TMessage message, MessageQueueTransactionType messageQueueTransactionType);

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

        private void ProcessIfMessageTagIsProvided(TMessage message, Action<TMessage, string> processAction)
        {
            _log.Debug(LoggerContext.New(Name), "thread pool info.");
            string messageTag = _tagMessageProvider(message);
            if (string.IsNullOrEmpty(messageTag))
            {
                _handleMessage(message);
                return;
            }

            processAction(message, messageTag);
        }

        private void Process(TMessage message, string messageTag)
        {
            RetryAwareHandle(message, m => Receive(m, _receiveTransactionType), messageTag,
                (producer, m) => producer.Produce(m, _sendTransactionType));
        }

        private void ProcessInMessageQueueTransaction(TMessage message, string messageTag)
        {
            TransportTransactionHelper.RunInQueueOnlyTransaction(mqTx =>
            {
                RetryAwareHandle(message, m => Receive(m, mqTx), messageTag,
                    (producer, m) => producer.Produce(m, mqTx));
            });
        }

        private bool IsTransactional
            => _transactionMode == TransportTransactionMode.QueueOnly || _transactionMode == TransportTransactionMode.TransactionScope;

        private void RetryAwareHandle(TMessage message, Action<TMessage> receive, string messageTag,
            Action<IMessageProducer<TMessage>, TMessage> produce)
        {
            try
            {
                receive(message);

                if (IsTransactional && _retryCountPerMessageKeeper.HandledMaxRetries(message.Id))
                {
                    _log.Error(LoggerContext.New(Name),
                        $"Message has failed the maximum number of {_retryCountPerMessageKeeper.MaxRetries} times allowed, ID: {message.Id}.");
                }
                else if (NeedToHandle(message))
                {
                    Preprocess(message);
                    var child = GetOrCreateRouterItem(messageTag);
                    produce(child.Producer, message);
                }
            }
            catch (Exception e)
            {
                _log.Error(LoggerContext.New(Name), $"Failed to process message, ID: {message.Id}; Exception: {e}");
                if (IsTransactional)
                {
                    _retryCountPerMessageKeeper.IncrementFailuresForMessage(message.Id);
                }
                throw;
            }

            if (IsTransactional)
            {
                _retryCountPerMessageKeeper.ClearFailuresForMessage(message.Id);
            }
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
