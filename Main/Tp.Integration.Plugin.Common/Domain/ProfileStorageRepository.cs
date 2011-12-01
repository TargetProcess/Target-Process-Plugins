// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using Tp.Integration.Plugin.Common.Storage;
using Tp.Integration.Plugin.Common.Storage.Persisters;

namespace Tp.Integration.Plugin.Common.Domain
{
	class ProfileStorageRepository : IStorageRepository
	{
		private readonly ProfileId _profileId;
		private readonly IProfileStoragePersister _profileStoragePersister;
		private readonly IPluginMetadata _pluginMetadata;

		public ProfileStorageRepository(ProfileId profileId, IProfileStoragePersister profileStoragePersister, IPluginMetadata pluginMetadata)
		{
			_profileId = profileId;
			_profileStoragePersister = profileStoragePersister;
			_pluginMetadata = pluginMetadata;
		}

		public bool IsNull
		{
			get { return false; }
		}

		public IStorage<T> Get<T>()
		{
			return new ProfileToStorageAdapter<T>(_profileId, _profileStoragePersister);
		}

		public IStorage<T> Get<T>(params StorageName[] storageNames)
		{
			return new ProfileToStorageAdapter<T>(_profileId, _profileStoragePersister, storageNames);
		}

		public T GetProfile<T>()
		{
			return (T) GetProfile();
		}

		private object GetProfile()
		{
			var profileType = _pluginMetadata.ProfileType;
			var profile = GetValue(profileType);
			return profile ?? CreateDefaultProfile(profileType);
		}

		private object GetValue(Type valueType)
		{
			return ProfileStorage[valueType];
		}

		private object CreateDefaultProfile(Type profileType)
		{
			return profileType != null ? Activator.CreateInstance(profileType) : new object();
		}

		private ProfileStorageCollection ProfileStorage
		{
			get { return new ProfileStorageCollection(_profileId, _profileStoragePersister); }
		}
	}
}