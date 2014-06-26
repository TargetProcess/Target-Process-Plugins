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
using Tp.Integration.Messages;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Messages.ServiceBus;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.PluginCommand.Embedded;
using Tp.Integration.Plugin.Common.Validation;
using Tp.Integration.Testing.Common;
using Tp.Testing.Common.NBehave;
using Tp.Testing.Common.NUnit;

namespace Tp.Integration.Plugin.Common.Tests.Common.PluginCommand
{
	[TestFixture, ActionSteps]
    [Category("PartPlugins1")]
	public class WhenAddANewProfileSpecs
	{
		private TransportMock _transportMock;

		[BeforeScenario]
		public void BeforeScenario()
		{
			_transportMock = TransportMock.CreateWithoutStructureMapClear(typeof (WhenAddANewProfileSpecs).Assembly,
			                                                              typeof (WhenAddANewProfileSpecs).Assembly);
		}

		[Test]
		public void ShouldHandleEmptyProfileName()
		{
			@"Given no profile name provided
				When trying to add a profile with name ' ' for account 'Account'
				Then validation errors for field 'Name' should be 'Profile Name is required'"
				.Execute();
		}

		[Test]
		public void ProfileNamesShouldBeUniqueWithinAccount()
		{
			@"Given profile with name 'ProfileName' for account 'Account'
				When trying to add a profile with name 'ProfileName' for account 'Account'
				Then validation errors for field 'Name' should be 'Profile name should be unique for plugin'"
				.Execute();
		}

		[Test]
		public void ProfileNamesAreCaseInsensitive()
		{
			@"Given profile with name 'PROFILENAME' for account 'Account'
				When trying to add a profile with name 'profilename' for account 'Account'
				Then validation errors for field 'Name' should be 'Profile name should be unique for plugin'"
				.Execute();
		}

		[Test]
		public void ProfileNameShouldContainAllowedSymbols()
		{
			@"Given profile with name 'ProfileName 1' for account 'Account'
				When adding a profile with name 'ProfileName_2' for account 'Account'
				And adding a profile with name 'ProfileName-3' for account 'Account'
				And adding a profile with name 'Prof-ile - Name_4 _' for account 'Account'
				Then account 'Account' should contain profiles:ProfileName 1,ProfileName_2,ProfileName-3,Prof-ile - Name_4 _"
				.Execute();
		}

		[Test]
		public void ProfileNameShouldNotContainInvalidSymbols()
		{
			@"Given going to add profile
				When trying to add a profile with name 'invalid*profile_name' for account 'Account'
				Then validation errors for field 'Name' should be 'You can only use letters, numbers, space, dash and underscore symbol in Profile Name'"
				.Execute();
		}
		
		[Test]
		public void ShouldUpdateExistingProfile()
		{
			@"Given profile with name 'ProfileName' for account 'Account'
					And Profile 'ProfileName' has StringValue 'string value' for account 'Account'
					And Profile 'ProfileName' for account 'Account' marked as Initialized
				When updating StringValue of profile 'ProfileName' with value 'new string value' for account 'Account'
				Then profile 'ProfileName' should have StringValue 'new string value' for account 'Account'"
				.Execute();
		}

		[Test]
		public void ShouldAddNewProfileIfNoSuchProfile()
		{
			@"Given profile with name 'ProfileName1' for account 'Account'
				When updating StringValue of profile 'ProfileName2' with value 'new string value' for account 'Account'
				Then profile 'ProfileName2' should have StringValue 'new string value' for account 'Account'"
				.Execute();
		}

		[Test]
		public void ShouldSaveProfileWhenCommandReceived()
		{
			@"Given profile with name 'ProfileName1' for account 'Account'
				When adding a profile with name 'ProfileName2' for account 'Account'
				Then account 'Account' should contain profiles:ProfileName1,ProfileName2"
				.Execute();
		}

