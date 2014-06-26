// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.IO;
using NServiceBus;
using NUnit.Framework;
using StructureMap;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Logging;
using Tp.Integration.Plugin.Common.PluginCommand.Embedded;
using Tp.Integration.Plugin.Common.PluginLifecycle;
using Tp.Testing.Common.NBehave;

namespace Tp.Integration.Plugin.Common.Tests.Common
{
	[TestFixture]
    [Category("PartPlugins1")]
	public class PluginLifecycleSpecs
	{
		[SetUp]
		public void Init()
		{
			ObjectFactory.Initialize(x =>
				{
					x.AddRegistry<PluginStorageWithInMemoryPersisterMockRegistry>();
					x.For<ILogManager>().HybridHttpOrThreadLocalScoped().Use<TpLogManager>();
					x.Forward<ILogManager, ILogProvider>();
				});
			
			if (Directory.Exists(DefaultPluginMashupRepository.PluginMashupDefaultPath))
			{
				Directory.Delete(DefaultPluginMashupRepository.PluginMashupDefaultPath, true);
			}

			//For command handling correct work. It is better to remove MockRegistry with TransportMock.
			Configure.With(typeof (AddProfileCommand).Assembly.GetTypes());
		}

		[Test]
		public void ShouldPublishPluginInfoUponStart()
		{
			@"Given profile 'Profile1_1' for account 'Account1' created
					And profile 'Profile1_2' for account 'Account1' created
					And profile 'Profile2_1' for account 'Account2' created
				When plugin started up
				Then PluginInfoMessage should be published
					And 1 PluginAccountMessage messages should be published
					And PluginAccountMessage should be published with account 'Account1' and profiles: Profile1_1, Profile1_2
					And PluginAccountMessage should be published with account 'Account2' and profiles: Profile2_1"
				.Execute(In.Context<PluginLifecycleActionSteps>().And<ProfileStorageSpecs>());
		}

		[Test]
		public void ShouldPublishPluginInfoMessageUponTargetProcessStartUp()
		{
			@"
				Given plugin started up
				When plugin receives TargetProcessStartedMessage
				Then PluginInfoMessage should be published
					And empty PluginAccountMessage message should be published"
				.Execute(In.Context<PluginLifecycleActionSteps>().And<ProfileStorageSpecs>());
		}

		[Test]
		public void ShouldPublishPluginInfoMessagesWithProfileDataUponTargetProcessStartUp()
		{
			@"
				Given profile 'Profile' for account 'Account' created
					And plugin started up
				When plugin receives TargetProcessStartedMessage
				Then PluginInfoMessage should be published 
					And PluginAccountMessage should be published with account 'Account' and profile 'Profile'"
				.Execute(In.Context<PluginLifecycleActionSteps>().And<ProfileStorageSpecs>());
		}

		[Test]
		public void ShouldPublishPluginInfoMessageUponProfileChanging()
		{
			@"Given account 'Account' has profile 'Profile' created
					And plugin started up
				When plugin modifies JiraUrl settings for profile 'Profile' in account 'Account' with value: 'http://jiraurl1'
				Then PluginAccountMessage should be published with account 'Account' and profile 'Profile'"
				.Execute(In.Context<PluginLifecycleActionSteps>().And<ProfileStorageSpecs>());
		}

		[Test]
		public void ShouldPublishPluginInfoMessageWhenUpdatedPluginInfoReceived()
		{
			@"Given profile 'Profile' for account 'Account' created
					And store string value 'stringValue' in profile 'Profile'
					And plugin started up
				When plugin receives 'AddProfile' command and account 'Account' with profile 'Profile1'
				Then account 'Account' should have profiles: Profile, Profile1
					And PluginAccountMessage should be published with account 'Account' and profile 'Profile'
					And PluginAccountMessage should be published with account 'Account' and profile 'Profile1'
					And profile 'Profile' should have string value 'stringValue'"
				.Execute(In.Context<PluginLifecycleActionSteps>().And<ProfileStorageSpecs>());
		}

		[Test]
		public void ShouldPublishPluginInfoMessageWhenUpdatedPluginInfoWithNoProfilesReceived()
		{
			@"Given profile 'Profile' for account 'Account' created
					And plugin started up
				When plugin receives 'DeleteProfile' command and account 'Account' with profile 'Profile'
				Then PluginAccountMessage should be published with account 'Account' and no profiles"
				.Execute(In.Context<PluginLifecycleActionSteps>().And<ProfileStorageSpecs>());
		}

