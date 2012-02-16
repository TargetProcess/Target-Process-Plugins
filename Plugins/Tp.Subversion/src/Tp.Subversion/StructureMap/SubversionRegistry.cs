// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Messages.SerializationPatches;
using Tp.Integration.Plugin.Common;
using Tp.SourceControl.Commands;
using Tp.SourceControl.RevisionStorage;
using Tp.SourceControl.Settings;
using Tp.SourceControl.StructureMap;
using Tp.SourceControl.VersionControlSystem;
using Tp.SourceControl.Workflow.Workflow;
using Tp.Subversion.RevisionStorage;
using Tp.Subversion.SerializationPatches.Xml;
using Tp.Subversion.Subversion;
using Tp.Subversion.Workflow;

namespace Tp.Subversion.StructureMap
{
	public class SubversionRegistry : SourceControlRegistry
	{
		public SubversionRegistry()
		{
			For<IExcludedAssemblyNamesSource>().HybridHttpOrThreadLocalScoped().Use<SvnPluginExcludedAssemblies>();
			For<IPatchCollection>().Use<SvnPatchCollection>();
		}

		protected override void ConfigureCheckConnectionErrorResolver()
		{
			For<ICheckConnectionErrorResolver>().Use<SubversionErrorResolver>();
		}

		protected override void ConfigureSourceControlConnectionSettingsSource()
		{
			For<ISourceControlConnectionSettingsSource>().Use<CurrentProfileToSvnConnectionSettingsAdapter>();
		}

		protected override void ConfigureRevisionIdComparer()
		{
			For<IRevisionIdComparer>().Use<SubversionRevisionIdComparer>();
		}

		protected override void ConfigureVersionControlSystem()
		{
			For<IVersionControlSystem>().Use<Subversion.Subversion>();
		}

		protected override void ConfigureRevisionStorage()
		{
			For<IRevisionStorageRepository>().HybridHttpOrThreadLocalScoped().Use<SubversionRevisionStorageRepository>();
		}

		protected override void ConfigureUserMapper()
		{
			For<UserMapper>().Use<SubversionUserMapper>();
		}
	}
}