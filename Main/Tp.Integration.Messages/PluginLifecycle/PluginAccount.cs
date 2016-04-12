using System;

namespace Tp.Integration.Messages.PluginLifecycle
{
	[Serializable]
	public class PluginAccount
	{
		public PluginAccount()
		{
			PluginProfiles = new PluginProfile[] { };
			Name = AccountName.Empty;
			PluginName = string.Empty;
		}

		public PluginName PluginName { get; set; }

		public AccountName Name { get; set; }

		public PluginProfile[] PluginProfiles { get; set; }
	}
}
