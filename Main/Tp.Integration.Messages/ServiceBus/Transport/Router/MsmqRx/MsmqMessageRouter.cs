using System;
using System.Collections.Generic;
using System.Messaging;
using System.Reactive.Concurrency;
using System.Transactions;
using StructureMap;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Interfaces;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Log;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Pump;

namespace Tp.Integration.Messages.ServiceBus.Transport.Router.MsmqRx
{
    public class MsmqMessageRouter : MessageRouter<MessageEx>
    {
        protected IRouterChildTagsSource RouterChildTagsSource;

        public MsmqMessageRouter(IMessageSource<MessageEx> messageSource, IProducerConsumerFactory<MessageEx> producerConsumerFactory,
            Func<MessageEx, string> tagMessageProvider, IScheduler scheduler, ILoggerContextSensitive log,
            TransportTransactionMode transactionMode, IsolationLevel isolationLevel, TimeSpan transactionTimeout,
            MessageQueueTransactionType sendTransactionType, MessageQueueTransactionType receiveTransactionType)
            : base(
                messageSource, producerConsumerFactory, tagMessageProvider, scheduler, log, transactionMode, isolationLevel,
                transactionTimeout, sendTransactionType, receiveTransactionType)
        {
            RouterChildTagsSource = ObjectFactory.GetInstance<IRouterChildTagsSource>();
        }

        protected override void Receive(MessageEx message, MessageQueueTransaction messageQueueTransaction)
        {
            message.DoTransactionReceive(messageQueueTransaction);
        }

        protected override void Receive(MessageEx message, MessageQueueTransactionType messageQueueTransactionType)
        {
            message.DoTransactionTypeReceive(messageQueueTransactionType);
        }

        protected override IEnumerable<string> GetChildTags()
        {
            return RouterChildTagsSource.GetChildTags();
        }

        protected override bool NeedToHandle(MessageEx m)
        {
            return RouterChildTagsSource.NeedToHandleMessage(m);
        }

        protected override void Preprocess(MessageEx message)
        {
            MessageLabel messageLabel = MessageLabel.Parse(message.Message.Label);
            var idForCorelation = messageLabel.IdForCorrelation;
            if (string.IsNullOrWhiteSpace(idForCorelation))
            {
                idForCorelation = message.Message.Id;
            }
            var newMessageLabel = new MessageLabel(messageLabel.WindowsIdentityName, idForCorelation);
            message.Message.Label = newMessageLabel.ToString();
        }
    }
}
