using System;

namespace Tp.Integration.Messages.ServiceBus.Transport.Router.Interfaces
{
	public interface IRouterFactory<TMessage>
	{
		IMessageConsumer<TMessage> CreateRouter(IMessageSource<TMessage> messageSource, IProducerConsumerFactory<TMessage> factory, Func<TMessage, string> routeBy);
	}
}