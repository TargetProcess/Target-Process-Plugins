// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Git.Workflow;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle.PluginCommand;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Git.Commands
{
	public class RescanRepositoryCommand : IPluginCommand
	{
		private readonly ITpBus _bus;
		private readonly IPluginContext _pluginContext;

		public RescanRepositoryCommand(ITpBus bus, IPluginContext pluginContext)
		{
			_bus = bus;
			_pluginContext = pluginContext;
		}

		public PluginCommandResponseMessage Execute(string _)
		{
			_bus.SendLocalWithContext(_pluginContext.ProfileName, _pluginContext.AccountName, new RepositoryRescanInitiatedMessage());

			return new PluginCommandResponseMessage {PluginCommandStatus = PluginCommandStatus.Succeed, ResponseData = string.Empty};
		}

		public string Name { get { return "RescanRepository"; } }
	}
}