using System;
using System.Linq;
using Tp.Integration.Plugin.Common.Validation;
using Tp.SourceControl.Commands;
using Tp.SourceControl.VersionControlSystem;
using MercurialSDK = Mercurial;

namespace Tp.Mercurial
{
    public class MercurialCheckConnectionCommand : VcsCheckConnectionCommand<MercurialPluginProfile>
    {
        protected override void CheckStartRevision(
            MercurialPluginProfile settings,
            IVersionControlSystem versionControlSystem,
            PluginProfileErrorCollection errors)
        {
            settings.ValidateStartRevision(errors);
        }

        protected override void OnCheckConnection(PluginProfileErrorCollection errors, MercurialPluginProfile settings)
        {
            settings.ValidateUri(errors);

            if (!errors.Any())
            {
                var identifyCommand = new MercurialSDK.IdentifyCommand().WithPath(settings.Uri).WithTimeout(20);
                MercurialSDK.NonPersistentClient.Execute(identifyCommand);
                if (identifyCommand.RawExitCode != 0)
                {
                    throw new Exception("Can't establish connection");
                }
            }
        }
    }
}
