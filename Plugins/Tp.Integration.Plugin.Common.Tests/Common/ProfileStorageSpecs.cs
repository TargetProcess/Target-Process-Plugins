// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using NBehave.Narrator.Framework;
using NServiceBus;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap;
using Tp.Integration.Messages;
using Tp.Integration.Messages.ServiceBus;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Storage.Persisters;
using Tp.Testing.Common.NBehave;
using Tp.Testing.Common.NUnit;

namespace Tp.Integration.Plugin.Common.Tests.Common
{
	[TestFixture]
	[ActionSteps]
    [Category("PartPlugins1")]
	public class ProfileStorageSpecs
	{
		public class ProfileStorageContext
		{
			public string CurrentStorageName { get; set; }
			public AccountName AccountName { get; set; }
			public ProfileName ProfileName { get; set; }
		}

		[SetUp]
		public void Init()
		{
			ObjectFactory.Initialize(x =>
			                         	{
			                         		x.AddRegistry<PluginStorageWithInMemoryPersisterMockRegistry>();
			                         		x.For<ProfileStorageContext>().Singleton().Use<ProfileStorageContext>();
			                         	});
		}

		[Test]
		public void RetrieveDataForSpecificAccountAndProfile()
		{
			@"Given account 'Account1' has profile 'Profile1' created
				And account 'Account2' has profile 'Profile2' created
			When store string value '11' to profile 'Profile1' for account 'Account1'
				And store string value '22' to profile 'Profile2' for account 'Account2'
			Then storage for profile 'Profile1' for 'Account1' should contain string value '11'
				And storage for profile 'Profile2' for 'Account2' should contain string value '22'
			"
				.Execute();
		}

		[Test]
		public void OverwriteDataForTheSameContext()
		{
			@"Given account 'Account1' has profile 'Profile1' created
			When store string value '11' to profile 'Profile1' for account 'Account1'
				And store string value '22' to profile 'Profile1' for account 'Account1'
			Then storage for profile 'Profile1' for 'Account1' should contain string value '22'
			"
				.Execute();
		}

		[Test]
		public void SaveProfileForSpecificContext()
		{
			@"Given account 'Account1' created
			When create profile 'Profile1' for account 'Account1'
			Then storage for 'Account1' should contain one profile 'Profile1'
			"
				.Execute();
		}

		[Test]
		public void ShouldAddEntityToList()
		{
			@"Given account 'Account1' has profile 'Profile1' created
			When store string value '11' to list in profile 'Profile1' for account 'Account1'
				And store string value '22' to list in profile 'Profile1' for account 'Account1'
			Then storage for profile 'Profile1' for 'Account1' should contain list of strings: 11, 22
			"
				.Execute();
		}

		[Test]
		public void ShouldUpdateEntityList()
		{
			@"Given account 'Account' has profile 'Profile' created
				And store string value '11' to list in profile 'Profile' for account 'Account'
				And store string value '22' to list in profile 'Profile' for account 'Account'
				And store string value '33' to list in profile 'Profile' for account 'Account'
			When update '22' value with '22_updated' value in list for profile 'Profile' for account 'Account'
			Then storage for profile 'Profile' for 'Account' should contain list of strings: 11, 22_updated, 33
			"
				.Execute();
		}

		[Test]
		public void ShouldRetrieveValueSubsetByName()
		{
			@"Given account 'Account' has profile 'Profile' created
				When storage 'CustomValuesStorage' for the profile is: 11, 22, 33
				Then the storage retrieved should be: 11, 22, 33
			"
				.Execute();
		}

		[Test]
		public void ShouldRetrieveValueSubsetByNames()
		{
			@"Given account 'Account' has profile 'Profile' created
				When storage 'CustomValuesStorage1' for the profile is: 11, 22, 33
					And storage 'CustomValuesStorage2' for the profile is: 44, 55
				Then the storage retrieved by names 'CustomValuesStorage1,CustomValuesStorage2' should be: 11, 22, 33, 44, 55
			"
				.Execute();
		}

