using System;
using System.Collections.Generic;
using System.Messaging;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Transactions;
using Tp.Integration.Messages.ServiceBus.Transport;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Interfaces;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Log;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Pump;

namespace Tp.Integration.Plugin.Common.Tests.Router.Model
{
    class TestRouterFactory : IRouterFactory<TestMessage>, IProducerConsumerFactory<TestMessage>
    {
        private readonly Waiter _waiter;
        private readonly Action<IList<TestMessage>> _orderChecker;
        private readonly Dictionary<string, MessageQueue<TestMessage>> _queues;

        public TestRouterFactory(Waiter waiter, Action<IList<TestMessage>> orderChecker)
        {
            _waiter = waiter;
            _orderChecker = orderChecker;
            _queues = new Dictionary<string, MessageQueue<TestMessage>>();
        }

        public IMessageSource<TestMessage> CreateSource(string sourceName, bool isChild)
        {
            MessageQueue<TestMessage> messageQueue = GetOrCreateMessageQueue(sourceName);
            return new MessageSource<TestMessage>(sourceName, GetMessageStream(messageQueue));
        }

        private IEnumerable<IObservable<TestMessage>> GetMessageStream(MessageQueue<TestMessage> messageQueue)
        {
            while (true)
            {
                yield return Observable.Start(() => messageQueue.Dequeue());
            }
        }

        public IMessageConsumer<TestMessage> CreateConsumer(IMessageSource<TestMessage> messageSource)
        {
            var consumer = new MessageConsumer<TestMessage>(messageSource, Scheduler.ThreadPool, new LoggerContextSensitive())
            {
                While = m => TestMessage.IsNotStopMessage(messageSource.Name, m)
            };
            _waiter.Register(consumer);
            consumer.AddObserver(new AccumulatingObserver(_orderChecker));
            return consumer;
        }

        public IMessageProducer<TestMessage> CreateProducer(IMessageSource<TestMessage> messageSource)
        {
            return new TestMessageProducer(GetOrCreateMessageQueue(messageSource.Name));
        }

        public IMessageConsumer<TestMessage> CreateRouter(IMessageSource<TestMessage> messageSource,
            IProducerConsumerFactory<TestMessage> factory, Func<TestMessage, string> routeBy, 
            TransportTransactionMode transactionMode = default(TransportTransactionMode), 
            IsolationLevel isolationLevel = default(IsolationLevel),
            TimeSpan transactionTimeout = default(TimeSpan),
            MessageQueueTransactionType sendTransactionType = default(MessageQueueTransactionType),
            MessageQueueTransactionType receiveTransactionType = default(MessageQueueTransactionType))
        {
            return new TestMessageRouter(messageSource, factory, routeBy, Scheduler.CurrentThread, new LoggerContextSensitive(),
                transactionMode, isolationLevel, transactionTimeout, sendTransactionType, receiveTransactionType);
        }

        private MessageQueue<TestMessage> GetOrCreateMessageQueue(string sourceName)
        {
            MessageQueue<TestMessage> queue;
            if (!_queues.TryGetValue(sourceName, out queue))
            {
                queue = new MessageQueue<TestMessage>();
                _queues.Add(sourceName, queue);
            }
            return queue;
        }
    }
}
