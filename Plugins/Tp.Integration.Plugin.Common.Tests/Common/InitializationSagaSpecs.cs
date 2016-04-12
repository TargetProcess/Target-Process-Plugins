// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Reflection;
using NBehave.Narrator.Framework;
using NServiceBus;
using NServiceBus.Saga;
using NServiceBus.Unicast.Transport;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap;
using Tp.Integration.Messages;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Messages.ServiceBus;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.PluginCommand.Embedded;
using Tp.Integration.Plugin.Common.Storage.Persisters;
using Tp.Integration.Testing.Common;
using Tp.Testing.Common.NBehave;
using Tp.Testing.Common.NUnit;
using ISagaMessage = Tp.Integration.Messages.EntityLifecycle.ISagaMessage;

namespace Tp.Integration.Plugin.Common.Tests.Common
{
	[TestFixture, ActionSteps]
    [Category("PartPlugins1")]
	public class InitializationSagaSpecs
	{
		private TransportMock _transport;
		private IProfileReadonly _profile;

		[Test, Ignore("This test is useful but now is covered by plugins tests, not here. Consider to enable it")]
		public void ShouldMarkProfileAsInitializedUponInitializationCompletion()
		{
			@"Given bug initialization saga defined
				When not initialized profile added
				Then profile should be marked as initialized"
				.Execute();
		}

		[Test, Ignore("This test is useful but now is covered by plugins tests, not here. Consider to enable it")]
		public void WhenInitializationSagaExistThenJustAddedProfileShouldBeNotInitialized()
		{
			@"Given bug initialization saga defined
					And target process is not able to support initialization workflow
				When not initialized profile added
				Then profile should remained not initialized"
				.Execute();
		}

		[Test]
		public void NewProfileShouldBeCreatedAsNotInitializedIfNewProfileInitializationSagaDefined()
		{
			var assemblyScannerRegistry = new AssemblyScannerMockRegistry(typeof (BugInitializationSaga).Assembly);
			ObjectFactory.Initialize(x => x.AddRegistry(assemblyScannerRegistry));
			var profile = new Profile();
			profile.Initialized.Should(Be.False, "profile.Initialized.Should(Be.False)");
		}

		[Test]
		public void DoNotCallInitializationSagaTwice()
		{
			var transportMock = TransportMock.CreateWithoutStructureMapClear(typeof(BugInitializationSaga).Assembly,
																		Assembly.GetExecutingAssembly());

			var profile = transportMock.AddProfile("Profile");
			profile.MarkAsInitialized();
			profile.Save();

			new BugInitializationSaga().Handle(new ProfileAddedMessage());

			BugInitializationSaga.CallCount.Should(Be.EqualTo(0), "BugInitializationSaga.CallCount.Should(Be.EqualTo(0))");
		}

		[Test]
		public void NewProfileShouldBeCreatedAsInitializedIfNewProfileInitializationSagaNotDefined()
		{
			var transportMock = TransportMock.CreateWithoutStructureMapClear(GetType().Assembly,
																		Assembly.GetExecutingAssembly());

			StubPluginMetadataWithNoInitializationSagas();

			var profile = transportMock.AddProfile("Profile");
			profile.MarkAsInitialized();
			profile.Save();

			profile.Initialized.Should(Be.True, "profile.Initialized.Should(Be.True)");
		}

		[Test]
		public void UpdatedProfileShouldBeMarkedAsNotInitializedIfUpdatedProfileInitializationSagaDefined()
		{
			var transportMock = TransportMock.CreateWithoutStructureMapClear(typeof (BugUpdateInitializationSaga).Assembly,
			                                                                 Assembly.GetExecutingAssembly());

			var profile = transportMock.AddProfile("Profile");
			profile.MarkAsInitialized();
			profile.Save();

			BugUpdateInitializationSaga._freezeSaga = true;
			UpdateProfile(transportMock, profile);

			var account = ObjectFactory.GetInstance<IAccountCollection>().GetOrCreate(AccountName.Empty);
			account.Profiles["Profile"].Initialized.Should(Be.EqualTo(false), "account.Profiles[\"Profile\"].Initialized.Should(Be.EqualTo(false))");
		}

		private static void UpdateProfile(TransportMock transportMock, IProfileReadonly profile)
		{
			var addOrUpdateProfileCmd = new ExecutePluginCommandCommand
			                            	{
			                            		CommandName = EmbeddedPluginCommands.AddOrUpdateProfile,
			                            		Arguments = profile.ConvertToDto().Serialize()
			                            	};
			transportMock.HandleMessageFromTp(
				new List<HeaderInfo> {new HeaderInfo {Key = BusExtensions.ACCOUNTNAME_KEY, Value = AccountName.Empty.Value}},
				addOrUpdateProfileCmd);
		}

