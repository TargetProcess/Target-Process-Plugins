using System;
using System.Messaging;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Log;

namespace Tp.Integration.Messages.ServiceBus.Transport.Router.MsmqRx
{
	class MsmqMessageProducer : MsmqMessageProducerBase<MessageEx>
	{
		private readonly ILoggerContextSensitive _log;
		public MsmqMessageProducer(string queueName, Func<MessageQueueTransactionType> transactionType, ILoggerContextSensitive log):base(queueName, transactionType)
		{
			_log = log;
		}

		public override void Produce(MessageEx message)
		{
			Send(message.Message);
			_log.Debug(LoggerContext.New(Name), "msg was routed to child queue.");
		}
	}
}