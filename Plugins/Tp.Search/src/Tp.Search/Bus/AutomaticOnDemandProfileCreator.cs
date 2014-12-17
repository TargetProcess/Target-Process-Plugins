// 
// Copyright (c) 2005-2014 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using StructureMap;
using Tp.Integration.Messages;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Messages.ServiceBus.Transport.Router.MsmqRx;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.PluginCommand.Embedded;
using Tp.Search.Bus.Commands;

namespace Tp.Search.Bus
{
	public class AutomaticOnDemandProfileCreator : ITargetProcessMessageWhenNoProfilesHandler, ITargetProcessConditionalMessageRouter
	{
		private readonly IPluginContext _pluginContext;
		private readonly ITpBus _bus;
		public AutomaticOnDemandProfileCreator(IPluginContext pluginContext, ITpBus bus)
		{
			_pluginContext = pluginContext;
			_bus = bus;
		}

		public void Handle(ITargetProcessMessage message)
		{
			CreateProfile(message);
		}

		public bool Handle(MessageEx message)
		{
			return message.AccountTag != AccountName.Empty;
		}

		private void CreateProfile(ITargetProcessMessage message)
		{
			if (message.IsMessageOfCrudType() && _pluginContext.AccountName != AccountName.Empty)
			{
				var command = ObjectFactory.GetInstance<AddOrUpdateProfileCommand>();
				var result = command.Execute(new PluginProfileDto { Name = SearcherProfile.Name }.Serialize());
				if (result.PluginCommandStatus == PluginCommandStatus.Succeed)
				{
					_bus.SendLocalWithContext(new ProfileName(SearcherProfile.Name), _pluginContext.AccountName, new ExecutePluginCommandCommand
					{
						CommandName = SetEnableForTp2.CommandName,
						Arguments = bool.TrueString
					});
				}
			}
		}
	}
}