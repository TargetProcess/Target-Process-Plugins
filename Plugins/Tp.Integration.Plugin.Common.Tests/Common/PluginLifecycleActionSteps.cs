// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NBehave.Narrator.Framework;
using NServiceBus;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap;
using Tp.Integration.Messages;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Messages.ServiceBus;
using Tp.Integration.Messages.TargetProcessLifecycle;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Mashup;
using Tp.Integration.Plugin.Common.PluginCommand;
using Tp.Integration.Plugin.Common.PluginCommand.Embedded;
using Tp.Integration.Plugin.Common.PluginLifecycle;
using Tp.Integration.Plugin.Common.Storage.Repositories;
using Tp.Plugin.Core.Attachments;
using Tp.Testing.Common.NUnit;

namespace Tp.Integration.Plugin.Common.Tests.Common
{
	[ActionSteps]
	public class PluginLifecycleActionSteps
	{
		[Given(@"plugin receives '$commandName' command and account '$accountName' with profile '$profileName'")]
		public void ReceivePluginSpecificChangePluginProfileCommand(string commandName, string accountName, string profileName)
		{
			ObjectFactory.GetInstance<PluginContextMock>().AccountName = accountName;
			ObjectFactory.GetInstance<PluginContextMock>().ProfileName = profileName;

			var command = new ExecutePluginCommandCommand
			              	{CommandName = commandName, Arguments = new PluginProfileDto {Name = profileName}.Serialize()};
			ObjectFactory.GetInstance<PluginCommandHandler>().Handle(command);
		}

		[Given("plugin has input queue set to '$pluginInputQueue'")]
		public void SetPluginInputQueue(string pluginInputQueue)
		{
			ObjectFactory.GetInstance<IPluginSettings>().Stub(x => x.PluginInputQueue).Return(pluginInputQueue);
		}

		[Then("PluginInfoMessage should not be published")]
		public void PluginInfoMessageShouldNotBePublished()
		{
			_sentMessages.OfType<PluginInfoMessage>().Should(Is.Empty);
		}

		[Then(@"account '$accountName' should have profiles: (?<profileNames>([^,]+,?\s*)+)")]
		public void AccountShouldHaveProfiles(string accountName, string[] profileNames)
		{
			var account = ObjectFactory.GetInstance<IAccountCollection>().GetOrCreate(accountName);
			account.Profiles.Select(x => x.Name.Value)
				.ToArray().Should(Is.EquivalentTo(profileNames));
			account.Profiles.Where(x => x.GetProfile<SampleJiraProfile>( /*Profile.CreateDefaultProfile().GetType()*/) == null).
				ToArray().Should(Is.Empty,
				                 "Profile property should not be null");
		}

		[Then(@"account '$accountName' should have no profiles")]
		public void AccountShouldHaveNoProfiles(string accountName)
		{
			var accounts = ObjectFactory.GetInstance<IAccountRepository>().GetAll().ToList().FindAll(x => x.Name == accountName);
			foreach (var account in accounts)
			{
				account.Profiles.Should(Is.Empty);
			}
		}

		[When("store string value '$stringValue' in profile '$profileName'")]
		public void StoreValueInProfile(string stringValue, string profileName)
		{
			var profile = PluginSqlPersisterSpecs.GetProfile(profileName);
			profile.Get<string>().ReplaceWith(stringValue);
		}

		[Then("profile '$profileName' should have string value '$stringValue'")]
		public void ProfileShouldHaveStringValue(string profileName, string stringValue)
		{
			var profile = PluginSqlPersisterSpecs.GetProfile(profileName);
			profile.Get<string>().First().Should(Is.EqualTo(stringValue));
		}

		private readonly List<IMessage> _sentMessages = new List<IMessage>();

		[Given("plugin started up")]
		public void StartPlugin()
		{
			_sentMessages.Clear();
			var bus = ObjectFactory.GetInstance<IBus>();
			bus.ResetExpectations();
			NServiceBusMockRegistry.Setup(bus);
			bus.Stub(x => x.Send()).IgnoreArguments().WhenCalled(StoreMessage).Return(null);
			bus.Stub(x => x.SendToUi()).IgnoreArguments().WhenCalled(StoreMessage).Return(null);
			ObjectFactory.GetInstance<PluginContextMock>().PluginName =
				ObjectFactory.GetInstance<IPluginContext>().PluginName.Value;

			ObjectFactory.GetInstance<PluginInitializer>().Init();
		}

