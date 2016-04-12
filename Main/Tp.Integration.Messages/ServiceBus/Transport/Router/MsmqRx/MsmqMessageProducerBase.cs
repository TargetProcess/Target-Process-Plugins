using System;
using System.Messaging;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Interfaces;

namespace Tp.Integration.Messages.ServiceBus.Transport.Router.MsmqRx
{
	public abstract class MsmqMessageProducerBase<T> : IMessageProducer<T>
	{
		private readonly string _name;
		private readonly MessageQueue _queue;
		private readonly Func<MessageQueueTransactionType> _transactionType;

		protected MsmqMessageProducerBase(string queueName, Func<MessageQueueTransactionType> transactionType)
		{
			_name = queueName + "~producer";
			_queue = MessageQueueFactory.GetOrCreateMessageQueue(queueName);
			_transactionType = transactionType;
		}

		protected void Send(object obj)
		{
			_queue.Send(obj, _transactionType());
		}

		public string Name
		{
			get { return _name; }
		}

		public abstract void Produce(T message);

		public void Dispose()
		{
			string path = _queue.Path;
			_queue.Dispose();
			MessageQueue.Delete(path);
		}
	}
}
