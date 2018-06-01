// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Git.RevisionStorage;
using Tp.Git.VersionControlSystem;
using Tp.Git.Workflow;
using Tp.Integration.Plugin.Common;
using Tp.SourceControl.Commands;
using Tp.SourceControl.RevisionStorage;
using Tp.SourceControl.Settings;
using Tp.SourceControl.StructureMap;
using Tp.SourceControl.VersionControlSystem;
using Tp.SourceControl.Workflow.Workflow;

namespace Tp.Git.StructureMap
{
    public class GitRegistry : SourceControlRegistry
    {
        public GitRegistry()
        {
            For<IExcludedAssemblyNamesSource>().Singleton().Use<GitPluginExcludedAssemblies>();

            if (PluginSettings.LoadInt("UseLibgit", 0) == 0)
            {
                For<IGitClientFactory>().Singleton().Use<NGitClientFactory>();
                For<IConnectionChecker>().Singleton().Use<NGitConnectionChecker>();
                For<ICheckConnectionErrorResolver>().Use<NGitCheckConnectionErrorResolver>();

                NGitMockSystemReader.Register();
            }
            else
            {
                For<IGitClientFactory>().Singleton().Use<LibgitClientFactory>();
                For<IConnectionChecker>().Singleton().Use<LibgitConnectionChecker>();
                For<ICheckConnectionErrorResolver>().Use<LibgitCheckConnectionErrorResolver>();
            }
        }

        protected override void ConfigureCheckConnectionErrorResolver()
        {
        }

        protected override void ConfigureSourceControlConnectionSettingsSource()
        {
            For<ISourceControlConnectionSettingsSource>().Use<GitCurrentProfileToConnectionSettingsAdapter>();
            For<IGitConnectionSettings>().Use<GitCurrentProfileToConnectionSettingsAdapter>();
        }

        protected override void ConfigureRevisionIdComparer()
        {
            For<IRevisionIdComparer>().HybridHttpOrThreadLocalScoped().Use<GitRevisionIdComparer>();
        }

        protected override void ConfigureVersionControlSystem()
        {
            For<IVersionControlSystem>().Use<GitVersionControlSystem>();
        }

        protected override void ConfigureRevisionStorage()
        {
            For<IRevisionStorageRepository>().Use<GitRevisionStorageRepository>();
        }

        protected override void ConfigureUserMapper()
        {
            For<UserMapper>().Use<GitUserMapper>();
        }
    }
}
