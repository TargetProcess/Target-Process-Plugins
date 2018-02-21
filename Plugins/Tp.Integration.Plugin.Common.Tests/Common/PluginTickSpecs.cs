// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using NBehave.Narrator.Framework;
using NServiceBus.Unicast.Transport;
using NUnit.Framework;
using StructureMap;
using Tp.Core;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Messages.ServiceBus;
using Tp.Integration.Messages.Ticker;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.PluginCommand.Embedded;
using Tp.Integration.Plugin.Common.Tests.Common.PluginCommand;
using Tp.Integration.Testing.Common;
using Tp.Testing.Common.NBehave;
using Tp.Testing.Common.NUnit;

namespace Tp.Integration.Plugin.Common.Tests.Common
{
    [TestFixture, ActionSteps]
    [Category("PartPlugins1")]
    public class PluginTickSpecs
    {
        [SetUp]
        public void BeforeScenario()
        {
            ObjectFactory.Configure(
                x =>
                    x.For<TransportMock>().HybridHttpOrThreadLocalScoped().Use(
                        TransportMock.CreateWithoutStructureMapClear(typeof(PluginTickSpecs).Assembly)));
        }

        [Test]
        public void ShouldSendTickMessageToPluginIfIntervalElapsed()
        {
            @"
				Given profile with name 'Profile1' for account 'Account1'
					And profile with name 'Profile2' for account 'Account2'
					And profile 'Profile1' for account 'Account1' has Sync Interval '5' minutes
					And profile 'Profile1' for account 'Account1' has Last Synchronization Date '2010-11-29 16:48:00.000'
					And profile 'Profile2' for account 'Account2' has Sync Interval '5' minutes
					And profile 'Profile2' for account 'Account2' has Last Synchronization Date '2010-11-29 16:49:00.000'
					And Current Time is '2010-11-29 16:53:00.000'
				When Check Interval Elapsed
				Then Tick message should be sent to profile 'Profile1' for account 'Account1'
					And Last Synchronization Date should be about '2010-11-29 16:53:00.000' in profile 'Profile1' for account 'Account1'"
                .Execute(In.Context<WhenAddANewProfileSpecs>().And<PluginTickSpecs>());
        }

        [Test]
        public void ShouldSendTickMessageToPluginIfNoLastSyncDateSpecified()
        {
            @"
				Given profile with name 'Profile1' for account 'Account1'
					And profile 'Profile1' for account 'Account1' has Sync Interval '5' minutes
					And Current Time is '2010-11-29 16:53:00.000'
				When Check Interval Elapsed
				Then Tick message should be sent to profile 'Profile1' for account 'Account1'
					And Last Synchronization Date should be empty for account 'Account1' and profile 'Profile1'"
                .Execute(In.Context<WhenAddANewProfileSpecs>().And<PluginTickSpecs>());
        }

        [Test]
        public void ShouldNotSendTickMessageIfProfileIsNotInitialized()
        {
            @"
				Given profile with name 'Profile1' for account 'Account1'
					And profile 'Profile1' for account 'Account1' has Sync Interval '5' minutes
					And profile 'Profile1' for account 'Account1' is not initialized
					And Current Time is '2010-11-29 16:53:00.000'
				When Check Interval Elapsed
				Then no Tick message should be sent"
                .Execute(In.Context<WhenAddANewProfileSpecs>().And<PluginTickSpecs>());
        }

        [Test]
        public void ShouldNotTickTwiceAtOneTime()
        {
            @"
				Given profile with name 'Profile1' for account 'Account1'
					And profile 'Profile1' for account 'Account1' has Sync Interval '1' minutes
					And profile 'Profile1' for account 'Account1' has Last Synchronization Date '2010-11-29 16:48:00.000'
					And Current Time is '2010-11-29 16:53:00.000'
				When Check Interval Elapsed occurs several times
				Then only one Tick message should be sent to profile 'Profile1' for account 'Account1'
					And Last Synchronization Date should be about '2010-11-29 16:53:00.000' in profile 'Profile1' for account 'Account1'"
                .Execute(In.Context<WhenAddANewProfileSpecs>().And<PluginTickSpecs>());
        }

        [Test]
        public void ShouldHasEmptyLastSyncDateOnFirstTick()
        {
            @"
				Given profile with name 'Profile1' for account 'Account1'
				When Check Interval Elapsed
				Then Last Synchronization Date should be empty for account 'Account1' and profile 'Profile1'
			".Execute(In.Context<WhenAddANewProfileSpecs>().And<PluginTickSpecs>());
        }

        [Test]
        public void ShouldHasNonEmptyLastSyncDateOnSecondTick()
        {
            @"
				Given profile with name 'Profile1' for account 'Account1'
				When Check Interval Elapsed occurs several times
				Then Last Synchronization Date should not be empty for account 'Account1' and profile 'Profile1'
			".Execute(In.Context<WhenAddANewProfileSpecs>().And<PluginTickSpecs>());
        }

