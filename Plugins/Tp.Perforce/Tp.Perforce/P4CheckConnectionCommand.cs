// 
// Copyright (c) 2005-2018 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Linq;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Validation;
using Tp.SourceControl.Commands;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.Perforce
{
    public class P4CheckConnectionCommand : VcsCheckConnectionCommand<P4PluginProfile>
    {
        public P4CheckConnectionCommand(IProfileCollection profileCollection) : base(profileCollection)
        {
        }

        protected override void OnCheckConnection(PluginProfileErrorCollection errors, P4PluginProfile settings)
        {
            settings.ValidateUri(errors);

            if (!errors.Any())
            {
                base.OnCheckConnection(errors, settings);
            }
        }

        protected override void CheckStartRevision(P4PluginProfile settings,
            IVersionControlSystem versionControlSystem,
            PluginProfileErrorCollection errors)
        {
            if (settings.ValidateStartRevision(errors))
            {
                var startRevision = int.Parse(settings.StartRevision) > 1 ? settings.StartRevision : "1";
                versionControlSystem.CheckRevision(startRevision, errors);
            }
        }
    }
}
