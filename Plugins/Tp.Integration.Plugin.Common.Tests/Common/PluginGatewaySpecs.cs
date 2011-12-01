// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Linq;
using NBehave.Narrator.Framework;
using NServiceBus;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap;
using Tp.Integration.Messages;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Messages.ServiceBus;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Gateways;
using Tp.Integration.Plugin.Common.Storage.Persisters;
using Tp.Testing.Common.NBehave;
using Tp.Testing.Common.NUnit;
using IProfile = Tp.Integration.Plugin.Common.Domain.IProfile;

namespace Tp.Integration.Plugin.Common.Tests.Common
{
	[TestFixture]
	public class PluginGatewaySpecs
	{
		[SetUp]
		public void Init()
		{
			ObjectFactory.Initialize(x =>
			                         	{
			                         		x.AddRegistry<NServiceBusMockRegistry>();
			                         		x.For<ITpBus>().Use<TpBus>();
			                         		x.For<IAccountCollection>().HybridHttpOrThreadLocalScoped().Use(MockRepository.GenerateStub<IAccountCollection>());
			                         		x.For<IProfileGatewayFactory>().HybridHttpOrThreadLocalScoped().Use(MockRepository.GenerateStub<IProfileGatewayFactory>());
			                         		x.IncludeRegistry<PluginMetadataSourceMockRegistry>();
			                         		x.For<IPluginContext>().HybridHttpOrThreadLocalScoped().Use<PluginContext>();
											x.For<IPluginCurrentObjectContext>().HybridHttpOrThreadLocalScoped().Use<PluginCurrentObjectContext>();
											x.For<IProfileCollection>().Use<CurrentProfileCollection>();
			                         	});
		}

		[Test]
		public void ShouldHandlePluginMessages()
		{
			@"Given account 'Account' defined
					And account 'Account' has profiles: profile1, profile2
				When plugin message received
				Then plugin message should be sent locally to profiles: profile1, profile2
				"
				.Execute(In.Context<PluginGatewayActionSteps>());
		}

		[Test]
		public void ShouldNotHandleAlreadyDistributedMessages()
		{
			@"Given account 'Account' defined
					And account 'Account' has profiles: profile1, profile2
				When already distributed to profile 'profile1' plugin message received
				Then no plugin messages should be sent locally
					"
				.Execute(In.Context<PluginGatewayActionSteps>());
		}

		#region ActionSteps

		[ActionSteps]
		public class PluginGatewayActionSteps
		{
			[Then("plugin command message should be passed to another handler")]
			public void PluginCommandMessageShouldBePassedToAnotherHandler()
			{
				Bus.AssertWasNotCalled(x => x.DoNotContinueDispatchingCurrentMessageToHandlers());
			}

			[When("already distributed to profile '$profileName' plugin message received")]
			public void ReceiveAlreadyDistributedMessageForProfile(string profileName)
			{
				Bus.SetIn(_account.Name);
				Bus.SetIn(new ProfileName(profileName));
				Gateway.Handle(MockRepository.GenerateStub<ITargetProcessMessage>());
			}

			[Then("no plugin messages should be sent locally")]
			public void NoPluginMessagesShouldBeSentLocally()
			{
				ProfileGatewayFactory.AssertWasNotCalled(x => x.Create(null, null), m => m.IgnoreArguments());
			}

			private IAccount _account;

			[Given("account '$accountName' defined")]
			public void CreateAccount(string accountName)
			{
				_account = MockRepository.GenerateStub<IAccount>();
				_account.Stub(x => x.Name).Return(accountName);
				_account.Stub(x => x.Profiles).Return(MockRepository.GenerateStub<IProfileCollection>());
			}

