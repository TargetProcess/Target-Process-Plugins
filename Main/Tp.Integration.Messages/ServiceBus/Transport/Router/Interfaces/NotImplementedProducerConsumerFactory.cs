namespace Tp.Integration.Messages.ServiceBus.Transport.Router.Interfaces
{
	public class NotImplementedProducerConsumerFactory<TMessage> : IProducerConsumerFactory<TMessage>
	{
		public IMessageSource<TMessage> CreateSource(string sourceName, bool isChild)
		{
			throw new System.NotImplementedException();
		}

		public IMessageConsumer<TMessage> CreateConsumer(IMessageSource<TMessage> source)
		{
			throw new System.NotImplementedException();
		}

		public IMessageProducer<TMessage> CreateProducer(IMessageSource<TMessage> source)
		{
			throw new System.NotImplementedException();
		}
	}
}