		private void StoreMessage(MethodInvocation obj)
		{
			var firstArg = obj.Arguments[0] as object[];
			if (firstArg != null)

				_sentMessages.AddRange(firstArg.OfType<IMessage>());
		}

		[Given("plugin has custom mashup repository")]
		public void RegisterCustomMashupRepository()
		{
			ObjectFactory.Configure(x => x.For<IPluginMashupRepository>().HybridHttpOrThreadLocalScoped().Use
			                             	(MockRepository.GenerateStub<IPluginMashupRepository>()));
		}

		[Given(
			"plugin has mashup '$mashupName' with script '$scriptContent' for placeholder '$placeholderName' registered"
			)]
		public void RegisterScriptForPlugin(string mashupName, string scriptContent, string placeholderName)
		{
			var pluginMashupFile = MockRepository.GenerateStub<PluginMashup>(mashupName, new[] {string.Empty},
			                                                                 new[] {placeholderName});
			pluginMashupFile.Stub(x => x.GetScripts()).Return(new[]
			                                                  	{
			                                                  		new PluginMashupScript
			                                                  			{ScriptContent = scriptContent, FileName = string.Empty}
			                                                  	});
			pluginMashupFile.Stub(x => x.IsValid).Return(true);
			ObjectFactory.GetInstance<IPluginMashupRepository>().Stub(x => x.PluginMashups).Return(new[] {pluginMashupFile});
		}

		[Given("no custom mashup repository specified")]
		public void NoCustomMashupRepositorySpecified()
		{
			ObjectFactory.EjectAllInstancesOf<IPluginMashupRepository>();
			Directory.CreateDirectory(DefaultPluginMashupRepository.PluginMashupDefaultPath);
		}

		[Given("mashups specified in folder:")]
		public void SetupMashups(string mashupFileName, string mashupFileContent, string mashupConfigFileName,
		                         string configContent)
		{
			var mashupDirectory =
				Directory.CreateDirectory(Path.Combine(DefaultPluginMashupRepository.PluginMashupDefaultPath,
				                                       Path.GetFileNameWithoutExtension(mashupFileName)));
			using (
				var configWriter =
					File.CreateText(Path.Combine(mashupDirectory.FullName, mashupConfigFileName))
				)
			{
				configWriter.Write(configContent);
			}

			using (var scriptWriter = File.CreateText(Path.Combine(mashupDirectory.FullName, mashupFileName)))
			{
				scriptWriter.Write(mashupFileContent);
			}
		}

		[Given("plugin has no script registered")]
		public void PluginHasNoScripts()
		{
			ObjectFactory.EjectAllInstancesOf<IPluginMashupRepository>();
		}

		[When("plugin receives TargetProcessStartedMessage")]
		public void ReceiveTargetProcessStartedMessage()
		{
			ObjectFactory.GetInstance<TargetProcessLifecycleMessageHandler>().Handle(new TargetProcessStartedMessage());
		}

		[When("plugin should publish PluginInfoMessage")]
		public void PluginShouldPublishInfo()
		{
			ObjectFactory.GetInstance<IBus>().AssertWasCalled(x => x.Send(Arg<IMessage[]>.Matches(y => IsValid(y))));
		}

		private static bool IsValid(IMessage[] messages)
		{
			messages.Length.Should(Is.EqualTo(1));
			return messages[0] is PluginInfoMessage;
		}

		[Given(@"JiraUrl settings for profile '$profileName' in account '$accountName' has value: '$jiraUrl'")]
		public void SetJiraUrlSetting(string profileName, string accountName, string jiraUrl)
		{
			var profile = new SampleJiraProfile {JiraUrl = jiraUrl};
			ObjectFactory.GetInstance<PluginContextMock>().AccountName = accountName;
			Repository.Add(new ProfileCreationArgs(profileName, profile));
		}

