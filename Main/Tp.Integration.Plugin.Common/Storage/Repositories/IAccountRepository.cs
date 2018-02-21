// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using Tp.Integration.Messages;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Integration.Plugin.Common.Storage.Repositories
{
    internal interface IAccountRepository
    {
        IList<AccountDomainObject> GetAll();
        AccountDomainObject Add(AccountName accountName);
        AccountDomainObject GetBy(AccountName accountName);
        void Remove(AccountName accountName);
    }
}