		[Test]
		public void ShouldRemoveProfileForCorrectPlugin()
		{
			@"Given profile 'Profile1' for account 'Account' created
					And profile 'Profile2' for account 'Account' created
					And plugin started up
				When plugin receives 'DeleteProfile' command and account 'Account' with profile 'Profile2'
				Then PluginAccountMessage should be published with account 'Account' and profile 'Profile1'
						And file storage for account 'Account' and profile 'Profile1' should be deleted
						And account 'Account' should have profiles: Profile1"
				.Execute(In.Context<PluginLifecycleActionSteps>().And<ProfileStorageSpecs>());
		}

		[Test]
		public void ShouldRemoveFileStorageWhenRemovingPluginProfile()
		{
			@"Given profile 'Profile1' for account 'Account' created
					And plugin started up
					And file added to account 'Account' profile 'Profile1' file storage
				When plugin receives 'DeleteProfile' command and account 'Account' with profile 'Profile1'
				Then file storage for account 'Account' and profile 'Profile1' should be deleted"
				.Execute(In.Context<PluginLifecycleActionSteps>().And<ProfileStorageSpecs>());
		}

		[Test]
		public void ShouldBeAbleToRetriveFileStorageInTheOldWay()
		{
			@"Given profile 'Profile1' for account 'Account' created
					And plugin started up
				When file '21EC2020-3AEA-1069-A2DD-08002B30309D' added to account 'Account' profile 'Profile1' file storage in the old way
				Then file storage for account 'Account' and profile 'Profile1' should return file '21EC2020-3AEA-1069-A2DD-08002B30309D'"
				.Execute(In.Context<PluginLifecycleActionSteps>().And<ProfileStorageSpecs>());
		}

		[Test]
		public void ShouldSendScriptsToTargetProcessUponStartWhenCustomRepositorySpecified()
		{
			@"Given profile 'Profile1_1' for account 'Account1' created
					And plugin has custom mashup repository
					And plugin has mashup 'mashup1' with script 'alert(1)' for placeholder 'placeholder1' registered
				When plugin started up
				Then mashup 'mashup1' with script 'alert(1)' for placeholder 'placeholder1' should be sent to TargetProcess"
				.Execute(In.Context<PluginLifecycleActionSteps>().And<ProfileStorageSpecs>());
		}

		[Test]
		public void ShouldSendScriptsToTargetProcessWhenNoCustomRepositorySpecified()
		{
			@"Given profile 'Profile1_1' for account 'Account1' created
					And no custom mashup repository specified
					And mashups specified in folder:
					|MashupFileName	|MashupFileContent|	MashupConfigFileName|	ConfigContent|
					|File1.js|alert(1)|File1.cfg|Placeholders:placeholder1|
					|File2.js|alert(2)|File2.cfg|Placeholders:placeholder2|
				When plugin started up
				Then mashup 'File1' with script '.\File1.js' with content 'alert(1)' should be sent to TargetProcess
					And mashup 'File1' with script '.\File1.cfg' with content 'Placeholders:placeholder1' should be sent to TargetProcess
					And mashup 'File2' with script '.\File2.js' with content 'alert(2)' should be sent to TargetProcess
					And mashup 'File2' with script '.\File2.cfg' with content 'Placeholders:placeholder1' should be sent to TargetProcess"
				.Execute(In.Context<PluginLifecycleActionSteps>().And<ProfileStorageSpecs>());
		}

		[Test]
		public void ShouldNotSendMessagesIfNoScriptsDefined()
		{
			@"Given profile 'Profile1_1' for account 'Account1' created
					And plugin has no script registered
				When plugin started up
				Then no script should be sent to TargetProcess"
				.Execute(In.Context<PluginLifecycleActionSteps>().And<ProfileStorageSpecs>());
		}

		[Test]
		public void ShouldSendPluginQueueNameWithPluginInfo()
		{
			@"
				Given plugin has input queue set to 'pluginInputQueue'
				When plugin started up
				Then PluginInfoMessage should be published with plugin input queue 'pluginInputQueue'"
				.Execute(In.Context<PluginLifecycleActionSteps>().And<ProfileStorageSpecs>());
		}
	}
}