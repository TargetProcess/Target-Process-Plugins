using System.Configuration;

namespace Tp.Integration.Messages.ServiceBus.UnicastBus
{
    public class PluginRouterSection
        : ConfigurationSection
    {
        [ConfigurationProperty("Mode", IsRequired = true)]
        public PluginRouterMode Mode => (PluginRouterMode) this["Mode"];

        [ConfigurationProperty("ProxyQueue", IsRequired = true)]
        public string ProxyQueue => (string) this["ProxyQueue"];

        [ConfigurationProperty("ProxyPath", IsRequired = false, DefaultValue = "PluginRouter\\bin\\Tp.Integration.Router.exe")]
        public string ProxyPath => (string) this["ProxyPath"];
    }
}
