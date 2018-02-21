using System;
using System.Messaging;

namespace Tp.Integration.Messages.ServiceBus.Transport.Router.Interfaces
{
    public interface IMessageProducer<in TMessage> : IDisposable
    {
        string Name { get; }
        void Produce(TMessage message, MessageQueueTransaction messageQueueTransaction);
        void Produce(TMessage message, MessageQueueTransactionType messageQueueTransactionType);
    }
}
