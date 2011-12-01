// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Linq;
using NUnit.Framework;
using StructureMap;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Storage.Persisters;
using Tp.Testing.Common.NUnit;

namespace Tp.Integration.Plugin.Common.Tests.Common
{
	[TestFixture]
	public class ProfileStorageSqlPersisterSpecs : SqlPersisterSpecBase
	{
		private ProfileStorageSqlPersister _persister;
		private Profile _profile;

		protected override void OnInit()
		{
			ObjectFactory.GetInstance<IAccountCollection>().GetOrCreate("Account");
			_profile = ObjectFactory.GetInstance<IProfilePersister>().Add("Profile", true, "Account");
			_persister = ObjectFactory.GetInstance<ProfileStorageSqlPersister>();
		}

		[Test]
		public void ShouldStoreAndRetrieveRecord()
		{
			var profileStorage = new ProfileStorage {ProfileId = _profile.Id, ValueKey = "value key"};
			profileStorage.SetValue("my value");
			_persister.Insert(profileStorage);

			var retrieved = _persister.FindBy(_profile.Id).Single();

			retrieved.Should(Be.EqualTo(profileStorage).Using<ProfileStorage>(Compare));
		}

		[Test]
		public void ShouldStoreAndRetrieveNamedRecordForProfile()
		{
			var profileStorages = CreateProfileStorages();

			var retrieved = _persister.FindBy(_profile.Id).ToArray();
			retrieved.Should(Be.EquivalentTo(profileStorages).Using<ProfileStorage>(Compare));
		}

		[Test]
		public void ShouldStoreAndRetrieveNamedRecordForProfileByValueKey()
		{
			var profileStorages = CreateProfileStorages();

			var profileStorage = profileStorages[1];
			var retrieved = _persister.FindBy(_profile.Id, profileStorage.ValueKey).Single();

			retrieved.Should(Be.EqualTo(profileStorage).Using<ProfileStorage>(Compare));
		}

		[Test]
		public void ShouldStoreAndRetrieveNamedRecordForProfileByName()
		{
			var profileStorages = CreateProfileStorages();

			var profileStorage = profileStorages[1];
			var retrieved = _persister.FindBy(_profile.Id, profileStorage.ValueKey, profileStorage.Name).Single();
			retrieved.Should(Be.EqualTo(profileStorage).Using<ProfileStorage>(Compare));
		}

		[Test]
		public void ShouldStoreAndRetrieveNamedRecordForProfileByNameAndValue()
		{
			var profileStorages = CreateProfileStorages();

			var profileStorage = profileStorages[1];
			var retrieved = _persister.FindBy(profileStorage.Name, _profile.Id, profileStorage.ValueKey, profileStorage.GetValue());
			retrieved.Should(Be.EqualTo(profileStorage).Using<ProfileStorage>(Compare));

			_persister.FindBy(profileStorage.Name, _profile.Id, profileStorage.ValueKey, profileStorage.GetValue() + "123").Should(
				Be.Null);
		}

		[Test]
		public void ShouldDetectContainmentByNameAndValue()
		{
			var profileStorages = CreateProfileStorages();

			var profileStorage = profileStorages[1];
			_persister.Contains(profileStorage.Name, _profile.Id, profileStorage.ValueKey, profileStorage.GetValue()).Should(Be.True);

			_persister.Contains(profileStorage.Name, _profile.Id, profileStorages[0].ValueKey, profileStorage.GetValue()).Should(
				Be.False);
		}

		[Test]
		public void ShouldDeleteByName()
		{
			var profileStorages = CreateProfileStorages();

			var profileStorage = profileStorages[1];
			_persister.Delete(_profile.Id, profileStorage.ValueKey, profileStorage.Name);
			_persister.Contains(profileStorage.Name, _profile.Id, profileStorage.ValueKey, profileStorage.GetValue()).Should(
				Be.False);
		}

		private ProfileStorage[] CreateProfileStorages()
		{
			var profileStorage = new ProfileStorage {ProfileId = _profile.Id, ValueKey = "value key", Name = "Name"};
			profileStorage.SetValue("my value");
			var profileStorage1 = new ProfileStorage {ProfileId = _profile.Id, ValueKey = "value key1", Name = "Name"};
			profileStorage1.SetValue("my value1");

			var profileStorage2 = new ProfileStorage {ProfileId = _profile.Id, ValueKey = "value key2", Name = "Name2"};
			profileStorage2.SetValue("my value2");

			var profileStorages = new[] {profileStorage, profileStorage1, profileStorage2};
			_persister.Insert(profileStorages);
			return profileStorages;
		}

		private static int Compare(ProfileStorage actual, ProfileStorage expected)
		{
			if (actual.Id != expected.Id)
				return -1;

			if (actual.ProfileId != expected.ProfileId)
				return -1;

			if (actual.Name != expected.Name)
				return -1;

			if (!actual.GetValue().Equals(expected.GetValue()))
				return -1;

			return 0;
		}
	}
}