using Tp.Integration.Messages.PluginLifecycle;

namespace Tp.MashupManager.CustomCommands.Args
{
    public class InstallPackageCommandArg : PackageCommandArg
    {
        public ulong CreationDate { get; set; }
        public MashupUserInfo CreatedBy { get; set; }
    }
}
