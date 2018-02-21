// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Messages;

namespace Tp.Integration.Plugin.Common.Domain
{
    public class ProfileCreationArgs
    {
        private readonly ProfileName _profileName;
        private readonly object _settings;

        public ProfileCreationArgs(ProfileName profileName, object settings)
        {
            _profileName = profileName;
            _settings = settings;
        }

        public object Settings
        {
            get { return _settings; }
        }

        public ProfileName ProfileName
        {
            get { return _profileName; }
        }
    }
}
