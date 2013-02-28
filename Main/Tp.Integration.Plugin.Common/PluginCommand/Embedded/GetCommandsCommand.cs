// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Linq;
using Tp.Integration.Messages;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Messages.PluginLifecycle.PluginCommand;

namespace Tp.Integration.Plugin.Common.PluginCommand.Embedded
{
	public class GetCommandsCommand : IPluginCommand
	{
		private readonly IPluginCommandRepository _pluginCommandRepository;

		public GetCommandsCommand(IPluginCommandRepository pluginCommandRepository)
		{
			_pluginCommandRepository = pluginCommandRepository;
		}

		public PluginCommandResponseMessage Execute(string args)
		{
			return new PluginCommandResponseMessage
			       	{
						ResponseData = _pluginCommandRepository.Select(x => x.Name).Where(x => x != DeleteUnusedQueuesCommand.Deleteunusedqueues).OrderBy(x => x).ToArray().Serialize(),
			       		PluginCommandStatus = PluginCommandStatus.Succeed
			       	};
		}

		public string Name
		{
			get { return EmbeddedPluginCommands.GetCommands; }
		}
	}
}