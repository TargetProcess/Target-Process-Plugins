using System;
using System.Collections.Generic;

namespace Tp.Integration.Messages.ServiceBus.Transport.Router.Interfaces
{
	public interface IMessageSource<TMessage> : IEnumerable<IObservable<TMessage>>
	{
		string Name { get; }
	}
}
