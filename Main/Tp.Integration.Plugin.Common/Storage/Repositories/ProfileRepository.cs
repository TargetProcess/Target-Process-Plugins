// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Linq;
using Tp.Integration.Messages;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Events.Aggregator;
using Tp.Integration.Plugin.Common.Storage.Persisters;

namespace Tp.Integration.Plugin.Common.Storage.Repositories
{
    internal class ProfileRepository : IProfileRepository, IProfileFactory
    {
        private readonly IProfilePersister _profilePersister;
        private readonly IProfileStoragePersister _profileStoragePersister;
        private readonly IPluginMetadata _pluginMetadata;
        private readonly IEventAggregator _eventAggregator;

        public ProfileRepository(IProfilePersister profilePersister, IProfileStoragePersister profileStoragePersister,
            IPluginMetadata pluginMetadata, IEventAggregator eventAggregator)
        {
            _profilePersister = profilePersister;
            _profileStoragePersister = profileStoragePersister;
            _pluginMetadata = pluginMetadata;
            _eventAggregator = eventAggregator;
        }

        public ProfileDomainObject Add(ProfileCreationArgs profileCreationArgs, AccountName accountName)
        {
            var profileCreated = _profilePersister.Add(profileCreationArgs.ProfileName, !_pluginMetadata.IsNewProfileInitializable,
                accountName);
            SaveSettings(profileCreationArgs.Settings, profileCreated);
            return Create(profileCreated, accountName);
        }

        public void Update(ProfileDomainObject profile, AccountName accountName)
        {
            var profileUpdated = _profilePersister.Update(profile.Name, profile.Initialized, accountName);
            SaveSettings(profile.Settings, profileUpdated);
        }

        public void Delete(ProfileName profileName, AccountName accountName)
        {
            _profilePersister.Delete(profileName, accountName);
        }

        public IEnumerable<ProfileDomainObject> GetAll(AccountName accountName)
        {
            return _profilePersister.GetAll(accountName).Select(p => Create(p, accountName));
        }

        private void SaveSettings(object settings, Profile profileCreated)
        {
            var storage = new ProfileStorageCollection(new ProfileId(profileCreated.Id), _profileStoragePersister);
            storage[settings.GetType()] = settings;
        }

        public ProfileDomainObject Create(Profile profile, AccountName accountName)
        {
            return new ProfileDomainObject(profile.Name, accountName, profile.Initialized, _pluginMetadata.ProfileType)
            {
                EventAggregator = _eventAggregator,
                ProfileRepository = this,
                StorageRepository = new ProfileStorageRepository(new ProfileId(profile.Id), _profileStoragePersister, _pluginMetadata)
            };
        }
    }
}
