using System.Messaging;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Interfaces;

namespace Tp.Integration.Plugin.Common.Tests.Router.Model
{
    class TestMessageProducer : IMessageProducer<TestMessage>
    {
        private readonly MessageQueue<TestMessage> _messageQueue;

        public TestMessageProducer(MessageQueue<TestMessage> messageQueue)
        {
            _messageQueue = messageQueue;
        }

        public void Produce(TestMessage message, MessageQueueTransaction messageQueueTransaction)
        {
            _messageQueue.Enqueue(message);
        }

        public void Produce(TestMessage message, MessageQueueTransactionType messageQueueTransactionType)
        {
            _messageQueue.Enqueue(message);
        }

        public string Name
        {
            get { return GetType().Name; }
        }

        public void Dispose()
        {
        }
    }
}
