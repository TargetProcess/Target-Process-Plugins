using System.Messaging;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Interfaces;

namespace Tp.Integration.Messages.ServiceBus.Transport.Router.MsmqRx
{
    public abstract class MsmqMessageProducerBase<T> : IMessageProducer<T>
    {
        private readonly string _name;
        private readonly MessageQueue _queue;

        protected MsmqMessageProducerBase(string queueName)
        {
            _name = queueName + "~producer";
            _queue = MessageQueueFactory.GetOrCreateMessageQueue(queueName);
        }

        protected void Send(object obj, MessageQueueTransaction messageQueueTransaction)
        {
            _queue.Send(obj, messageQueueTransaction);
        }


        protected void Send(object obj, MessageQueueTransactionType messageQueueTransactionType)
        {
            _queue.Send(obj, messageQueueTransactionType);
        }

        public string Name
        {
            get { return _name; }
        }

        public abstract void Produce(T message, MessageQueueTransaction messageQueueTransaction);
        public abstract void Produce(T message, MessageQueueTransactionType messageQueueTransactionType);

        public void Dispose()
        {
            string path = _queue.Path;
            _queue.Dispose();
            MessageQueue.Delete(path);
        }
    }
}
