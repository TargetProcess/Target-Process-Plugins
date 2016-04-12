// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Mashup;
using Tp.Tfs.RevisionStorage;
using Tp.Tfs.VersionControlSystem;
using Tp.SourceControl.Commands;
using Tp.SourceControl.RevisionStorage;
using Tp.SourceControl.Settings;
using Tp.SourceControl.StructureMap;
using Tp.SourceControl.VersionControlSystem;
using Tp.SourceControl.Workflow.Workflow;
using Tp.Tfs.WorkItemsIntegration;
using Tp.Tfs.WorkItemsIntegration.FeatureToggling;
using Tp.Tfs.Workflow;

namespace Tp.Tfs.StructureMap
{
	public class TfsRegistry : SourceControlRegistry
	{
		public TfsRegistry()
		{
			For<ICustomPluginSpecifyMessageHandlerOrdering>().Singleton().Use<PluginSpecifyMessageHandlerOrdering>();
			For<IExcludedAssemblyNamesSource>().HybridHttpOrThreadLocalScoped().Use<TfsPluginExcludedAssemblies>();

			For<IWorkItemsStore>().Use<TfsWorkItemsStore>();
			For<IWorkItemsComparer>().Use<TfsWorkItemsComparer>();
			For<IPluginMashupRepository>().Singleton().Use<TfsPluginMashupRepository>();
		}

		protected override void ConfigureCheckConnectionErrorResolver()
		{
			For<ICheckConnectionErrorResolver>().Use<TfsCheckConnectionErrorResolver>();
		}

		protected override void ConfigureSourceControlConnectionSettingsSource()
		{
			For<ISourceControlConnectionSettingsSource>().Use<TfsCurrentProfileToConnectionSettingsAdapter>();
		}

		protected override void ConfigureRevisionIdComparer()
		{
			For<IRevisionIdComparer>().HybridHttpOrThreadLocalScoped().Use<TfsRevisionIdComparer>();
		}

		protected override void ConfigureVersionControlSystem()
		{
			For<IVersionControlSystem>().Use<TfsVersionControlSystem>();
		}

		protected override void ConfigureRevisionStorage()
		{
			For<IRevisionStorageRepository>().Use<TfsRevisionStorageRepository>();
		}

		protected override void ConfigureUserMapper()
		{
			For<UserMapper>().Use<TfsUserMapper>();
		}
	}
}