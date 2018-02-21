// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using Tp.Integration.Messages;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Integration.Plugin.Common.Storage.Repositories
{
    internal interface IProfileRepository
    {
        ProfileDomainObject Add(ProfileCreationArgs profileCreationArgs, AccountName accountName);
        void Update(ProfileDomainObject profile, AccountName accountName);
        void Delete(ProfileName profileName, AccountName accountName);
        IEnumerable<ProfileDomainObject> GetAll(AccountName accountName);
    }
}
