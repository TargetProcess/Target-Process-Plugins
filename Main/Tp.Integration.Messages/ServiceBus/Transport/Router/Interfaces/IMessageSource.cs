using System;

namespace Tp.Integration.Messages.ServiceBus.Transport.Router.Interfaces
{
	public interface IMessageSource<TMessage> : IObservable<TMessage>
	{
		string Name { get; }
	}
}