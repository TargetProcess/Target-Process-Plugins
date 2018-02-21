using System;

namespace Tp.Integration.Messages.ServiceBus.Transport.Router.Interfaces
{
    public interface IMessageConsumer<TMessage> : IDisposable
    {
        Predicate<TMessage> While { get; set; }
        bool IsRunning { get; }
        string Name { get; }
        void AddObserver(IObserver<TMessage> observer);
        void Consume(Action<TMessage> handleMessage);
        void Dispose(string childTag);
    }
}
