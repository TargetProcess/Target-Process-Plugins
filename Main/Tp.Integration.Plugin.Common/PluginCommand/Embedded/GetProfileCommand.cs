// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Common;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Messages.PluginLifecycle.PluginCommand;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Integration.Plugin.Common.PluginCommand.Embedded
{
	public class GetProfileCommand : IPluginCommand
	{
		private readonly IProfileCollection _profileCollection;

		public GetProfileCommand(IProfileCollection profileCollection)
		{
			_profileCollection = profileCollection;
		}

		public PluginCommandResponseMessage Execute(string args, UserDTO user)
		{
			return new PluginCommandResponseMessage
			       	{
			       		ResponseData = _profileCollection[args].ConvertToDto().Serialize(),
			       		PluginCommandStatus = PluginCommandStatus.Succeed
			       	};
		}

		public string Name
		{
			get { return EmbeddedPluginCommands.GetProfile; }
		}
	}
}