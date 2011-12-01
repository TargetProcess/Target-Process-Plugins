// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Plugin.Common.Validation;
using Tp.SourceControl.Commands;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.Subversion.Commands
{
	public class SubversionCheckConnectionCommand : VcsCheckConnectionCommand<SubversionPluginProfile>
	{
		protected override void CheckStartRevision(SubversionPluginProfile settings, IVersionControlSystem versionControlSystem, PluginProfileErrorCollection errors)
		{
			if (settings.RevisionSpecified && settings.ValidateStartRevision(errors))
			{
				versionControlSystem.CheckRevision(settings.StartRevision, errors);
			}
		}
	}
}