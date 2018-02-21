using System;
using System.Collections.Generic;
using System.Messaging;
using System.Reactive.Concurrency;
using System.Transactions;
using Tp.Integration.Messages.ServiceBus.Transport;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Interfaces;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Log;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Pump;

namespace Tp.Integration.Plugin.Common.Tests.Router.Model
{
    class TestMessageRouter : MessageRouter<TestMessage>
    {
        public TestMessageRouter(IMessageSource<TestMessage> messageSource, IProducerConsumerFactory<TestMessage> producerConsumerFactory,
            Func<TestMessage, string> tagMessageProvider, IScheduler scheduler, ILoggerContextSensitive log,
            TransportTransactionMode transactionMode, IsolationLevel isolationLevel, TimeSpan transactionTimeout,
            MessageQueueTransactionType sendTransactionType, MessageQueueTransactionType receiveTransactionType)
            : base(
                messageSource, producerConsumerFactory, tagMessageProvider, scheduler, log, transactionMode, isolationLevel,
                transactionTimeout, sendTransactionType, receiveTransactionType)
        {
        }

        public int ReceiveCallCount { get; private set; }

        protected override IEnumerable<string> GetChildTags()
        {
            return new string[] { };
        }

        protected override void Receive(TestMessage message, MessageQueueTransaction messageQueueTransaction)
        {
            ReceiveCallCount++;
        }

        protected override void Receive(TestMessage message, MessageQueueTransactionType messageQueueTransactionType)
        {
            ReceiveCallCount++;
        }

        protected override void Preprocess(TestMessage message)
        {
        }

        protected override bool NeedToHandle(TestMessage messageTag)
        {
            return true;
        }
    }
}
