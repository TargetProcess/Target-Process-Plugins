// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Linq;
using Tp.Bugzilla.Schemas;
using Tp.Integration.Plugin.Common.Validation;

namespace Tp.Bugzilla.ConnectionValidators
{
	public class SettingsValidator : Validator
	{
		private readonly DeserializeValidator _deserializeValidator;

		public SettingsValidator(BugzillaProfile profile, DeserializeValidator deserializeValidator) : base(profile)
		{
			_deserializeValidator = deserializeValidator;
		}

		protected override void ExecuteConcreate(PluginProfileErrorCollection errors)
		{
			CheckVersions(_deserializeValidator.Data, errors);
		}

		private static void CheckVersions(bugzilla_properties bugzillaProperties, PluginProfileErrorCollection errors)
		{
			if (!bugzillaProperties.script_version.StartsWith(BugzillaInfo.SupportedCgiScriptVersion))
				errors.Add(
					new PluginProfileError
						{
							Message = string.Format(
								"The 'tp2.cgi' script version '{0}' is not supported by this plugin. Please update your 'tp2.cgi' script and try again.",
								string.IsNullOrEmpty(bugzillaProperties.script_version) ? "undefined" : bugzillaProperties.script_version),
								AdditionalInfo = ValidationErrorType.InvalidTpCgiVersion.ToString()
						});

			if (!BugzillaInfo.SupportedVersions.Any(version => bugzillaProperties.version.StartsWith(version)))
			{
				errors.Add(new PluginProfileError
				           	{
				           		Message = string.Format(
				           			"Version of Bugzilla '{0}' is not supported by this plugin. We can't guarantee that Bugzilla plugin will work correctly.",
				           			bugzillaProperties.version),
									AdditionalInfo = ValidationErrorType.InvalidBugzillaVersion.ToString()
				           	});
			}
		}
	}
}