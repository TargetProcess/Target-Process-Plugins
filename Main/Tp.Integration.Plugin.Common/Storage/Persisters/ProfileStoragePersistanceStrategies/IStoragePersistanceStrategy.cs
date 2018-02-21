// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;

namespace Tp.Integration.Plugin.Common.Storage.Persisters.ProfileStoragePersistanceStrategies
{
    internal interface IStoragePersistanceStrategy
    {
        void Insert(params ProfileStorage[] profileStorages);
        void Update(ProfileStorage itemToUpdate);
        void Delete(params ProfileStorage[] itemsToRemove);
        void Clear();

        IEnumerable<ProfileStorage> GetAllStorages();
        ProfileStorage FindBy<T>(T item);
        bool Contains<T>(T item);
    }
}
