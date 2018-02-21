using System;
using System.Transactions;
using NServiceBus;
using NServiceBus.Config;
using NServiceBus.ObjectBuilder;
using Tp.Integration.Messages.ServiceBus.Transport.Router;

namespace Tp.Integration.Messages.ServiceBus.Transport
{
    /// <summary>
    /// Extends the base Configure class with MsmqTransport specific methods.
    /// Reads administrator set values from the MsmqTransportConfig section
    /// of the app.config.
    /// </summary>
    public class ConfigMsmqTransport<TTransport> : Configure
        where TTransport : IMsmqTransport
    {
        /// <summary>
        /// Wraps the given configuration object but stores the same 
        /// builder and configurer properties.
        /// </summary>
        /// <param name="config"></param>
        public void Configure(Configure config)
        {
            Builder = config.Builder;
            Configurer = config.Configurer;

            transportConfig = Configurer.ConfigureComponent<TTransport>(ComponentCallModelEnum.Singleton);
            transportConfig.ConfigureProperty(x => x.PluginQueueFactory, new PluginQueueFactory());

            var cfg = GetConfigSection<MsmqTransportConfig>();

            if (cfg != null)
            {
                transportConfig.ConfigureProperty(t => t.InputQueue, cfg.InputQueue);
                transportConfig.ConfigureProperty(t => t.NumberOfWorkerThreads, cfg.NumberOfWorkerThreads);
                transportConfig.ConfigureProperty(t => t.ErrorQueue, cfg.ErrorQueue);
                transportConfig.ConfigureProperty(t => t.MaxRetries, cfg.MaxRetries);
            }
        }

        private IComponentConfig<TTransport> transportConfig;

        /// <summary>
        /// Sets the transaction mode of the endpoint.
        /// When set to <see cref="TransportTransactionMode.None"/> the endpoint will lose messages if an exception occurs
        /// When set to <see cref="TransportTransactionMode.QueueOnly"/> the endpoint will note lose messages if an exception occurs
        /// When set to <see cref="TransportTransactionMode.TransactionScope"/> message will be proccessed inside distributed transaction scope
        /// </summary>
        /// <param name="transactionMode"></param>
        /// <returns></returns>
        public ConfigMsmqTransport<TTransport> TransactionMode(TransportTransactionMode transactionMode)
        {
            transportConfig.ConfigureProperty(t => t.TransactionMode, transactionMode);
            return this;
        }

        public ConfigMsmqTransport<TTransport> HostingMode(RoutableTransportMode value)
        {
            transportConfig.ConfigureProperty(t => t.RoutableTransportMode, value);
            return this;
        }

        /// <summary>
        /// Requests that the incoming queue be purged of all messages when the bus is started.
        /// All messages in this queue will be deleted if this is true.
        /// Setting this to true may make sense for certain smart-client applications, 
        /// but rarely for server applications.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ConfigMsmqTransport<TTransport> PurgeOnStartup(bool value)
        {
            transportConfig.ConfigureProperty(t => t.PurgeOnStartup, value);
            return this;
        }

        /// <summary>
        /// Sets the isolation level that database transactions on this endpoint will run at.
        /// This value is only relevant when <see cref="TransactionMode"/> has been set to <see cref="TransportTransactionMode.TransactionScope"/>.
        /// 
        /// Higher levels like RepeatableRead and Serializable promise a higher level
        /// of consistency, but at the cost of lower parallelism and throughput.
        /// 
        /// If you wish to run sagas on this endpoint, RepeatableRead is the suggested value
        /// and is the default value.
        /// </summary>
        /// <param name="isolationLevel"></param>
        /// <returns></returns>
        public ConfigMsmqTransport<TTransport> IsolationLevel(IsolationLevel isolationLevel)
        {
            transportConfig.ConfigureProperty(t => t.IsolationLevel, isolationLevel);
            return this;
        }

        /// <summary>
        /// If queues configured do not exist, will not cause them
        /// to be created on startup.
        /// </summary>
        /// <returns></returns>
        public ConfigMsmqTransport<TTransport> DoNotCreateQueues()
        {
            transportConfig.ConfigureProperty(t => t.DoNotCreateQueues, true);
            return this;
        }

        /// <summary>
        /// Sets the time span where a transaction will timeout.
        /// 
        /// Most endpoints should leave it at the default.
        /// </summary>
        /// <param name="transactionTimeout"></param>
        /// <returns></returns>
        public ConfigMsmqTransport<TTransport> TransactionTimeout(TimeSpan transactionTimeout)
        {
            transportConfig.ConfigureProperty(t => t.TransactionTimeout, transactionTimeout);
            return this;
        }

        /// <summary>
        /// Sets the maximum number of attempts to process message when exception occurs.
        /// When set to <see cref="int.MaxValue"/> message is retried to process until success (unlimited number of attempts).
        /// This value is only relevant when <see cref="TransactionMode"/> has been
        /// set to <see cref="TransportTransactionMode.QueueOnly"/> or <see cref="TransportTransactionMode.TransactionScope"/>.
        /// 
        /// Most endpoints should leave it at the default.
        /// </summary>
        /// <param name="maxRetries"></param>
        /// <returns></returns>
        public ConfigMsmqTransport<TTransport> MaxRetries(int maxRetries)
        {
            transportConfig.ConfigureProperty(t => t.MaxRetries, maxRetries);
            return this;
        }
    }
}