		[Given(@"plugin modifies JiraUrl settings for profile '$profileName' in account '$accountName' with value: '$jiraUrl'"
			)]
		public void ModifyJiraUrlByPlugin(string profileName, string accountName, string jiraUrl)
		{
			var account = ObjectFactory.GetInstance<IAccountRepository>().GetAll().First(x => x.Name == accountName);
			var context = ObjectFactory.GetInstance<PluginContextMock>();

			context.ProfileName = profileName;
			context.AccountName = account.Name;
			ObjectFactory.GetInstance<IBus>().SetIn((ProfileName) profileName);
			ObjectFactory.GetInstance<IBus>().SetIn(account.Name);

			var profile =
				ObjectFactory.GetInstance<IAccountCollection>().GetOrCreate(context.AccountName).Profiles[context.ProfileName].
					GetProfile<SampleJiraProfile>();
			profile.JiraUrl = jiraUrl;

			var updatedProfileDto = new PluginProfileDto {Name = context.ProfileName.Value, Settings = profile};
			ObjectFactory.GetInstance<AddOrUpdateProfileCommand>().Execute(updatedProfileDto.Serialize());
		}

		[Given("file added to account '$accountName' profile '$profileName' file storage")]
		public void AddFileToProfileFileStorage(string accountName, string profileName)
		{
			var profile =
				ObjectFactory.GetInstance<IAccountRepository>().GetAll().First(x => x.Name == accountName).Profiles[profileName];

			var folder = profile.FileStorage.GetFolder();
			using (var fileStream = File.Create(Path.Combine(folder, "1.txt")))
			using (var fileWriter = new StreamWriter(fileStream))
			{
				fileWriter.Write("sample content");
			}
		}

		[When("file '$fileName' added to account '$accountName' profile '$profileName' file storage in the old way")]
		public void AddFileToProfileFileStorageInTheOldWay(string fileName, string accountName, string profileName)
		{
			var folder = ObjectFactory.GetInstance<PluginDataFolder>().Path;

			using (var fileStream = File.Create(Path.Combine(folder, fileName)))
			using (var fileWriter = new StreamWriter(fileStream))
			{
				fileWriter.Write("sample content");
			}
		}

		[Then("$messageCount PluginAccountMessage messages should be published")]
		public void AmountOfPluginAccountMessagesShouldBeSent(int messageCount)
		{
			_sentMessages.OfType<PluginAccountMessageSerialized>().Count().Should(Is.EqualTo(messageCount));
		}

		[Then("empty PluginAccountMessage message should be published")]
		public void EmptyPluginAccountMessagesShouldBeSent()
		{
			_sentMessages.OfType<PluginAccountMessageSerialized>().ForEach(x => x.GetAccounts().Should(Be.Empty));
		}

		private static IProfileCollection Repository
		{
			get { return ObjectFactory.GetInstance<IProfileCollection>(); }
		}

		[Then(
			@"PluginAccountMessage should be published with account '$accountName' and profiles: (?<profileNames>([^,]+,?\s*)+)")
		]
		public void ShouldPublishPluginInfoMessageWithProfiles(string accountName, string[] profileNames)
		{
			var pluginLifecycleMessage =
				_sentMessages.OfType<PluginAccountMessageSerialized>().Where(x => x.GetAccounts().Any(y => y.Name == accountName)).
					Last();
			pluginLifecycleMessage.GetAccounts().First(x => x.Name == accountName).PluginProfiles.Select(x => x.Name.Value).
				ToArray().Should(
					Is.EquivalentTo(profileNames));
		}

		[Then("PluginInfoMessage should be published ")]
		public void PluginInfoMessageShouldBePublished()
		{
			var messages = _sentMessages.OfType<PluginInfoMessage>();
			messages.FirstOrDefault(x => x.Info.Name == ObjectFactory.GetInstance<IPluginContext>().PluginName).Should(
				Be.Not.Null);
		}

		[Then("PluginAccountMessage should be published with account '$accountName' and profile '$profileName'")]
		public void ShouldPublishPluginInfoMessage(string accountName, string profileName)
		{
			var messages = _sentMessages.OfType<PluginAccountMessageSerialized>();
			var message =
				messages.First(
					x =>
					x.GetAccounts().First().Name == accountName &&
					x.GetAccounts().First().PluginProfiles.Any(y => y.Name == profileName));

			message.GetAccounts().First().PluginProfiles.SingleOrDefault(x => x.Name == profileName).Should(Is.Not.Null);

			ObjectFactory.GetInstance<PluginContextMock>().AccountName = accountName;
			ObjectFactory.GetInstance<IProfileCollection>().SingleOrDefault(x => x.Name == profileName).Should(Is.Not.Null);
		}

