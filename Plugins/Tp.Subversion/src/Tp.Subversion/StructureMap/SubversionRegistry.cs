// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Plugin.Common;
using Tp.SourceControl.Commands;
using Tp.SourceControl.Settings;
using Tp.SourceControl.StructureMap;
using Tp.SourceControl.VersionControlSystem;
using Tp.Subversion.Subversion;

namespace Tp.Subversion.StructureMap
{
	public class SubversionRegistry : SourceControlRegistry
	{
		public SubversionRegistry()
		{
			For<IExcludedAssemblyNamesSource>().Singleton().Use<SvnPluginExcludedAssemblies>();
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
	}
}