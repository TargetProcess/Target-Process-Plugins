namespace Tp.Integration.Messages.PluginLifecycle
{
    public class PluginAccountMessageSerialized : IPluginLifecycleMessage
    {
        public string SerializedMessage { get; set; }

        public PluginAccount[] GetAccounts()
        {
            return string.IsNullOrEmpty(SerializedMessage) ? new PluginAccount[] { } : SerializedMessage.Deserialize<PluginAccount[]>();
        }
    }
}
