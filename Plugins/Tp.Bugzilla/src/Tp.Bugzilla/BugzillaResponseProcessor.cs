// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using Tp.Bugzilla.ConnectionValidators;

namespace Tp.Bugzilla
{
	public class BugzillaResponseProcessor
	{
		private const string BUGZILLA_VERSION = "BUGZILLA VERSION: ";
		private const string SUPPORTED_BUGZILLA_VERSION = "SUPPORTED BUGZILLA VERSION: ";
		private const string SCRIPT_VERSION = "SCRIPT VERSION: ";

		private readonly string _bugzillaVersion;
		private readonly string _supportedBugzillaVersion;
		private readonly string _scriptVersion;
		private readonly string[] _parts;

		public BugzillaResponseProcessor(string content)
		{
			_parts = content.Split(new[] {'\n'}, StringSplitOptions.RemoveEmptyEntries);

			if (_parts.Length < 3)
			{
				return;
			}

			_bugzillaVersion = GetVersion(_parts[0], BUGZILLA_VERSION);
			_supportedBugzillaVersion = GetVersion(_parts[1], SUPPORTED_BUGZILLA_VERSION);
			_scriptVersion = GetVersion(_parts[2], SCRIPT_VERSION);
		}

		public string Process()
		{
			Validate();

			var content = GetContent();

			AssertIsNotError(content);

			return content;
		}

		private void Validate()
		{
			if (!HasValue())
			{
				return;
			}

			if (!SettingsValidator.BugzillaVersionIsSupported(_bugzillaVersion))
			{
				throw new ApplicationException(string.Format(SettingsValidator.BUGZILLA_VERSION_IS_NOT_SUPPORTED_BY_PLUGIN,
				                                             _bugzillaVersion));
			}

			if (!SettingsValidator.ScriptSupportsProvidedBugzillaVersion(_bugzillaVersion, _supportedBugzillaVersion))
			{
				throw new ApplicationException(string.Format(SettingsValidator.BUGZILLA_VERSION_IS_NOT_SUPPORTED_BY_TP2_CGI,
				                                             _bugzillaVersion));
			}

			if (!SettingsValidator.ScriptVersionIsValid(_scriptVersion))
			{
				throw new ApplicationException(string.Format(SettingsValidator.TP2_CGI_IS_NOT_SUPPORTED_BY_THIS_PLUGIN,
				                                             _scriptVersion));
			}
		}

		private bool HasValue()
		{
			return !string.IsNullOrEmpty(_bugzillaVersion) && !string.IsNullOrEmpty(_supportedBugzillaVersion) &&
			       !string.IsNullOrEmpty(_scriptVersion);
		}

		private static string GetVersion(string source, string key)
		{
			return !source.StartsWith(key) ? null : source.Substring(key.Length);
		}

		public string GetContent()
		{
			return HasValue() ? string.Join("\n", _parts.Skip(3)) : string.Join("\n", _parts);
		}

		private static void AssertIsNotError(string content)
		{
			if (content.StartsWith("ERROR"))
			{
				throw new ApplicationException(content.Substring(7));
			}
		}
	}
}