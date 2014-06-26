// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections;
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
using Tp.Testing.Common.NBehave;
using Tp.Testing.Common.NUnit;
using IProfile = Tp.Integration.Plugin.Common.Domain.IProfile;

namespace Tp.Integration.Plugin.Common.Tests.Common
{
	[TestFixture, ActionSteps]
    [Category("PartPlugins1")]
	public class ProfileGatewaySpecs : SqlPersisterSpecBase
	{
		private IProfileGateway _profileGateway;
		private SampleMessage _sampleMessage;
		private static IProfile _profile;
		private List<IMessage> _sentLocally;

		protected override void InitializeContainer(IInitializationExpression initializationExpression)
		{
			base.InitializeContainer(initializationExpression);
			initializationExpression.For<IPluginContext>().HybridHttpOrThreadLocalScoped().Use<PluginContext>();
		}

		[Test]
		public void ShouldPassMessagesToProfileWhenProfileInitialized()
		{
			@"Given profile 'Profile' for account 'Account' initialized
				When a message received
				Then a message should be passed to profile 'Profile' for account 'Account' for processing"
				.Execute();
		}

		[Test]
		public void ShouldNotPassMessagesToProfileWhenProfileNotInitialized()
		{
			@"Given profile 'Profile' for account 'Account' not initialized
				When a message received
				Then no message should be passed to profile 'Profile' for account 'Account' for processing"
				.Execute();
		}

		[Test]
		public void ShouldPersistMessagesWhileInitializationIsInProgress()
		{
			@"Given profile 'Profile' for account 'Account' not initialized
					And messages received: Message1, Message2
				When profile 'Profile' is marked as initialized
				Then messages should be passed to profile 'Profile' for account 'Account' for processing: Message1, Message2"
				.Execute();
		}

		[Test]
		public void ShouldStopMessageDistributionWhenBecameNotInitialized()
		{
			@"Given profile 'Profile' for account 'Account' initialized
					And messages received: Message1, Message2
					And profile 'Profile' became not initialized
				When messages received: Message3, Message4
				Then messages should be passed to profile 'Profile' for account 'Account' for processing: Message1, Message2"
				.Execute();
		}

		[BeforeScenario]
		public void BeforeScenario()
		{
			_sentLocally = new List<IMessage>();
			Bus.Stub(x => x.SendLocal(null)).IgnoreArguments().WhenCalled(x => _sentLocally.Add(((IMessage[]) x.Arguments[0])[0]));
		}

		[Given("profile '$profileName' for account '$accountName' initialized")]
		public void CreateInitializedProfile(string profileName, string accountName)
		{
			_profileGateway = CreateProfileGateway(accountName, profileName, true);
		}

		[Given("profile '$profileName' for account '$accountName' not initialized")]
		public void CreateNotInitializedProfile(string profileName, string accountName)
		{
			_profileGateway = CreateProfileGateway(accountName, profileName, false);
		}

		[Given(@"messages received: (?<messageNames>([^,]+,?\s*)+)")]
		public void MessagesReceived(string[] messageNames)
		{
			foreach (var messageName in messageNames)
			{
				_profileGateway.Send(new SampleMessage {Name = messageName});
			}
		}

		private static IProfileGateway CreateProfileGateway(AccountName accountName, ProfileName profileName, bool initialized)
		{
			var account = ObjectFactory.GetInstance<IAccountCollection>().GetOrCreate(accountName);
			_profile = account.Profiles.Add(new ProfileCreationArgs(profileName, new object()));
			if(initialized)
			{
				_profile.MarkAsInitialized();
			}
			else
			{
				_profile.MarkAsNotInitialized();
			}

			_profile.Save();

			return new ProfileGateway(_profile, accountName, ObjectFactory.GetInstance<ITpBus>());
		}

		[When("a message received")]
		public void MessageReceived()
		{
			_sampleMessage = new SampleMessage();
			_profileGateway.Send(_sampleMessage);
		}

		[When("profile '$profileName' became not initialized")]
		public void MakeProfileNotInitialized(string profileName)
		{
			_profile.Name.Value.Should(Be.EqualTo(profileName));
			_profile.MarkAsNotInitialized();
			_profile.Save();
		}

		[When("profile '$profileName' is marked as initialized")]
		public void MarkProfileAsInitialized(string profileName)
		{
			_profile.MarkAsInitialized();
			_profile.Save();
		}

		[Then("a message should be passed to profile '$profileName' for account '$accountName' for processing")]
		public void MessageShouldBePassedToProfile(string profileName, string accountName)
		{
			AssertMessagesWereSent(_sentLocally.OfType<SampleMessage>().Select(x => x.Name).ToArray(),
			                       new[] {_sampleMessage.Name}, profileName, accountName);
		}

		[Then("no message should be passed to profile '$profileName' for account '$accountName' for processing")]
		public void NoMessagesShouldBePassedToProfile(string profileName, string accountName)
		{
			Bus.AssertWasNotCalled(x => x.SendLocal(null), x => x.IgnoreArguments());
		}

		[Then(
			@"messages should be passed to profile '$profileName' for account '$accountName' for processing: (?<messageNames>([^,]+,?\s*)+)"
			)]
		public void MessagesShouldBePassedToProfile(string profileName, string accountName, string[] messageNames)
		{
			var messagesSentLocally = _sentLocally.OfType<SampleMessage>().Select(x => x.Name).ToArray();
			AssertMessagesWereSent(messagesSentLocally, messageNames, profileName, accountName);
		}

		private static void AssertMessagesWereSent(
			IEnumerable messagesSentLocally,
			IEnumerable<string> expectedMessageNames,
			string profileName,
			string accountName)
		{
			messagesSentLocally.Should(Be.EquivalentTo(expectedMessageNames));
			Bus.GetOutProfileName().Should(Be.EqualTo((ProfileName) profileName));
			Bus.GetOutAccountName().Should(Be.EqualTo(new AccountName(accountName)));
		}

		private static IBus Bus
		{
			get { return ObjectFactory.GetInstance<IBus>(); }
		}
	}

	[Serializable]
	public class SampleMessage : ISagaMessage
	{
		public string Name { get; set; }
		public Guid SagaId { get; set; }
		public byte[] Bytes { get; set; }
	}
}