namespace Tp.Integration.Messages.ServiceBus.Transport
{
	public class PluginQueueFactory : IPluginQueueFactory
	{
		public IPluginQueue Create(string queueName)
		{
			return new PluginQueue(queueName);
		}
	}

	public interface IPluginQueueFactory
	{
		IPluginQueue Create(string queueName);
	}
}
