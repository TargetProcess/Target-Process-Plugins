using System.Configuration;

namespace Tp.Integration.Messages.ServiceBus.UnicastBus
{
    public class PluginRouterSection
        : ConfigurationSection
    {
        [ConfigurationProperty("Mode", IsRequired = true)]
        public PluginRouterMode Mode
        {
            get { return (PluginRouterMode) this["Mode"]; }
        }

        [ConfigurationProperty("ProxyQueue", IsRequired = true)]
        public string ProxyQueue
        {
            get { return (string) this["ProxyQueue"]; }
        }

        [ConfigurationProperty("ProxyPath", IsRequired = false, DefaultValue = "PluginRouter\\bin\\Tp.Integration.Router.exe")]
        public string ProxyPath
        {
            get { return (string) this["ProxyPath"]; }
        }
    }
}
