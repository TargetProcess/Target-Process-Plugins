using NServiceBus;
using NServiceBus.MessageInterfaces.MessageMapper.Reflection;
using NServiceBus.ObjectBuilder;

namespace Tp.Integration.Messages.ServiceBus.Serialization
{
    public static class ConfigureAdvancedXmlSerializer
    {
        /// <summary>
        /// Use XML serialization that supports interface-based messages.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Configure AdvancedXmlSerializer(this Configure config)
        {
            config.Configurer.ConfigureComponent<MessageMapper>(ComponentCallModelEnum.Singleton);
            config.Configurer.ConfigureComponent<AdvancedXmlSerializer>(ComponentCallModelEnum.Singleton);

            return config;
        }
    }
}
