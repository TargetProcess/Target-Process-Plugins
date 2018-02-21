// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Plugin.Common.Validation;
using Tp.SourceControl.Commands;
using Tp.SourceControl.VersionControlSystem;
using System.Linq;

namespace Tp.Git
{
    public class GitCheckConnectionCommand : VcsCheckConnectionCommand<GitPluginProfile>
    {
        private readonly IConnectionChecker _connectionChecker;

        public GitCheckConnectionCommand(IConnectionChecker connectionChecker)
        {
            _connectionChecker = connectionChecker;
        }

        protected override void CheckStartRevision(GitPluginProfile settings, IVersionControlSystem versionControlSystem,
            PluginProfileErrorCollection errors)
        {
            settings.ValidateStartRevision(errors);
        }

        protected override void OnCheckConnection(PluginProfileErrorCollection errors, GitPluginProfile settings)
        {
            settings.ValidateUri(errors);
            if (errors.Any())
            {
                return;
            }

            _connectionChecker.Check(settings);
        }   
    }
}
