using Tp.Integration.Plugin.Common;
using Tp.Perforce.RevisionStorage;
using Tp.Perforce.VersionControlSystem;
using Tp.Perforce.Workflow;
using Tp.SourceControl.Commands;
using Tp.SourceControl.RevisionStorage;
using Tp.SourceControl.Settings;
using Tp.SourceControl.StructureMap;
using Tp.SourceControl.VersionControlSystem;
using Tp.SourceControl.Workflow.Workflow;

namespace Tp.Perforce.StructureMap
{
    public class P4Registry : SourceControlRegistry
    {
        public P4Registry()
        {
            For<IExcludedAssemblyNamesSource>().Singleton().Use<P4PluginExcludedAssemblies>();
        }

        protected override void ConfigureCheckConnectionErrorResolver()
        {
            For<ICheckConnectionErrorResolver>().Use<P4CheckConnectionErrorResolver>();
        }

        protected override void ConfigureSourceControlConnectionSettingsSource()
        {
            For<ISourceControlConnectionSettingsSource>().Use<P4CurrentProfileToConnectionSettingsAdapter>();
        }

        protected override void ConfigureRevisionIdComparer()
        {
            For<IRevisionIdComparer>().HybridHttpOrThreadLocalScoped().Use<P4RevisionIdComparer>();
        }

        protected override void ConfigureVersionControlSystem()
        {
            For<IVersionControlSystem>().Use<P4VersionControlSystem>();
        }

        protected override void ConfigureRevisionStorage()
        {
            For<IRevisionStorageRepository>().Use<P4RevisionStorageRepository>();
        }

        protected override void ConfigureUserMapper()
        {
            For<UserMapper>().Use<P4UserMapper>();
        }
    }
}
