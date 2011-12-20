using NServiceBus;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.MashupManager
{
	public class DeleteProfileCommandHandler : IHandleMessages<ExecutePluginCommandCommand>
	{
		private readonly ITpBus _tpBus;
		private readonly IPluginMetadata _pluginMetadata;

		public DeleteProfileCommandHandler(ITpBus tpBus, IPluginMetadata pluginMetadata)
		{
			_tpBus = tpBus;
			_pluginMetadata = pluginMetadata;
		}

		public void Handle(ExecutePluginCommandCommand message)
		{
			if(message.CommandName == EmbeddedPluginCommands.DeleteProfile)
			{
				_tpBus.DoNotContinueDispatchingCurrentMessageToHandlers();
				_tpBus.Reply(new PluginCommandResponseMessage
				{
					ResponseData =
						string.Format("Cannot delete profile for '{0}' plugin", _pluginMetadata.PluginData.Name),
					PluginCommandStatus = PluginCommandStatus.Error
				});
			}
		}
	}
}