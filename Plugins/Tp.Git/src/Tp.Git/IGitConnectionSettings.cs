using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tp.SourceControl.Settings;

namespace Tp.Git
{
    public interface IGitConnectionSettings : ISourceControlConnectionSettingsSource
    {
        bool UseSsh { get; }
        string SshPrivateKey { get; }
        string SshPublicKey { get; }
    }
}
