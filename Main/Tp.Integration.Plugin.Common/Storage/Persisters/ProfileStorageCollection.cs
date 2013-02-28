// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;

namespace Tp.Integration.Plugin.Common.Storage.Persisters
{
	using Tp.Core;

	internal class ProfileStorageCollection
	{
		private readonly ProfileId _profileId;
		private readonly IProfileStoragePersister _persister;

		public ProfileStorageCollection(ProfileId profileId, IProfileStoragePersister persister)
		{
			_profileId = profileId;
			_persister = persister;
		}

		public object this[Type type]
		{
			get
			{
				var key = ProfileStorage.Key(type);
				var profileStorage = _persister.FindBy(_profileId, key).FirstOrDefault();
				return profileStorage != null ? profileStorage.GetValue() : null;
			}
			set
			{
				if (value == null)
					return;

				var key = ProfileStorage.Key(value.GetType());
				var valueStorage = _persister.FindBy(_profileId, key).FirstOrDefault();

				if (valueStorage != null)
				{
					valueStorage.SetValue(value);
					_persister.Update(valueStorage);
				}
				else
				{
					valueStorage = new ProfileStorage(new TypeNameWithoutVersion(value.GetType())) {ProfileId = _profileId.Value};
					valueStorage.SetValue(value);
					_persister.Insert(valueStorage);
				}
			}
		}
	}
}