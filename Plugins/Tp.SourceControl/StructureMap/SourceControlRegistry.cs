// 
// Copyright (c) 2005-2017 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Net;
using StructureMap.Configuration.DSL;
using Tp.Integration.Plugin.Common;
using Tp.Plugins.Toolkit.Repositories;
using Tp.SourceControl.Comments.DSL;
using Tp.SourceControl.Diff;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.SourceControl.StructureMap
{
    public abstract class SourceControlRegistry : Registry
    {
        protected SourceControlRegistry()
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            For<IActionFactory>().HybridHttpOrThreadLocalScoped().Use<ActionFactory>();
            For<IDiffProcessor>().HybridHttpOrThreadLocalScoped().Use<DiffProcessor>();
            For<IVersionControlSystemFactory>().HybridHttpOrThreadLocalScoped().Use<VersionControlSystemFactory>();
            For<IRepository<TpUserData>>().HybridHttpOrThreadLocalScoped().Use<DataRepository<TpUserData>>();
            For<IWantToRunBeforeBusStart>().HybridHttpOrThreadLocalScoped().Use<UserStorageMigrator>();

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
