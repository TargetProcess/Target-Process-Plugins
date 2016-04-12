// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using StructureMap;
using Tp.Integration.Common;
using Tp.Integration.Messages;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Messages.PluginLifecycle.PluginCommand;
using Tp.Integration.Plugin.Common.PluginCommand.Embedded;
using Tp.Integration.Plugin.Common.Validation;
using Tp.Integration.Plugin.TestRunImport.Mappers;

namespace Tp.Integration.Plugin.TestRunImport.Commands
{
	public class ValidateProfileForMappingCommand : IPluginCommand
	{
		public PluginCommandResponseMessage Execute(string args, UserDTO user)
		{
			return new PluginCommandResponseMessage
			{ResponseData = OnExecute(args), PluginCommandStatus = PluginCommandStatus.Succeed};
		}

		private static string OnExecute(string args)
		{
			var profile = args.DeserializeProfile();
			return ValidateProfileForMapping(profile).Serialize();
		}

		private static PluginProfileErrorCollection ValidateProfileForMapping(PluginProfileDto profileDto)
		{
			var errors = new PluginProfileErrorCollection();
			if (((TestRunImportPluginProfile)profileDto.Settings).PostResultsToRemoteUrl)
			{
				errors.Add(new PluginProfileError { FieldName = "Mapping", Message = "Check mapping is not available when results are posted to the remote Url" });
			}
			else
			{
				ObjectFactory.GetInstance<IMappingProfileValidator>().ValidateProfileForMapping(
					((TestRunImportPluginProfile) profileDto.Settings), errors);
			}
			return errors;
		}

		public string Name
		{
			get { return "ValidateProfileForMapping"; }
		}
	}
}