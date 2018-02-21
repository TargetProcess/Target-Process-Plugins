using System.Messaging;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Log;

namespace Tp.Integration.Messages.ServiceBus.Transport.Router.MsmqRx
{
    class MsmqMessageProducer : MsmqMessageProducerBase<MessageEx>
    {
        private readonly ILoggerContextSensitive _log;

        public MsmqMessageProducer(string queueName, ILoggerContextSensitive log)
            : base(queueName)
        {
            _log = log;
        }

        public override void Produce(MessageEx message, MessageQueueTransaction messageQueueTransaction)
        {
            Send(message.Message, messageQueueTransaction);
            _log.Debug(LoggerContext.New(Name), "msg was routed to child queue.");
        }

        public override void Produce(MessageEx message, MessageQueueTransactionType messageQueueTransactionType)
        {
            Send(message.Message, messageQueueTransactionType);
            _log.Debug(LoggerContext.New(Name), "msg was routed to child queue.");
        }
    }
}