		[Test]
		public void ShouldUpdateValueSubset()
		{
			@"Given account 'Account' has profile 'Profile' created
				And storage 'CustomValuesStorage' for the profile is: 11, 22, 33
				When update the named storage with values: 11_updated, 22_updated, 33_updated
				Then the storage retrieved should be: 11_updated, 22_updated, 33_updated"
				.Execute();
		}

		[Test]
		public void ShouldRetrieveEmptyValueSubsetIfNoStorageNamesProvided()
		{
			@"Given account 'Account' has profile 'Profile' created
				When storage 'CustomValuesStorage1' for the profile is: 11, 22, 33
				Then the storage retrieved by empty names array should be empty
			"
				.Execute();
		}

		[When(@"storage '$storageName' for the profile is: (?<namedStorageValues>([^,]+,?\s*)+)")]
		public void CreateSubset(string storageName, string[] namedStorageValues)
		{
			SetCurrentPluginContext(Context.AccountName, Context.ProfileName);

			var storage = StorageRepository.Get<string>(storageName);
			Context.CurrentStorageName = storageName;
			storage.AddRange(namedStorageValues);
		}

		[When(@"update the named storage with values: (?<storageValuesUpdated>([^,]+,?\s*)+)")]
		public void UpdateSubset(string[] storageValuesUpdated)
		{
			var subset = StorageRepository.Get<string>(Context.CurrentStorageName);
			subset.ReplaceWith(storageValuesUpdated);
		}

		[Then(@"the storage retrieved should be: (?<subsetValues>([^,]+,?\s*)+)")]
		public void AssertRetrievedSubset(string[] subsetValues)
		{
			var subset = StorageRepository.Get<string>(Context.CurrentStorageName);
			subset.ToArray().Should(Be.EquivalentTo(subsetValues), "subset.ToArray().Should(Be.EquivalentTo(subsetValues))");
		}

		[Then("the storage retrieved by empty names array should be empty")]
		public void AssertEmptySubset()
		{
			var subset = StorageRepository.Get<string>(Enumerable.Empty<StorageName>().ToArray()).ToArray();
			subset.ToArray().Should(Be.Empty, "subset.ToArray().Should(Be.Empty)");
		}

		[Then(@"the storage retrieved by names '$names' should be: (?<subsetValues>([^,]+,?\s*)+)")]
		public void AssertRetrievedSubset(string names, string[] subsetValues)
		{
			var subset = StorageRepository.Get<string>(names.Split(',').Select(n => new StorageName(n)).ToArray());
			subset.ToArray().Should(Be.EquivalentTo(subsetValues), "subset.ToArray().Should(Be.EquivalentTo(subsetValues))");
		}

		private static ProfileStorageContext Context
		{
			get { return ObjectFactory.GetInstance<ProfileStorageContext>(); }
		}

		[When(
			"update '$valueToUpdate' value with '$newValue' value in list for profile '$profileName' for account '$accountName'")
		]
		public void ReplaceValueInList(string valueToUpdate, string newValue, string profileName, string accountName)
		{
			SetCurrentPluginContext(accountName, profileName);

			var list = StorageRepository.Get<StringValue>();
			list.Remove(x => x.Value == valueToUpdate);

			list.Add(new StringValue {Value = newValue});
		}

		[Serializable]
		public class StringValue
		{
			public string Value { get; set; }
		}

		private static IStorageRepository StorageRepository
		{
			get { return ObjectFactory.GetInstance<IStorageRepository>(); }
		}

		[Given("account '$accountName' has profile '$profileName' created")]
		public void CreateProfile(string accountName, string profileName)
		{
			ObjectFactory.GetInstance<PluginContextMock>().AccountName = accountName;
			ProfileCollection.Add(new ProfileCreationArgs(profileName, new object()));

			Bus.ResetExpectations();
			NServiceBusMockRegistry.Setup(Bus);

			Context.AccountName = accountName;
			Context.ProfileName = profileName;
		}