		[Given("no profile name provided")]
		public void GivenNoProfileName()
		{
		}

		[Given("going to add profile")]
		public void GoingToCreateProfile()
		{
		}

		[Given(@"Profile '$profileName' for account '$accountName' marked as Initialized")]
		public void MarkProfileAsInitialized(string profileName, string accountName)
		{
			var profile = ObjectFactory.GetInstance<IAccountCollection>().GetOrCreate(accountName).Profiles.First(x => x.Name == profileName);
			profile.MarkAsInitialized();
			profile.Save();
		}

		[Given("Profile '$profileName' has StringValue '$stringValue' for account '$accountName'")]
		[When("updating StringValue of profile '$profileName' with value '$stringValue' for account '$accountName'")]
		public void SetStringValue(string profileName, string stringValue, string accountName)
		{
			var profileUpdated = new PluginProfileDto
			                     	{Name = profileName, Settings = new SampleProfileSerialized {StringValue = stringValue}};
			var addOrUpdateProfileCmd = new ExecutePluginCommandCommand { CommandName = EmbeddedPluginCommands.AddOrUpdateProfile, Arguments = profileUpdated.Serialize() };
			_transportMock.HandleMessageFromTp(
				new List<HeaderInfo> {new HeaderInfo {Key = BusExtensions.ACCOUNTNAME_KEY, Value = accountName}},
				addOrUpdateProfileCmd);
		}

		[Given("profile with name '$profileName' for account '$accountName'")]
		[When("trying to add a profile with name '$profileName' for account '$accountName'")]
		[When("adding a profile with name '$profileName' for account '$accountName'")]
		public void CreateProfile(string profileName, string accountName)
		{
			var profileDto = new PluginProfileDto {Name = profileName, Settings = new SampleProfileSerialized()};
			var createProfileCmd = new ExecutePluginCommandCommand { CommandName = EmbeddedPluginCommands.AddProfile, Arguments = profileDto.Serialize() };

			_transportMock.HandleMessageFromTp(
				new List<HeaderInfo> {new HeaderInfo {Key = BusExtensions.ACCOUNTNAME_KEY, Value = accountName}}, createProfileCmd);

			var profile = ObjectFactory.GetInstance<IAccountCollection>().GetOrCreate(accountName).Profiles[profileName];
			profile.MarkAsInitialized();
			profile.Save();
		}

		[Then("validation errors for field '$fieldName' should be '$errorMsg'")]
		public void FieldValidationErrorShouldBe(string fieldName, string errorMsg)
		{
			var exceptionResponseMsg =
				_transportMock.TpQueue.GetMessages<PluginCommandResponseMessage>().Single(
					x => x.PluginCommandStatus == PluginCommandStatus.Fail);
			var errorCollection = exceptionResponseMsg.ResponseData.Deserialize<PluginProfileErrorCollection>();
			var fieldValidationError = errorCollection.Where(x => x.FieldName == fieldName);

			new[] { errorMsg }.Should(Be.SubsetOf(fieldValidationError.Select(x => x.Message).ToArray()));
		}

		[Then("profile '$profileName' should have StringValue '$stringValue' for account '$accountName'")]
		public void ProfileShouldHaveStringValue(string profileName, string stringValue, string accountName)
		{
			var profile = ObjectFactory.GetInstance<IAccountCollection>().GetOrCreate(accountName).Profiles[profileName];
			((SampleProfileSerialized) profile.ConvertToDto().Settings).StringValue.Should(Be.EqualTo(stringValue));
		}

		[Then(@"account '$accountName' should contain profiles:(?<profileNames>([^,]+,?\s*)+)")]
		public void AccountShouldContainProfiles(string accountName, string[] profileNames)
		{
			ObjectFactory.GetInstance<IAccountCollection>().GetOrCreate(accountName).Profiles.Select(x => x.Name.Value).Should(
				Be.EquivalentTo(profileNames));
		}
	}
}