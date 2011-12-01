// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using StructureMap;
using Tp.Git.VersionControlSystem;
using Tp.Integration.Plugin.Common.Activity;
using Tp.SourceControl.Commands;
using Tp.SourceControl.Settings;
using Tp.SourceControl.StructureMap;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.Git.StructureMap
{
	public class GitRegistry : SourceControlRegistry
	{
		protected override void ConfigureCheckConnectionErrorResolver()
		{
			For<ICheckConnectionErrorResolver>().Use<GitCheckConnectionErrorResolver>();
		}

		protected override void ConfigureSourceControlConnectionSettingsSource()
		{
			For<ISourceControlConnectionSettingsSource>().Use<GitCurrentProfileToConnectionSettingsAdapter>();
		}

		protected override void ConfigureRevisionIdComparer()
		{
			For<IRevisionIdComparer>().HybridHttpOrThreadLocalScoped().Use<GitRevisionIdComparer>();
		}

		protected override void ConfigureVersionControlSystem()
		{
			For<IVersionControlSystem>().Use(() =>
				new GitVersionControlSystem(
					ObjectFactory.GetInstance<ISourceControlConnectionSettingsSource>(),
					ObjectFactory.GetInstance<ICheckConnectionErrorResolver>(),
					ObjectFactory.GetInstance<IActivityLogger>()));
		}
	}
}