namespace Tp.Integration.Messages.ServiceBus.Transport.Router.Interfaces
{
	public interface IProducerConsumerFactory<TMessage>
	{
		IMessageSource<TMessage> CreateSource(string sourceName, bool isChild);
		IMessageConsumer<TMessage> CreateConsumer(IMessageSource<TMessage> source);
		IMessageProducer<TMessage> CreateProducer(IMessageSource<TMessage> source);
	}
}