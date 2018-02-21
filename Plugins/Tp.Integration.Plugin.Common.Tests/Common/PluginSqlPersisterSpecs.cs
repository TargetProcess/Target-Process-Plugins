// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using System.Runtime.Serialization;
using NBehave.Narrator.Framework;
using NServiceBus;
using NUnit.Framework;
using StructureMap;
using Tp.Integration.Messages.ServiceBus;
using Tp.Integration.Messages.Ticker;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Testing.Common.NBehave;
using Tp.Testing.Common.NUnit;

namespace Tp.Integration.Plugin.Common.Tests.Common
{
    [TestFixture]
    [ActionSteps]
    [Category("PartPlugins1")]
    public class PluginSqlPersisterSpecs : SqlPersisterSpecBase
    {
        [Test]
        public void ShouldStoreAccountInfo()
        {
            @"Given plugin in local database mode
				When store account 'Account1' with profiles: Profile1_1
				Then account 'Account1' should have profiles stored in local database: Profile1_1"
                .Execute();
        }

        [Test]
        public void ShouldStoreProfileData()
        {
            @"Given plugin in local database mode
					And store account 'Account1' with profiles: Profile1_1
				When store string value 'testString' in profile 'Profile1_1'
				Then profile 'Profile1_1' should have string value 'testString'"
                .Execute();
        }

        [Test]
        public void ShouldStoreListOfProfileData()
        {
            @"Given plugin in local database mode
					And store account 'Account1' with profiles: Profile1_1
				When store string value 'testString0' to list in profile 'Profile1_1'
					And store string value 'testString1' to list in profile 'Profile1_1'
				Then profile 'Profile1_1' should have string value list: testString0, testString1"
                .Execute();
        }

        [Test]
        public void ShouldDeleteProfileData()
        {
            @"Given plugin in local database mode
					And store account 'Account1' with profiles: Profile1_1, Profile1_2
				When remove profile 'Profile1_1' from account 'Account1'
				Then account 'Account1' should have profiles stored in local database: Profile1_2"
                .Execute();
        }

        [Test]
        public void ShouldUpdateProfileData()
        {
            @"Given plugin in local database mode
					And store account 'Account1' with profiles: Profile1_1
					And profile 'Profile1_1' has JiraLogin set to 'TestLogin' for account 'Account1'
				When update 'Profile1_1' JiraLogin to 'TestLogin_Updated' for account 'Account1'
				Then account 'Account1' should have profile 'Profile1_1' with JiraLogin 'TestLogin_Updated'"
                .Execute();
        }

        [Test]
        public void ShouldCreateProfileInLocalDatabase()
        {
            @"
				Given plugin in local database mode
					And store account 'Account1' with profiles: Profile1_1
				When create 'Profile1_2' for account 'Account1'
				Then account 'Account1' should have profiles stored in local database: Profile1_1, Profile1_2"
                .Execute();
        }

        [Test]
        public void ShouldNotSaveNotSerializableValue()
        {
            PutPluginInLocalDatabaseMode();
            StoreAccount("Account", new[] { "Profile" });
            var accountCollection = ObjectFactory.GetInstance<IAccountCollection>();
            var pluginStorage =
                accountCollection.GetOrCreate("Account").Profiles.Single(x => x.Name == "Profile");

            try
            {
                pluginStorage.Get<LastSyncDate>().Add(new LastSyncDate(DateTime.MinValue));
                Assert.Fail("SerializationException was not thrown");
            }
            catch (SerializationException)
            {
            }
            finally
            {
                pluginStorage.Get<LastSyncDate>().Should(Be.Empty, "pluginStorage.Get<LastSyncDate>().Should(Be.Empty)");
            }

            pluginStorage.Get<LastSyncDate>().Add(new LastSyncDate(DateTime.Now));
            pluginStorage.Get<LastSyncDate>().Should(Be.Not.Empty, "pluginStorage.Get<LastSyncDate>().Should(Be.Not.Empty)");
        }

        [Test]
        public void ShouldRevertRange()
        {
            PutPluginInLocalDatabaseMode();
            StoreAccount("Account", new[] { "Profile" });
            var accountCollection = ObjectFactory.GetInstance<IAccountCollection>();
            var pluginStorage =
                accountCollection.GetOrCreate("Account").Profiles.Single(x => x.Name == "Profile");

            try
            {
                pluginStorage.Get<LastSyncDate>().AddRange(new[]
                    { new LastSyncDate(DateTime.MinValue), new LastSyncDate(DateTime.Now) });
                Assert.Fail("SerializationException was not thrown");
            }
            catch (SerializationException)
            {
            }
            finally
            {
                pluginStorage.Get<LastSyncDate>().Should(Be.Empty, "pluginStorage.Get<LastSyncDate>().Should(Be.Empty)");
            }

            pluginStorage.Get<LastSyncDate>().AddRange(new[] { new LastSyncDate(DateTime.Now), new LastSyncDate(DateTime.Now) });
            pluginStorage.Get<LastSyncDate>()
                .Count()
                .Should(Be.EqualTo(2), "pluginStorage.Get<LastSyncDate>().Count().Should(Be.EqualTo(2))");
        }