		[Given("account '$accountName' created")]
		public void CreateAccount(string accountName)
		{
		}

		[Given(@"profile '$profileName' for account '$accountName' created")]
		public void CreateProfileForAccountAndPlugin(string profileName, string accountName)
		{
			ObjectFactory.GetInstance<PluginContextMock>().ProfileName = profileName;
			ObjectFactory.GetInstance<PluginContextMock>().AccountName = accountName;
			CreateProfile(accountName, profileName);
		}

		[When("store string value '$stringValue' to profile '$profileName' for account '$accountName'")]
		public void StoreStringValueToProfileStorage(string stringValue, string profileName, string accountName)
		{
			SetCurrentPluginContext(accountName, profileName);
			StorageRepository.Get<StringValue>().ReplaceWith(new StringValue {Value = stringValue});
		}

		[When("store string value '$stringValue' to list in profile '$profileName' for account '$accountName'")]
		public void StoreStringValueToListInProfileStorage(string stringValue, string profileName, string accountName)
		{
			SetCurrentPluginContext(accountName, profileName);

			var list = StorageRepository.Get<StringValue>();
			list.Add(new StringValue {Value = stringValue});
		}

		[When("create profile '$profileName' for account '$accountName'")]
		public void CreateProfileForAccount(string profileName, string accountName)
		{
			ObjectFactory.GetInstance<PluginContextMock>().AccountName = accountName;
			ProfileCollection.Add(new ProfileCreationArgs(profileName, new object()));
		}

		[Then("storage for profile '$profileName' for '$accountName' should contain string value '$stringValue'")]
		public void ProfileStorageShouldContainStringValue(string profileName, string accountName, string stringValue)
		{
			SetCurrentPluginContext(accountName, profileName);
			StorageRepository.Get<StringValue>().First().Value.Should(Be.EqualTo(stringValue), "StorageRepository.Get<StringValue>().First().Value.Should(Be.EqualTo(stringValue))");
		}

		[Then(
			@"storage for profile '$profileName' for '$accountName' should contain list of strings: (?<stringValues>([^,]+,?\s*)+)"
			)]
		public void ProfileStorageShouldContainListOfStrings(string profileName, string accountName, string[] stringValues)
		{
			SetCurrentPluginContext(accountName, profileName);
			var list = StorageRepository.Get<StringValue>();
			list.Select(x => x.Value).ToArray().Should(Be.EquivalentTo(stringValues), "list.Select(x => x.Value).ToArray().Should(Be.EquivalentTo(stringValues))");
		}

		[Then("storage for '$accountName' should contain one profile '$profileName'")]
		public void ProfileRepositoryShouldContainOneProfile(string accountName, string profileName)
		{
			ObjectFactory.GetInstance<PluginContextMock>().AccountName = accountName;
			ProfileCollection.ToList().Any(x => x.Name == profileName).
				Should(Be.True, string.Format("Cannot find profile {0} in repository", profileName));
		}

		private static void SetCurrentPluginContext(AccountName accountName, ProfileName profileName, string pluginName)
		{
			var context = ObjectFactory.GetInstance<PluginContextMock>();
			context.AccountName = accountName;
			context.PluginName = pluginName;
			context.ProfileName = profileName;

			var profile = ProfileCollection.First(x => x.Name == profileName);
			Bus.SetIn(accountName);
			Bus.SetIn(profile.Name);
		}

		public static void SetCurrentPluginContext(AccountName accountName, ProfileName profileName)
		{
			SetCurrentPluginContext(accountName, profileName,
									ObjectFactory.GetInstance<IPluginMetadata>().PluginData.Name);
		}

		private static IBus Bus
		{
			get { return ObjectFactory.GetInstance<IBus>(); }
		}

		private static IProfileCollection ProfileCollection
		{
			get { return ObjectFactory.GetInstance<IProfileCollection>(); }
		}
	}
}