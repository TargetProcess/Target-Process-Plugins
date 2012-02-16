// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using StructureMap.Configuration.DSL;
using Tp.SourceControl.Comments.DSL;
using Tp.SourceControl.Diff;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.SourceControl.StructureMap
{
	public abstract class SourceControlRegistry : Registry
	{
		protected SourceControlRegistry()
		{
			For<IActionFactory>().HybridHttpOrThreadLocalScoped().Use<ActionFactory>();
			For<IDiffProcessor>().HybridHttpOrThreadLocalScoped().Use<DiffProcessor>();
			For<IVersionControlSystemFactory>().HybridHttpOrThreadLocalScoped().Use<VersionControlSystemFactory>();
			ConfigureVersionControlSystem();
			ConfigureSourceControlConnectionSettingsSource();
			ConfigureRevisionIdComparer();
			ConfigureCheckConnectionErrorResolver();
			ConfigureRevisionStorage();
			ConfigureUserMapper();
		}

		protected abstract void ConfigureCheckConnectionErrorResolver();

		protected abstract void ConfigureSourceControlConnectionSettingsSource();

		protected abstract void ConfigureRevisionIdComparer();

		protected abstract void ConfigureVersionControlSystem();

		protected abstract void ConfigureRevisionStorage();

		protected abstract void ConfigureUserMapper();
	}
}