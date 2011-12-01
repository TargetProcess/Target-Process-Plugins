//
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
//

using System.Collections.Generic;
using System.Linq;
using Tp.Core;
using Tp.Integration.Plugin.Common.Storage;
using Tp.Integration.Plugin.Common.Storage.Persisters;

namespace Tp.Integration.Testing.Common.Persisters
{
	public class ProfileStorageInMemoryPersister : IProfileStoragePersister
	{
		private readonly List<ProfileStorage> _profileStorage = new List<ProfileStorage>();

		public void Insert(params ProfileStorage[] profileStorages)
		{
			_profileStorage.AddRange(profileStorages);
		}

		public void Update(params ProfileStorage[] profileStorages)
		{
		}

		public void Delete(ProfileId profileId, TypeNameWithoutVersion key)
		{
			_profileStorage.RemoveAll(x => x.ProfileId == profileId.Value && x.ValueKey == key.Value);
		}

		public void Delete(ProfileId profileId, TypeNameWithoutVersion key, params StorageName[] storageNames)
		{
			_profileStorage.RemoveAll(x => storageNames.Select(s => s.Value).Contains(x.Name) && x.ProfileId == profileId.Value && x.ValueKey == key.Value);
		}

		public void Delete(params ProfileStorage[] profileStorages)
		{
			foreach (var profileStorage in profileStorages)
			{
				_profileStorage.Remove(profileStorage);
			}
		}

		public IEnumerable<ProfileStorage> FindBy(ProfileId profileId, TypeNameWithoutVersion key)
		{
			return FindBy(profileId).Where(x => x.ValueKey == key.Value);
		}

		public IEnumerable<ProfileStorage> FindBy(ProfileId profileId, TypeNameWithoutVersion key, params StorageName[] storageNames)
		{
			return FindBy(profileId).Where(x => x.ValueKey == key.Value && storageNames.Select(s => GetLowerOrNull(s.Value)).Contains(GetLowerOrNull(x.Name)));
		}

		public ProfileStorage FindBy(ProfileId profileId, TypeNameWithoutVersion key, object item)
		{
			return FindBy(profileId, key).Where(x => x.GetValue() == item).FirstOrDefault();
		}

		public ProfileStorage FindBy(StorageName storageName, ProfileId profileId, TypeNameWithoutVersion key, object item)
		{
			return FindBy(profileId, key).Where(x => x.GetValue() == item && storageName.Value == x.Name).FirstOrDefault();
		}

		public IEnumerable<ProfileStorage> FindBy(ProfileId profileId)
		{
			return _profileStorage.FindAll(x => x.ProfileId == profileId.Value);
		}

		public bool Contains(ProfileId profileId, TypeNameWithoutVersion key, object item)
		{
			return FindBy(profileId, key, item) != null;
		}

		public bool Contains(StorageName storageName, ProfileId profileId, TypeNameWithoutVersion key, object item)
		{
			return FindBy(storageName, profileId, key, item) != null;
		}

		private string GetLowerOrNull(string source)
		{
			return source != null ? source.ToLower() : null;
		}
	}
}
