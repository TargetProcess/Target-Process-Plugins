// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Messages.PluginLifecycle.PluginCommand;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Validation;

namespace Tp.Integration.Plugin.Common.PluginCommand.Embedded
{
	public abstract class EditProfileCommandBase : ProfileCommandBase, IPluginCommand
	{
		private readonly IPluginMetadata _pluginMetadata;

		protected EditProfileCommandBase(IProfileCollection profileCollection, ITpBus bus, IPluginContext pluginContext,
		                                 IPluginMetadata pluginMetadata)
			: base(profileCollection, bus, pluginContext)
		{
			_pluginMetadata = pluginMetadata;
		}

		public PluginCommandResponseMessage Execute(string args)
		{
			var profileDto = args.DeserializeProfile();
			return OnExecute(profileDto);
		}

		public abstract string Name { get; }

		protected IPluginMetadata PluginMetadata
		{
			get { return _pluginMetadata; }
		}

		protected void NormalizeProfile(PluginProfileDto dto)
		{
			if (dto.Name == null)
				dto.Name = string.Empty;

			dto.Name = dto.Name.Trim();
		}

		protected abstract PluginCommandResponseMessage OnExecute(PluginProfileDto profileProfileDto);

		public void AddPluginProfile(PluginProfileDto pluginProfile)
		{
			var errors = new PluginProfileErrorCollection();
			ValidateUniqueness(pluginProfile, errors);
			ValidateProfile(pluginProfile, errors);
			HandleErrors(errors);

			ChangeProfiles(
				profileCollection => profileCollection.Add(new ProfileCreationArgs(pluginProfile.Name, pluginProfile.Settings)));
		}

		protected static void ValidateProfile(PluginProfileDto pluginProfile, PluginProfileErrorCollection errors)
		{
			new ProfileDtoValidator(pluginProfile).Validate(errors);
		}

		protected static void HandleErrors(PluginProfileErrorCollection errors)
		{
			if (!errors.Empty())
			{
				throw new PluginProfileValidationException(errors);
			}
		}

		private void ValidateUniqueness(PluginProfileDto pluginProfile, PluginProfileErrorCollection errors)
		{
			if (
				ProfileCollection.Any(
					x => string.Equals(x.Name.Value, pluginProfile.Name, StringComparison.InvariantCultureIgnoreCase)))
			{
				errors.Add(new PluginProfileError {FieldName = "Name", Message = "Profile name should be unique for plugin"});
			}
		}
	}
}