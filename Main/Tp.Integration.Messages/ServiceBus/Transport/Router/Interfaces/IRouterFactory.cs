using System;
using System.Messaging;
using System.Transactions;

namespace Tp.Integration.Messages.ServiceBus.Transport.Router.Interfaces
{
    public interface IRouterFactory<TMessage>
    {
        IMessageConsumer<TMessage> CreateRouter(IMessageSource<TMessage> messageSource, IProducerConsumerFactory<TMessage> factory,
            Func<TMessage, string> routeBy, TransportTransactionMode transactionMode, IsolationLevel isolationLevel, TimeSpan transactionTimeout,
            MessageQueueTransactionType sendTransactionType, MessageQueueTransactionType receiveTransactionType);
    }
}
