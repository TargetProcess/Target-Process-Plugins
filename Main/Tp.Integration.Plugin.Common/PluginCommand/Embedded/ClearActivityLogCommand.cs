// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Messages;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Messages.PluginLifecycle.PluginCommand;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Integration.Plugin.Common.PluginCommand.Embedded
{
	public class ClearActivityLogCommand : IPluginCommand
	{
		private readonly IProfile _profile;

		public ClearActivityLogCommand(IProfile profile)
		{
			_profile = profile;
		}

		public PluginCommandResponseMessage Execute(string args)
		{
			var filter = args.Deserialize<ActivityFilter>();

			_profile.Log.ClearBy(filter);

			return new PluginCommandResponseMessage
			       	{ResponseData = string.Empty, PluginCommandStatus = PluginCommandStatus.Succeed};
		}

		public string Name
		{
			get { return EmbeddedPluginCommands.ClearActivityLog; }
		}
	}
}