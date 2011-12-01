using System;

namespace Tp.Integration.Messages.ServiceBus.Transport.Router.Interfaces
{
	public interface IMessageProducer<in TMessage> : IDisposable
	{
		void Produce(TMessage message);

		string Name { get; }
	}
}