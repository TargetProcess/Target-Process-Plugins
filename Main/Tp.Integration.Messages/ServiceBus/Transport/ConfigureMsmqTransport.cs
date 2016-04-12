using NServiceBus;

namespace Tp.Integration.Messages.ServiceBus.Transport
{
	public static class ConfigureMsmqTransport
	{
		public static ConfigMsmqTransport<TTransport> MsmqTransport<TTransport>(this Configure config)
			where TTransport : IMsmqTransport
		{
			var cfg = new ConfigMsmqTransport<TTransport>();
			cfg.Configure(config);

			return cfg;
		}
	}
}
