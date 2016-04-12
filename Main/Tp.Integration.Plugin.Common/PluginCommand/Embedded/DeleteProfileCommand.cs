// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.IO;
using Tp.Integration.Common;
using Tp.Integration.Messages;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Messages.PluginLifecycle.PluginCommand;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Integration.Plugin.Common.PluginCommand.Embedded
{
	public class DeleteProfileCommand : ProfileCommandBase, IPluginCommand
	{
		public DeleteProfileCommand(IProfileCollection profileCollection, ITpBus bus, IPluginContext pluginContext)
			: base(profileCollection, bus, pluginContext)
		{
		}

		public string Name
		{
			get { return EmbeddedPluginCommands.DeleteProfile; }
		}

		public PluginCommandResponseMessage Execute(string args, UserDTO user)
		{
			DeletePluginProfile(PluginContext.ProfileName);
			SendProfileChangedLocalMessage(new PluginProfile().Name, new ProfileDeletedMessage());

			return new PluginCommandResponseMessage
			       	{ResponseData = string.Empty, PluginCommandStatus = PluginCommandStatus.Succeed};
		}

		public void DeletePluginProfile(ProfileName pluginProfile)
		{
			ChangeProfiles(profileCollection =>
			               	{
			               		var profile = profileCollection[pluginProfile];
			               		profile.Log.Remove();
								profile.FileStorage.Clear();
			               		profileCollection.Remove(profile);
			               	});
		}
	}
}