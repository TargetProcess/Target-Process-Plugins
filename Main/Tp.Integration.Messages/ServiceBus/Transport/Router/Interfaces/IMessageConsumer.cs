using System;
using System.Transactions;

namespace Tp.Integration.Messages.ServiceBus.Transport.Router.Interfaces
{
	public interface IMessageConsumer<TMessage> : IDisposable
	{
		Predicate<TMessage> While { get; set; }
		bool IsRunning { get; }
		string Name { get; }
		bool IsTransactional { get; set; }
		IsolationLevel IsolationLevel { get; set; }
		TimeSpan TransactionTimeout { get; set; }
		void AddObserver(IObserver<TMessage> observer);
		void Consume(Action<TMessage> handleMessage);
		void Dispose(string childTag);
	}
}
