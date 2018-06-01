// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Plugin.Common.Domain;
using Tp.SourceControl;

namespace Tp.Git
{
    public class GitCurrentProfileToConnectionSettingsAdapter : CurrentProfileToConnectionSettingsAdapter<GitPluginProfile>, IGitConnectionSettings
    {
        public GitCurrentProfileToConnectionSettingsAdapter(IStorageRepository repository)
            : base(repository)
        {
        }

        public bool UseSsh => Profile.UseSsh;
        public string SshPrivateKey => Profile.SshPrivateKey;
        public string SshPublicKey => Profile.SshPublicKey;
    }
}
