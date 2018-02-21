using System;

namespace Tp.Integration.Messages.PluginLifecycle
{
    [Serializable]
    public class PluginInfoMessage : IPluginLifecycleMessage
    {
        public PluginInfoMessage()
        {
            Info = new PluginInfo();
        }

        public PluginInfo Info { get; set; }
    }
}
