// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Linq;
using Tp.BugTracking.ConnectionValidators;
using Tp.BugTracking.Settings;
using Tp.Bugzilla.Schemas;
using Tp.Integration.Plugin.Common.Validation;

namespace Tp.Bugzilla.ConnectionValidators
{
	public class SettingsValidator : Validator
	{
		private readonly DeserializeValidator _deserializeValidator;

		public const string BUGZILLA_VERSION_IS_NOT_SUPPORTED_BY_PLUGIN = "Version of Bugzilla '{0}' is not supported by this plugin. We can't guarantee that Bugzilla plugin will work correctly.";
		public const string BUGZILLA_VERSION_IS_NOT_SUPPORTED_BY_TP2_CGI = "Bugzilla version '{0}' is not supported by 'tp2.cgi'. Please update 'tp2.cgi' and try again.";
		public const string TP2_CGI_IS_NOT_SUPPORTED_BY_THIS_PLUGIN = "The 'tp2.cgi' script version '{0}' is not supported by this plugin. Please update your 'tp2.cgi' script and try again.";

		public SettingsValidator(IBugTrackingConnectionSettingsSource connectionSettings, DeserializeValidator deserializeValidator)
			: base(connectionSettings)
		{
			_deserializeValidator = deserializeValidator;
		}

		protected override void ExecuteConcreate(PluginProfileErrorCollection errors)
		{
			CheckVersions(_deserializeValidator.Data, errors);
		}

		private static void CheckVersions(bugzilla_properties bugzillaProperties, PluginProfileErrorCollection errors)
		{
			if (!ScriptVersionIsValid(bugzillaProperties.script_version))
			{
				errors.Add(
					new PluginProfileError
						{
							Message = string.Format(
								TP2_CGI_IS_NOT_SUPPORTED_BY_THIS_PLUGIN,
								string.IsNullOrEmpty(bugzillaProperties.script_version) ? "undefined" : bugzillaProperties.script_version),
							AdditionalInfo = ValidationErrorType.InvalidTpCgiVersion.ToString()
						});
			}

			if (!errors.Any() && !BugzillaVersionIsSupported(bugzillaProperties.version))
			{
				errors.Add(new PluginProfileError
				           	{
				           		Message = string.Format(
				           			BUGZILLA_VERSION_IS_NOT_SUPPORTED_BY_PLUGIN,
				           			bugzillaProperties.version),
									AdditionalInfo = ValidationErrorType.InvalidBugzillaVersion.ToString()
				           	});
			}

			if (!errors.Any() && !ScriptSupportsProvidedBugzillaVersion(bugzillaProperties.version, bugzillaProperties.supported_bugzilla_version))
			{
				errors.Add(new PluginProfileError
				           	{
								Message = string.Format(
									BUGZILLA_VERSION_IS_NOT_SUPPORTED_BY_TP2_CGI,
									bugzillaProperties.version),
								AdditionalInfo = ValidationErrorType.InvalidTpCgiVersion.ToString()
							});
			}
		}

		public static bool ScriptVersionIsValid(string scriptVersion)
		{
			return scriptVersion.StartsWith(BugzillaInfo.SupportedCgiScriptVersion);
		}

		public static bool BugzillaVersionIsSupported(string bugzillaVersion)
		{
			return BugzillaInfo.SupportedVersions.Any(bugzillaVersion.StartsWith);
		}

		public static bool ScriptSupportsProvidedBugzillaVersion(string bugzillaVersion, string supportedBugzillaVersion)
		{
			return string.IsNullOrEmpty(supportedBugzillaVersion) || bugzillaVersion.StartsWith(supportedBugzillaVersion);
		}
	}
}