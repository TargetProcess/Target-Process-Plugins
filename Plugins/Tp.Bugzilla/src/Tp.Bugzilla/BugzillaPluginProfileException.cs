// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using Tp.Integration.Plugin.Common.Validation;

namespace Tp.Bugzilla
{
	public class BugzillaPluginProfileException : Exception
	{
		private readonly BugzillaProfile _bugzillaProfile;

		public BugzillaPluginProfileException(BugzillaProfile bugzillaProfile, PluginProfileErrorCollection errorCollection)
		{
			_bugzillaProfile = bugzillaProfile;
			ErrorCollection = errorCollection;
		}

		public override string Message
		{
			get
			{
				return string.Format("Bugzilla profile '{0}' is not valid. Reasons : '{1}'", _bugzillaProfile,
				                     ErrorCollection.Select(x => x.Message).ToString("|"));
			}
		}

		public PluginProfileErrorCollection ErrorCollection { get; private set; }
	}
}