			[Given(@"account '$accountName' has profiles: (?<profileNames>([^,]+,?\s*)+)")]
			public void CreatePluginProfiles(string accountName, string[] pluginProfileNames)
			{
				_profiles = pluginProfileNames.Select(x => new Profile {Name = x}).OfType<IProfile>().ToList();
				var enumerator = _profiles.GetEnumerator();

				_account.Profiles.Stub(x => x.GetEnumerator()).Return(enumerator);
				_profiles.ForEach(x => _account.Profiles.Stub(y => y[x.Name]).Return(x));

				AccountCollection.Stub(x => x.GetOrCreate(accountName)).Return(_account);

				foreach (var profile in _profiles)
				{
					ProfileGatewayFactory.Stub(y => y.Create(_account.Name, profile)).Return(
						MockRepository.GenerateStub<IProfileGateway>());
				}
			}

			private static IAccountCollection AccountCollection
			{
				get { return ObjectFactory.GetInstance<IAccountCollection>(); }
			}

			private static IProfileGatewayFactory ProfileGatewayFactory
			{
				get { return ObjectFactory.GetInstance<IProfileGatewayFactory>(); }
			}


			[When("plugin message received")]
			public void ReceivePluginMessage()
			{
				_pluginMessage = MockRepository.GenerateStub<ITargetProcessMessage>();
				Bus.SetIn(_account.Name);

				Gateway.Handle(_pluginMessage);
			}

			[When("plugin saga message for profile '$profileName' received")]
			public void ReceivePluginSagaMessage(string profileName)
			{
				var sagaMessage = MockRepository.GenerateStub<ISagaMessage>();

				Bus.SetIn(_account.Name);
				Bus.SetIn((ProfileName) profileName);
				_profileSpecificPluginMessage = sagaMessage;
				Gateway.Handle(_profileSpecificPluginMessage);
			}

			[Then(@"plugin message should be sent locally to profiles: (?<profileNames>([^,]+,?\s*)+)")]
			public void PluginMessagesShouldBeSentToProfiles(string[] profileNames)
			{
				MessageShouldBeSentToProfiles(_pluginMessage, profileNames);
			}

			[Then(@"plugin saga message should be sent locally to bus with profile '$profileName'")]
			public void PluginSagaMessageShouldBeSentToProfiles(string profileName)
			{
				var bus = ObjectFactory.GetInstance<IBus>();
				bus.AssertWasCalled(x => x.SendLocal(_profileSpecificPluginMessage));
				bus.OutgoingHeaders[BusExtensions.PROFILENAME_KEY].Should(Be.EqualTo(profileName));
				bus.OutgoingHeaders[BusExtensions.ACCOUNTNAME_KEY].Should(Be.EqualTo(_account.Name.Value));
			}

			[Then(@"no message should be sent to profiles: (?<profileNames>([^,]+,?\s*)+)")]
			public void NoMessageShouldBeSentToProfiles(string[] profileNames)
			{
				foreach (var gateway in
					_profiles.Where(profile => profileNames.Contains(profile.Name.Value)).Select(
						profile => ProfileGatewayFactory.Create(_account.Name, profile)))
				{
					gateway.AssertWasNotCalled(x => x.Send(null), methodOptions => methodOptions.IgnoreArguments());
				}
			}

			#region Helpers

			private void MessageShouldBeSentToProfiles(ITargetProcessMessage message, IEnumerable<string> profileNames)
			{
				foreach (var profile in _profiles)
				{
					if (!profileNames.Contains(profile.Name.Value)) continue;
					var gateway = ProfileGatewayFactory.Create(_account.Name, profile);
					gateway.AssertWasCalled(x => x.Send(message));
				}
			}

			private PluginGateway _gateway;
			private ITargetProcessMessage _pluginMessage;
			private ITargetProcessMessage _profileSpecificPluginMessage;
			private IEnumerable<IProfile> _profiles;

			private PluginGateway Gateway
			{
				get { return _gateway ?? (_gateway = ObjectFactory.GetInstance<PluginGateway>()); }
			}

			private static IBus Bus
			{
				get { return ObjectFactory.GetInstance<IBus>(); }
			}

			#endregion
		}

		#endregion
	}
}