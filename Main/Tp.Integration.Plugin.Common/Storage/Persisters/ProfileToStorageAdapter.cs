//
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Tp.Core;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Storage.Persisters.ProfileStoragePersistanceStrategies;

namespace Tp.Integration.Plugin.Common.Storage.Persisters
{
    internal class ProfileToStorageAdapter<T> : IStorage<T>
    {
        private readonly TypeNameWithoutVersion _key;
        private readonly ProfileId _profileId;
        private readonly IStoragePersistanceStrategy _persistanceStrategy;

        public ProfileToStorageAdapter(ProfileId profileId, IProfileStoragePersister persister)
            : this(profileId, persister, typeof(T), null)
        {
        }

        public ProfileToStorageAdapter(ProfileId profileId, IProfileStoragePersister persister, params StorageName[] storageNames)
            : this(profileId, persister, typeof(T), storageNames)
        {
        }

        public ProfileToStorageAdapter(ProfileId profileId, IProfileStoragePersister persister, Type keyType,
            params StorageName[] storageNames)
        {
            _profileId = profileId;
            _key = ProfileStorage.Key(keyType);

            if (storageNames == null)
            {
                _persistanceStrategy = new NotNamedStoragePersistanceStrategy(persister, profileId, _key);
            }
            else
            {
                _persistanceStrategy = new NamedStoragePersistanceStrategy(persister, profileId, _key, storageNames);
            }
        }

        #region IStorage<T> Members

        public void ReplaceWith(params T[] value)
        {
            _persistanceStrategy.Clear();
            AddRange(value);
        }

        public void Update(T value, Predicate<T> condition)
        {
            var storages = _persistanceStrategy.GetAllStorages();
            var itemToUpdate = storages.FirstOrDefault(x => condition(x.GetValue<T>()));
            if (itemToUpdate == null) return;

            itemToUpdate.SetValue(value);
            _persistanceStrategy.Update(itemToUpdate);
        }

        public bool IsNull
        {
            get { return false; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _persistanceStrategy.GetAllStorages().Select(x => x.GetValue<T>()).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Expression Expression
        {
            get { return _persistanceStrategy.GetAllStorages().Select(x => x.GetValue<T>()).AsQueryable().Expression; }
        }

        public Type ElementType
        {
            get { return typeof(T); }
        }

        public IQueryProvider Provider
        {
            get { return _persistanceStrategy.GetAllStorages().AsQueryable().Provider; }
        }

        public void Add(T item)
        {
            AddRange(new[] { item });
        }

        public void AddRange(IEnumerable<T> items)
        {
            var profileStorages = items.Select(ConvertToProfileStorage).ToArray();
            _persistanceStrategy.Insert(profileStorages);
        }

        public void Remove(Predicate<T> condition)
        {
            var storages = _persistanceStrategy.GetAllStorages();
            var itemsToRemove = storages.Where(x => condition(x.GetValue<T>())).ToArray();
            _persistanceStrategy.Delete(itemsToRemove);
        }

        public void Clear()
        {
            _persistanceStrategy.Clear();
        }

        public bool Contains(T item)
        {
            return _persistanceStrategy.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            var values2Copy = _persistanceStrategy.GetAllStorages().Skip(arrayIndex).Select(x => x.GetValue<T>()).ToArray();
            values2Copy.CopyTo(array, 0);
        }

        public bool Remove(T item)
        {
            var item2Remove = FindProfileStorageByValue(item);
            if (item2Remove != null)
            {
                _persistanceStrategy.Delete(item2Remove);
                return true;
            }
            return false;
        }

        public int Count
        {
            get { return _persistanceStrategy.GetAllStorages().Count(); }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion IStorage<T> Members

        private ProfileStorage ConvertToProfileStorage(T value)
        {
            var result = new ProfileStorage(_key) { ProfileId = _profileId.Value };
            result.SetValue(value);
            return result;
        }

        private ProfileStorage FindProfileStorageByValue(T item)
        {
            return _persistanceStrategy.FindBy(item);
        }
    }
}