        #region Action Steps

        [Given("profile '$profileName' has JiraLogin set to '$JiraLogin' for account '$accountName'")
        ]
        public void SetJiraLoginInProfile(string profileName, string jiraLogin, string accountName)
        {
            var profileToUpdate = GetAccount(accountName).Profiles[profileName];
            profileToUpdate.Settings = new SampleJiraProfile { JiraLogin = jiraLogin };
            profileToUpdate.Save();
        }

        [When("update '$profileName' JiraLogin to '$jiraLoginUpdated' for account '$accountName'")]
        public void UpdateJiraLoginForProfile(string profileName, string jiraLoginUpdated, string accountName,
            string pluginName)
        {
            var profile = ObjectFactory.GetInstance<IAccountCollection>().GetOrCreate(accountName).Profiles[profileName];
            profile.Settings = new SampleJiraProfile { JiraLogin = jiraLoginUpdated };
            profile.Save();
        }

        [Then("account '$accountName' should have profile '$profileName' with JiraLogin '$jiraLogin'")]
        public void JiraLoginInProfileShouldMatch(string accountName, string profileName, string jiraLogin)
        {
            GetAccount(accountName).Profiles.First(x => x.Name == profileName).GetProfile<SampleJiraProfile>().JiraLogin.Should(
                Be.EqualTo(jiraLogin),
                "GetAccount(accountName).Profiles.First(x => x.Name == profileName).GetProfile<SampleJiraProfile>().JiraLogin.Should(Be.EqualTo(jiraLogin))");
        }

        [When(@"remove profile '$profileName' from account '$accountName'")]
        public void RemoveProfile(string profileName, string accountName)
        {
            var account = GetAccount(accountName);
            account.Profiles.Remove(account.Profiles[profileName]);
        }

        private static IAccount GetAccount(string accountName)
        {
            return ObjectFactory.GetInstance<IAccountCollection>().GetOrCreate(accountName);
        }

        [When("create '$profileName' for account '$accountName'")]
        public void CreateProfile(string profileName, string accountName)
        {
            GetAccount(accountName).Profiles.Add(new ProfileCreationArgs(profileName, new object()));
        }

        [Given("plugin in local database mode")]
        public void PutPluginInLocalDatabaseMode()
        {
            SetupDB();
        }

        [When(@"store account '$accountName' with profiles: (?<profileNames>([^,]+,?\s*)+)")]
        public void StoreAccount(string accountName, string[] profileNames)
        {
            var account = ObjectFactory.GetInstance<IAccountCollection>().GetOrCreate(accountName);
            foreach (var profileName in profileNames)
            {
                account.Profiles.Add(new ProfileCreationArgs(profileName, new object()));
            }

            ObjectFactory.GetInstance<IBus>().CurrentMessageContext.Headers[BusExtensions.ACCOUNTNAME_KEY] = accountName;
        }

        [When("store string value '$stringValue' in profile '$profileName'")]
        public void StoreValueInProfile(string stringValue, string profileName)
        {
            var profile = GetProfile(profileName);
            profile.Get<string>().ReplaceWith(stringValue);
        }

        [When("store string value '$stringValue' to list in profile '$profileName'")]
        public void StoreValueToListInProfile(string stringValue, string profileName)
        {
            var profile = GetProfile(profileName);
            var list = profile.Get<string>();
            list.Add(stringValue);
        }

        public static IProfileReadonly GetProfile(string profileName)
        {
            return (from account in ObjectFactory.GetInstance<IAccountCollection>()
                where account.Profiles.Any(x => x.Name == profileName)
                select account.Profiles.First(x => x.Name == profileName)).FirstOrDefault();
        }

        [Then(@"account '$accountName' should have profiles stored in local database: (?<profileNames>([^,]+,?\s*)+)")]
        public void AccountShouldHaveProfiles(string accountName, string[] profileNames)
        {
            GetAccount(accountName)
                .Profiles.Select(x => x.Name.Value)
                .ToArray()
                .Should(Be.EquivalentTo(profileNames),
                    "GetAccount(accountName).Profiles.Select(x => x.Name.Value).ToArray().Should(Be.EquivalentTo(profileNames))");
        }

        [Then("profile '$profileName' should have string value '$stringValue'")]
        public void ProfileShouldHaveStringValue(string profileName, string stringValue)
        {
            var profile = GetProfile(profileName);
            profile.Get<string>().First().Should(Be.EqualTo(stringValue), "profile.Get<string>().First().Should(Be.EqualTo(stringValue))");
        }

        [Then(@"profile '$profileName' should have string value list: (?<stringValues>([^,]+,?\s*)+)")]
        public void ProfileShouldHaveStringValues(string profileName, string[] stringValues)
        {
            var profile = GetProfile(profileName);
            profile.Get<string>()
                .ToArray()
                .Should(Be.EquivalentTo(stringValues), "profile.Get<string>().ToArray().Should(Be.EquivalentTo(stringValues))");
        }

        #endregion
    }
}
