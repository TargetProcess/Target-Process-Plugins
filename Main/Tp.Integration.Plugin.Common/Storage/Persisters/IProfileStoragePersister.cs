// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using Tp.Core;

namespace Tp.Integration.Plugin.Common.Storage.Persisters
{
	public interface IProfileStoragePersister
	{
		void Insert(params ProfileStorage[] profileStorages);

		void Update(params ProfileStorage[] profileStorages);

		void Delete(ProfileId profileId, TypeNameWithoutVersion key);
		void Delete(ProfileId profileId, TypeNameWithoutVersion key, params StorageName[] storageNames);

		void Delete(params ProfileStorage[] profileStorages);

		IEnumerable<ProfileStorage> FindBy(ProfileId profileId, TypeNameWithoutVersion key);
		IEnumerable<ProfileStorage> FindBy(ProfileId profileId, TypeNameWithoutVersion key, params StorageName[] storageNames);

		ProfileStorage FindBy(ProfileId profileId, TypeNameWithoutVersion key, object item);
		ProfileStorage FindBy(StorageName storageName, ProfileId profileId, TypeNameWithoutVersion key, object item);

		IEnumerable<ProfileStorage> FindBy(ProfileId profileId);

		bool Contains(ProfileId profileId, TypeNameWithoutVersion key, object item);
		bool Contains(StorageName storageName, ProfileId profileId, TypeNameWithoutVersion key, object item);
	}
}