        [Given("profile '$profileName' for account '$accountName' has Last Synchronization Date '$date'")]
        public void SetLastSyncDate(string profileName, string accountName, string date)
        {
            var profile = ObjectFactory.GetInstance<IAccountCollection>().GetOrCreate(accountName).Profiles[profileName];
            profile.Get<LastSyncDate>().ReplaceWith(new LastSyncDate(DateTime.Parse(date)));
        }

        [Given("profile '$profileName' for account '$accountName' is not initialized")]
        public void SetProfileNotInitialized(string profileName, string accountName)
        {
            var profile = ObjectFactory.GetInstance<IAccountCollection>().GetOrCreate(accountName).Profiles[profileName];
            profile.MarkAsNotInitialized();
            profile.Save();
        }

        [Given("profile '$profileName' for account '$accountName' has Sync Interval '$syncInterval' minutes")]
        public void SetSyncInterval(string profileName, string accountName, int syncInterval)
        {
            var currentProfileDto = new PluginProfileDto
            {
                Name = profileName,
                Settings = new SampleProfileSerialized { SynchronizationInterval = syncInterval }
            };
            var createProfileCmd = new ExecutePluginCommandCommand
            {
                CommandName = EmbeddedPluginCommands.AddOrUpdateProfile,
                Arguments = currentProfileDto.Serialize()
            };

            ObjectFactory.GetInstance<TransportMock>().HandleMessageFromTp(
                new List<HeaderInfo> { new HeaderInfo { Key = BusExtensions.ACCOUNTNAME_KEY, Value = accountName } }, createProfileCmd);
        }

        [Given("Current Time is '$currentTime'")]
        public void SetCurrentTime(string currentTime)
        {
            CurrentDate.Setup(() => DateTime.Parse(currentTime));
        }

        [When("Check Interval Elapsed")]
        public void CheckIntervalElapsed()
        {
            ObjectFactory.GetInstance<TransportMock>().HandleLocalMessage(new List<HeaderInfo>(),
                new CheckIntervalElapsedMessage());
        }

        [When("Check Interval Elapsed occurs several times")]
        public void CheckIntervalElapsedOccursSeveralTimes()
        {
            ObjectFactory.GetInstance<TransportMock>().HandleLocalMessage(new List<HeaderInfo>(),
                new CheckIntervalElapsedMessage(),
                new CheckIntervalElapsedMessage());
        }

        [Then("Tick message should be sent to profile '$profileName' for account '$accountName'")]
        [Then("only one Tick message should be sent to profile '$profileName' for account '$accountName'")]
        public void TickMessagesShouldBeSentTo(string profileName, string accountName)
        {
            var tickMessage = ObjectFactory.GetInstance<TransportMock>().LocalQueue.GetMessageInfos<TickMessage>().Single();
            tickMessage.AccountName.Should(Be.EqualTo(accountName), "tickMessage.AccountName.Should(Be.EqualTo(accountName))");
            tickMessage.ProfileName.Should(Be.EqualTo(profileName), "tickMessage.ProfileName.Should(Be.EqualTo(profileName))");
        }

        [Then("no Tick message should be sent")]
        public void NoTickMessageShouldBeSent()
        {
            ObjectFactory.GetInstance<TransportMock>()
                .LocalQueue.GetMessageInfos<TickMessage>()
                .Should(Be.Empty, "ObjectFactory.GetInstance<TransportMock>().LocalQueue.GetMessageInfos<TickMessage>().Should(Be.Empty)");
        }

        [Then(
             "Last Synchronization Date should be about '$lastSyncDateUpdated' in profile '$profileName' for account '$accountName'"
         )]
        public void LastSyncDateShouldBe(string lastSyncDateUpdated, string profileName, string accountName)
        {
            var profile = ObjectFactory.GetInstance<IAccountCollection>().GetOrCreate(accountName).Profiles[profileName];
            profile.Get<LastSyncDate>()
                .First()
                .Value.Minute.Should(Be.EqualTo(DateTime.Parse(lastSyncDateUpdated).Minute),
                    "profile.Get<LastSyncDate>().First().Value.Minute.Should(Be.EqualTo(DateTime.Parse(lastSyncDateUpdated).Minute))");
        }

        [Then("Last Synchronization Date should be empty for account '$accountName' and profile '$profileName'")]
        public void LastSyncDateShouldBeEmpty(string accountName, string profileName)
        {
            ObjectFactory.GetInstance<TransportMock>().LocalQueue.GetMessageInfos<TickMessage>().First().Message.LastSyncDate.
                Should(Be.Null,
                    "ObjectFactory.GetInstance<TransportMock>().LocalQueue.GetMessageInfos<TickMessage>().First().Message.LastSyncDate.Should(Be.Null)");
        }

        [Then("Last Synchronization Date should not be empty for account '$accountName' and profile '$profileName'")]
        public void LastSyncDateShouldBeNotEmpty(string accountName, string profileName)
        {
            ObjectFactory.GetInstance<TransportMock>().LocalQueue.GetMessageInfos<TickMessage>().Skip(1).First().Message.LastSyncDate.
                Should(Be.Not.Null,
                    "ObjectFactory.GetInstance<TransportMock>().LocalQueue.GetMessageInfos<TickMessage>().Skip(1).First().Message.LastSyncDate.Should(Be.Not.Null)");
        }
    }
}
