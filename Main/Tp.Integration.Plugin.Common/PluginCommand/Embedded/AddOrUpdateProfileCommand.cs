// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Linq;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Validation;

namespace Tp.Integration.Plugin.Common.PluginCommand.Embedded
{
	public class AddOrUpdateProfileCommand : EditProfileCommandBase
	{
		private readonly IProfileCollection _profileCollection;

		public AddOrUpdateProfileCommand(ITpBus bus, IProfileCollection profileCollection, IPluginContext pluginContext,
		                                 IPluginMetadata pluginMetadata)
			: base(profileCollection, bus, pluginContext, pluginMetadata)
		{
			_profileCollection = profileCollection;
		}

		public override string Name
		{
			get { return EmbeddedPluginCommands.AddOrUpdateProfile; }
		}

		protected override PluginCommandResponseMessage OnExecute(PluginProfileDto profileDto)
		{
			NormalizeProfile(profileDto);

			ITargetProcessMessage message;

			if (_profileCollection.Any(x => x.Name == profileDto.Name))
			{
				UpdatePluginProfile(profileDto);
				message = new ProfileUpdatedMessage();
			}
			else
			{
				AddPluginProfile(profileDto);
				message = new ProfileAddedMessage();
			}

			SendProfileChangedLocalMessage(profileDto.Name, message);

			return new PluginCommandResponseMessage
			       	{ResponseData = string.Empty, PluginCommandStatus = PluginCommandStatus.Succeed};
		}

		private void UpdatePluginProfile(PluginProfileDto pluginProfile)
		{
			var errors = new PluginProfileErrorCollection();
			ValidateProfile(pluginProfile, errors);
			HandleErrors(errors);

			ChangeProfiles(profileCollection =>
			               	{
			               		var profile = profileCollection[pluginProfile.Name];
			               		profile.Settings = pluginProfile.Settings;
			               		if (PluginMetadata.IsUpdatedProfileInitializable)
			               		{
			               			profile.MarkAsNotInitialized();
			               		}
			               		profile.Save();
			               	});
		}
	}
}