//
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
//

using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using Tp.Core;

namespace Tp.Integration.Plugin.Common.Storage.Persisters
{
    internal class ProfileStorageSqlPersister : IProfileStoragePersister
    {
        private readonly IDatabaseConfiguration _configuration;

        public ProfileStorageSqlPersister(IDatabaseConfiguration configuration)
        {
            _configuration = configuration;
        }

        private PluginDatabaseModelDataContext CreateContext()
        {
            return new PluginDatabaseModelDataContext(_configuration.ConnectionString, _configuration.CommandTimeoutSeconds);
        }

        private static void Flush(DataContext context)
        {
            var changeSet = context.GetChangeSet();
            changeSet.Inserts.OfType<ISavingChangesEventHandler>().ToList().ForEach(x => x.OnInsert());
            changeSet.Updates.OfType<ISavingChangesEventHandler>().ToList().ForEach(x => x.OnUpdate());
            changeSet.Deletes.OfType<ISavingChangesEventHandler>().ToList().ForEach(x => x.OnDelete());

            context.SubmitChanges();
        }

        #region IProfileStoragePersister Members

        public void Insert(params ProfileStorage[] profileStorages)
        {
            using (var context = CreateContext())
            {
                context.ProfileStorages.InsertAllOnSubmit(profileStorages);
                Flush(context);
            }
        }

        public void Update(params ProfileStorage[] profileStorages)
        {
            using (var context = CreateContext())
            {
                foreach (var profileStorage in profileStorages)
                {
                    var updatedProfileStorage = new ProfileStorage(new TypeNameWithoutVersion(profileStorage.ValueKey))
                    {
                        Id = profileStorage.Id,
                        ProfileId = profileStorage.ProfileId,
                        Name = profileStorage.Name
                    };
                    context.ProfileStorages.Attach(updatedProfileStorage);
                    updatedProfileStorage.SetValue(profileStorage.GetValue());
                }

                Flush(context);
            }
        }

        public IEnumerable<ProfileStorage> FindBy(ProfileId profileId)
        {
            using (var context = CreateContext())
            {
                return (from profileStorage in context.ProfileStorages
                    where profileStorage.ProfileId == profileId.Value
                    select profileStorage).ToArray();
            }
        }

        public void Delete(ProfileId profileId, TypeNameWithoutVersion key)
        {
            using (var context = CreateContext())
            {
                const string cmd =
                    "delete dbo.ProfileStorage where dbo.ProfileStorage.ProfileId = {0} AND dbo.ProfileStorage.ValueKey = {1}";
                context.ExecuteCommand(cmd, profileId.Value, key.Value);
            }
        }

        public void Delete(ProfileId profileId, TypeNameWithoutVersion key, params StorageName[] storageNames)
        {
            using (var context = CreateContext())
            {
                var inClausePlaceholders = string.Join(",", storageNames.Select((x, i) => "{{{0}}}".Fmt(i + 2)));
                var parameters = new object[] { profileId.Value, key.Value }.Concat(storageNames.Select(x => x.Value));

                var cmd =
                    "delete dbo.ProfileStorage where dbo.ProfileStorage.ProfileId = {0} AND dbo.ProfileStorage.ValueKey = {1} AND dbo.ProfileStorage.Name IN ("
                    + inClausePlaceholders + ")";
                context.ExecuteCommand(cmd, parameters.ToArray());
            }
        }

        public IEnumerable<ProfileStorage> FindBy(ProfileId profileId, TypeNameWithoutVersion key)
        {
            using (var context = CreateContext())
            {
                int id = profileId.Value;
                string valueKey = key.Value;

                return (from profileStorage in context.ProfileStorages
                    where profileStorage.ProfileId == id && profileStorage.ValueKey == valueKey
                    select profileStorage).ToArray();
            }
        }

        public IEnumerable<ProfileStorage> FindBy(ProfileId profileId, TypeNameWithoutVersion key, params StorageName[] storageNames)
        {
            using (var context = CreateContext())
            {
                int id = profileId.Value;
                string valueKey = key.Value;
                var starageNameValues = storageNames.Select(s => s.Value);

                return (from profileStorage in context.ProfileStorages
                    where
                    profileStorage.ProfileId == id && profileStorage.ValueKey == valueKey && starageNameValues.Contains(profileStorage.Name)
                    select profileStorage).ToArray();
            }
        }

        public ProfileStorage FindBy(StorageName storageName, ProfileId profileId, TypeNameWithoutVersion key, object item)
        {
            var itemsToSearch = FindBy(profileId, key, storageName);
            return itemsToSearch.FirstOrDefault(profileStorage => profileStorage.GetValue().Equals(item));
        }

        public bool Contains(ProfileId profileId, TypeNameWithoutVersion key, object item)
        {
            return FindBy(profileId, key, item) != null;
        }

        public bool Contains(StorageName storageName, ProfileId profileId, TypeNameWithoutVersion key, object item)
        {
            return FindBy(storageName, profileId, key, item) != null;
        }

        public void Delete(params ProfileStorage[] profileStorages)
        {
            using (var context = CreateContext())
            {
                context.ProfileStorages.AttachAll(profileStorages);
                context.ProfileStorages.DeleteAllOnSubmit(profileStorages);
                Flush(context);
            }
        }

        public ProfileStorage FindBy(ProfileId profileId, TypeNameWithoutVersion key, object item)
        {
            var itemsToSearch = FindBy(profileId, key);
            return itemsToSearch.FirstOrDefault(profileStorage => profileStorage.GetValue().Equals(item));
        }

        #endregion IProfileStoragePersister Members
    }
}
