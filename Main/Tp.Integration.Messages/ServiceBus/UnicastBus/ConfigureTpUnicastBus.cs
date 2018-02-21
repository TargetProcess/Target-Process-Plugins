﻿using NServiceBus;

namespace Tp.Integration.Messages.ServiceBus.UnicastBus
{
    /// <summary>
    /// Contains extension methods to NServiceBus.Configure.
    /// </summary>
    public static class ConfigureTpUnicastBus
    {
        /// <summary>
        /// Use unicast messaging (your best option on nServiceBus right now).
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static ConfigTpUnicastBus TpUnicastBus(this Configure config)
        {
            var cfg = new ConfigTpUnicastBus();
            cfg.Configure(config);

            return cfg;
        }
    }
}
