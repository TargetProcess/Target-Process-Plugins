using Tp.Integration.Messages;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Messages.PluginLifecycle.PluginCommand;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.PluginCommand.Embedded;

namespace Tp.Search.Bus.Commands
{
	class BuildSearchIndexesCommand : IPluginCommand
	{
		private readonly ITpBus _bus;
		private readonly IProfileCollection _profileCollection;
		private readonly IPluginContext _pluginContext;
		private readonly IPluginMetadata _pluginMetadata;
			
		public BuildSearchIndexesCommand(ITpBus bus, IProfileCollection profileCollection, IPluginContext pluginContext, IPluginMetadata pluginMetadata)
		{
			_bus = bus;
			_profileCollection = profileCollection;
			_pluginContext = pluginContext;
			_pluginMetadata = pluginMetadata;
		}

		public PluginCommandResponseMessage Execute(string _)
		{
			bool wasNoProfile = _pluginContext.ProfileName.IsEmpty;
			var c = new AddOrUpdateProfileCommand(_bus, _profileCollection, _pluginContext, _pluginMetadata);
			var profile = new PluginProfileDto { Name = SearcherProfile.Name }.Serialize();
			var result = c.Execute(profile);
			if (result.PluginCommandStatus == PluginCommandStatus.Succeed && wasNoProfile)
			{
				_bus.SendLocalWithContext(new ProfileName(SearcherProfile.Name), _pluginContext.AccountName, new ExecutePluginCommandCommand
					{
						CommandName = SetEnableForTp2.CommandName,
						Arguments = bool.TrueString
					});
			}
			return result;
		}

		public string Name { get { return "BuildSearchIndexes"; } }
	}
}