// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Messages;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Storage.Persisters;

namespace Tp.Integration.Plugin.Common.Storage.Repositories
{
    internal interface IProfileFactory
    {
        ProfileDomainObject Create(Profile profile, AccountName accountName);
    }
}
