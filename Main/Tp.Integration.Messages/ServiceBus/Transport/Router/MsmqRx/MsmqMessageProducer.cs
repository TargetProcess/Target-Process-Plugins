using System;
using System.Messaging;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Interfaces;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Log;

namespace Tp.Integration.Messages.ServiceBus.Transport.Router.MsmqRx
{
	class MsmqMessageProducer : IMessageProducer<MessageEx>
	{
		private readonly string _name;
		private readonly MessageQueue _queue;
		private readonly Func<MessageQueueTransactionType> _transactionType;
		private readonly ILoggerContextSensitive _log;

		public MsmqMessageProducer(string queueName, Func<MessageQueueTransactionType> transactionType, ILoggerContextSensitive log)
		{
			_name = queueName + "~producer";
			_queue = MessageQueueFactory.GetOrCreateMessageQueue(queueName);
			_transactionType = transactionType;
			_log = log;
		}

		public void Produce(MessageEx message)
		{
			_queue.Send(message.Message, _transactionType());
			_log.Debug(LoggerContext.New(_name), "msg was routed to child queue.");
		}

		public string Name
		{
			get { return _name; }
		}

		public void Dispose()
		{
			string path = _queue.Path;
			_queue.Dispose();
			MessageQueue.Delete(path);
		}
	}
}