using Tp.Integration.Plugin.Common.Domain;
using Tp.SourceControl;

namespace Tp.Perforce
{
    public class P4CurrentProfileToConnectionSettingsAdapter : CurrentProfileToConnectionSettingsAdapter<P4PluginProfile>
    {
        public P4CurrentProfileToConnectionSettingsAdapter(IStorageRepository repository)
            : base(repository)
        {
        }

        public string Workspace
        {
            get => Profile.Workspace;
            set => Profile.Workspace = value;
        }
    } 
}
