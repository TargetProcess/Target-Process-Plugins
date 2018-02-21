using Tp.Integration.Messages;

namespace Tp.Integration.Plugin.Common.Domain
{
    public class PluginContextSnapshot : IPluginContextSnapshot
    {
        private readonly AccountName _accountName;
        private readonly ProfileName _profileName;
        private readonly PluginName _pluginName;

        public PluginContextSnapshot(IPluginContext context) : this(context.AccountName, context.ProfileName, context.PluginName)
        {
        }

        public PluginContextSnapshot(AccountName accountName, ProfileName profileName, PluginName pluginName)
        {
            _accountName = accountName;
            _profileName = profileName;
            _pluginName = pluginName;
        }

        public AccountName AccountName
        {
            get { return _accountName; }
        }

        public ProfileName ProfileName
        {
            get { return _profileName; }
        }

        public PluginName PluginName
        {
            get { return _pluginName; }
        }
    }
}
