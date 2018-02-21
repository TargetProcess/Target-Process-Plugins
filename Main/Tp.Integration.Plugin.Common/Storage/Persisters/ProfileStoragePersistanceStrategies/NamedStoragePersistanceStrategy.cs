// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using Tp.Core;

namespace Tp.Integration.Plugin.Common.Storage.Persisters.ProfileStoragePersistanceStrategies
{
    internal class NamedStoragePersistanceStrategy : IStoragePersistanceStrategy
    {
        private readonly IProfileStoragePersister _persister;
        private readonly ProfileId _profileId;
        private readonly TypeNameWithoutVersion _key;
        private readonly StorageName[] _storageNames;

        public NamedStoragePersistanceStrategy(IProfileStoragePersister persister, ProfileId profileId, TypeNameWithoutVersion key,
            params StorageName[] storageNames)
        {
            _persister = persister;
            _profileId = profileId;
            _key = key;
            _storageNames = storageNames;
        }

        public void Update(ProfileStorage itemToUpdate)
        {
            _persister.Update(itemToUpdate);
        }

        public IEnumerable<ProfileStorage> GetAllStorages()
        {
            return _persister.FindBy(_profileId, _key, _storageNames);
        }

        public void Delete(params ProfileStorage[] itemsToRemove)
        {
            _persister.Delete(itemsToRemove);
        }

        public void Clear()
        {
            _persister.Delete(_profileId, _key, _storageNames);
        }

        public void Delete(ProfileId profileId, TypeNameWithoutVersion typeNameWithoutVersion)
        {
            _persister.Delete(profileId, typeNameWithoutVersion, _storageNames);
        }

        public bool Contains<T>(T item)
        {
            return _persister.Contains(_profileId, _key, item);
        }

        public ProfileStorage FindBy<T>(T item)
        {
            return _persister.FindBy(_profileId, _key, item);
        }

        public void Insert(params ProfileStorage[] profileStorages)
        {
            foreach (var storageName in _storageNames)
            {
                foreach (var profileStorage in profileStorages)
                {
                    profileStorage.Name = storageName.Value;
                }

                _persister.Insert(profileStorages);
            }
        }
    }
}