		[Test]
		public void UpdatedProfileShouldBeMarkedAsInitializedIfUpdatedProfileInitializationSagaNotDefined()
		{
			var transportMock = TransportMock.CreateWithoutStructureMapClear(GetType().Assembly,
			                                                                 Assembly.GetExecutingAssembly());

			StubPluginMetadataWithNoInitializationSagas();

			var profile = transportMock.AddProfile("Profile");
			profile.MarkAsInitialized();
			profile.Save();

			UpdateProfile(transportMock, profile);

			var account = ObjectFactory.GetInstance<IAccountCollection>().GetOrCreate(AccountName.Empty);
			account.Profiles["Profile"].Initialized.Should(Be.EqualTo(true), "account.Profiles[\"Profile\"].Initialized.Should(Be.EqualTo(true))");
		}

		private static void StubPluginMetadataWithNoInitializationSagas()
		{
			var metadata = ObjectFactory.GetInstance<IPluginMetadata>();
			var metadataMock = MockRepository.GenerateStub<IPluginMetadata>();
			metadataMock.Stub(y => y.IsNewProfileInitializable).Return(false);
			metadataMock.Stub(y => y.IsUpdatedProfileInitializable).Return(false);
			metadataMock.Stub(y => y.ProfileType).Return(metadata.ProfileType);
			metadataMock.Stub(y => y.PluginData).Return(metadata.PluginData);
			ObjectFactory.EjectAllInstancesOf<IPluginMetadata>();
			ObjectFactory.Configure(x => x.For<IPluginMetadata>().Singleton().Use(metadataMock));
		}

		[Given("bug initialization saga defined")]
		public void CreateBugInitializationSaga()
		{
			_transport = TransportMock.CreateWithoutStructureMapClear(typeof (BugInitializationSaga).Assembly,
			                                                          Assembly.GetExecutingAssembly());
		}

		[Given("target process is not able to support initialization workflow")]
		public void AddBugInitializationSagaWorkflowSupport()
		{
			ObjectFactory.Configure(x => x.For<TargetProcessState>().Use(new TargetProcessState(true)));
		}

		[When("not initialized profile added")]
		public void ReceiveProfileAddedMessage()
		{
			_profile = _transport.AddProfile("Profile_1");
		}

		[Then("profile should be marked as initialized")]
		public void ProfileShouldBeInitialized()
		{
			_profile.Initialized.Should(Be.True, "_profile.Initialized.Should(Be.True)");
		}

		[Then("profile should remained not initialized")]
		public void ProfileShouldBeNotInitialized()
		{
			_profile.Initialized.Should(Be.False, "_profile.Initialized.Should(Be.False)");
		}

		#region TestSaga

		//Test assembly is scanned for sagas in TransportMock. So this saga should set profile to initialized by default.
		public class BugInitializationSaga : NewProfileInitializationSaga<BugInitializationSaga.BugInitializationSagaData>,
		                                     IHandleMessages<TestBugQueryResult>
		{
			public static int CallCount;
			[Serializable]
			public class BugInitializationSagaData : ISagaEntity
			{
				public Guid Id { get; set; }
				public string Originator { get; set; }
				public string OriginalMessageId { get; set; }
			}

			public override void ConfigureHowToFindSaga()
			{
				ConfigureMapping<TestBugQueryResult>(x => x.Id, y => y.SagaId);
			}

			protected override void OnStartInitialization()
			{
				CallCount++;
				MarkAsComplete();
			}

			public void Handle(TestBugQueryResult message)
			{
				MarkAsComplete();
			}
		}

		public class BugUpdateInitializationSaga :
			UpdatedProfileInitializationSaga<BugInitializationSaga.BugInitializationSagaData>
		{
			public static bool _freezeSaga;

			protected override void OnStartInitialization()
			{
				if (_freezeSaga)
				{
					_freezeSaga = false;
				}
				else
				{
					MarkAsComplete();
				}
			}
		}

		public class TestBugQuery : ITargetProcessMessage
		{
		}

		public class TestBugQueryResult : ISagaMessage
		{
			public Guid SagaId { get; set; }
		}

		internal class TargetProcessState
		{
			private readonly bool _isDown;

			public TargetProcessState(bool isDown)
			{
				_isDown = isDown;
			}

			public bool IsDown
			{
				get { return _isDown; }
			}
		}

		#endregion
	}
}