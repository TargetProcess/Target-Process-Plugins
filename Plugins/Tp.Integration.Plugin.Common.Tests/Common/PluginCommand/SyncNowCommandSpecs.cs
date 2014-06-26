// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Linq;
using NBehave.Narrator.Framework;
using NServiceBus.Unicast.Transport;
using NUnit.Framework;
using StructureMap;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Messages.ServiceBus;
using Tp.Integration.Messages.Ticker;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Testing.Common;
using Tp.Testing.Common.NBehave;
using Tp.Testing.Common.NUnit;

namespace Tp.Integration.Plugin.Common.Tests.Common.PluginCommand
{
	[TestFixture, ActionSteps]
    [Category("PartPlugins1")]
	public class SyncNowCommandSpecs
	{
		[Test]
		public void ShouldStartSynchronizingWhenCommandReceived()
		{
			@"Given profile with name 'Profile1' for account 'Account'
					And profile with name 'Profile2' for account 'Account'
				When synchronization is forced for acccount 'Account' for profile 'Profile1'
				Then synchronization should be started for profile 'Profile1'
					And last sync date should be updated for profile 'Profile1'
					And command should be executed successfully"
				.Execute(In.Context<WhenAddANewProfileSpecs>().And<SyncNowCommandSpecs>());
		}

		[Test]
		public void ShouldFailIfProfileIsNotSpecified()
		{
			@"Given profile with name 'Profile1' for account 'Account'
					And profile with name 'Profile2' for account 'Account'
				When synchronization is forced for acccount 'Account'
				Then synchronization should not be started for any profile
					And command should fail"
				.Execute(In.Context<WhenAddANewProfileSpecs>().And<SyncNowCommandSpecs>());
		}

		[Test]
		public void ShouldFailIfProfileDoesNotExist()
		{
			@"Given profile with name 'Profile1' for account 'Account'
					And profile with name 'Profile2' for account 'Account'
				When synchronization is forced for acccount 'Account' for profile 'Profile3'
				Then synchronization should not be started for any profile
					And command should fail"
				.Execute(In.Context<WhenAddANewProfileSpecs>().And<SyncNowCommandSpecs>());
		}

		[When("synchronization is forced for acccount '$accountName' for profile '$profileName'")]
		public void StartSync(string accountName, string profileName)
		{
			ObjectFactory.GetInstance<TransportMock>().HandleMessageFromTp(
				new List<HeaderInfo>
					{
						new HeaderInfo {Key = BusExtensions.ACCOUNTNAME_KEY, Value = accountName},
						new HeaderInfo {Key = BusExtensions.PROFILENAME_KEY, Value = profileName}
					},
				new ExecutePluginCommandCommand {CommandName = EmbeddedPluginCommands.SyncNow});
		}

		[When("synchronization is forced for acccount '$accountName'")]
		public void StartSync(string accountName)
		{
			ObjectFactory.GetInstance<TransportMock>().HandleMessageFromTp(
				new List<HeaderInfo>
					{
						new HeaderInfo {Key = BusExtensions.ACCOUNTNAME_KEY, Value = accountName},
					},
				new ExecutePluginCommandCommand {CommandName = EmbeddedPluginCommands.SyncNow});
		}

		[Then(@"synchronization should be started for profile '$profileName'")]
		public void SyncShouldBeStarted(string profileName)
		{
			ObjectFactory.GetInstance<TransportMock>().LocalQueue.GetMessageInfos<TickMessage>().Count().Should(Be.EqualTo(1));
			ObjectFactory.GetInstance<TransportMock>().LocalQueue.GetMessageInfos<TickMessage>().Single(
				x => x.ProfileName == profileName).Should(Be.Not.Null);
		}

		[Then(@"synchronization should not be started for any profile")]
		public void SyncShouldNotBeStarted()
		{
			ObjectFactory.GetInstance<TransportMock>().LocalQueue.GetMessageInfos<TickMessage>().Count().Should(Be.EqualTo(0));
		}

		[Then("command should be executed successfully")]
		public void CheckSuccessStatus()
		{
			ObjectFactory.GetInstance<TransportMock>().TpQueue.GetMessages<PluginCommandResponseMessage>().Last().
				PluginCommandStatus.Should(Be.EqualTo(PluginCommandStatus.Succeed));
		}

		[Then("command should fail")]
		public void CheckFailStatus()
		{
			ObjectFactory.GetInstance<TransportMock>().TpQueue.GetMessages<PluginCommandResponseMessage>().Last().
				PluginCommandStatus.Should(Be.EqualTo(PluginCommandStatus.Error));
		}

		[Then("last sync date should be updated for profile '$profileName'")]
		public void CheckLastSyncDate(string profileName)
		{
			ObjectFactory.GetInstance<IStorageRepository>().Get<LastSyncDate>().First().Should(Be.Not.Null);
		}
	}
}