		[Then("PluginAccountMessage should be published with account '$accountName' and no profiles")]
		public void ShouldPublishPluginInfoMessageWithNoProfiles(string accountName)
		{
			var message = _sentMessages.OfType<PluginAccountMessageSerialized>().Last();
			message.Should(Is.Not.Null);
			message.GetAccounts().First().PluginProfiles.Should(Is.Empty);
		}

		[Then(
			"mashup '$mashupName' with script '$scriptFile' with content '$scriptContent' should be sent to TargetProcess"
			)]
		public void ScriptFileShouldBeSentToTargetProcess(string mashupName, string scriptFile, string scriptContent)
		{
			var bus = ObjectFactory.GetInstance<IBus>();
			var sentScripts = bus.GetCallsMadeOn<IBus, PluginMashupMessage>(x => x.SendToUi()).ToArray();
			var sentMashup = sentScripts.First().First(x => x.MashupName == mashupName);
			sentMashup.PluginMashupScripts.Select(x => x.ScriptContent).ToArray().Contains(
				Convert.ToBase64String(Encoding.ASCII.GetBytes(scriptContent)));
			sentMashup.PluginMashupScripts.Select(x => x.FileName).ToArray().Contains(scriptFile);
			sentMashup.PluginName.Should(Is.EqualTo(ObjectFactory.GetInstance<IPluginContext>().PluginName));
		}


		[Then(
			"mashup '$mashupName' with script '$scriptContent' for placeholder '$placeholderName' should be sent to TargetProcess"
			)]
		public void ScriptShouldBeSentToTargetProcess(string mashupName, string scriptContent, string placeholderName)
		{
			var bus = ObjectFactory.GetInstance<IBus>();
			var sentScripts = bus.GetCallsMadeOn<IBus, PluginMashupMessage>(x => x.SendToUi()).ToArray();
			var sentMashup = sentScripts.First().First(x => x.MashupName == mashupName);
			sentMashup.PluginMashupScripts.Select(x => x.ScriptContent).ToArray().Should(Is.EquivalentTo(new[] {scriptContent}));
			sentMashup.PluginName.Value.Should(Is.EqualTo(ObjectFactory.GetInstance<IPluginContext>().PluginName.Value));
			sentMashup.Placeholders.Should(Is.EquivalentTo(new[] {placeholderName}));
		}

		[Then("no script should be sent to TargetProcess")]
		public void NoScriptShouldBeSentToTargetProcess()
		{
			var bus = ObjectFactory.GetInstance<IBus>();
			var sentScripts = bus.GetCallsMadeOn<IBus, PluginMashupMessage>(x => x.SendToUi()).ToArray();
			sentScripts.Count().Should(Is.EqualTo(0));
		}

		[Then("PluginInfoMessage should be published with plugin input queue '$pluginInputQueue'")]
		public void InfoChangedMessageWithPluginInputQueueShouldBePublished(string pluginInputQueue)
		{
			var message = _sentMessages.OfType<PluginInfoMessage>().Single();
			message.Info.PluginInputQueue.Should(Is.EqualTo(pluginInputQueue));
		}

		[Then("file storage for account '$account' and profile '$profile' should be deleted")]
		public void FileStorageShouldBeRemoved(string account, string profile)
		{
			var folder = Path.Combine(ObjectFactory.GetInstance<PluginDataFolder>().Path, account, profile);
			Directory.Exists(folder).Should(Be.False);
		}

		[Then("file storage for account '$accountName' and profile '$profileName' should return file '$fileName'")]
		public void FileStorageShouldReturnFile(string accountName, string profileName, string fileName)
		{
			File.Exists(AttachmentFolder.GetAttachmentFileFullPath(new FileId(Guid.Parse(fileName)))).Should(Be.True);
		}
	}

	public class PluginContextMock : IPluginContext
	{
		public AccountName AccountName { get; set; }
		public ProfileName ProfileName { get; set; }
		public PluginName PluginName { get; set; }
	}
}