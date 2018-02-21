// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using Tp.Integration.Messages;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Integration.Plugin.Common.Events
{
    class ProfileChanged : EventArgs
    {
        private readonly IProfileReadonly _profile;
        private readonly AccountName _accountName;

        public ProfileChanged(IProfileReadonly profile, AccountName accountName)
        {
            _profile = profile;
            _accountName = accountName;
        }

        public IProfileReadonly Profile
        {
            get { return _profile; }
        }

        public AccountName AccountName
        {
            get { return _accountName; }
        }
    }
}
