// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Linq;
using NBehave.Narrator.Framework;
using NServiceBus.Unicast.Transport;
using NUnit.Framework;
using Tp.Integration.Messages;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Messages.ServiceBus;
using Tp.Integration.Plugin.Common.PluginCommand.Embedded;
using Tp.Integration.Plugin.Common.Validation;
using Tp.Integration.Testing.Common;
using Tp.Testing.Common.NBehave;
using Tp.Testing.Common.NUnit;

namespace Tp.PopEmailIntegration.Validation
{
	[TestFixture, ActionSteps]
	public class ValidateProfileFieldsSpecs
	{
		private TransportMock _transportMock;

		[BeforeScenario]
		public void BeforeScenario()
		{
			_transportMock = TransportMock.CreateWithoutStructureMapClear(typeof (ProjectEmailProfile).Assembly,
			                                                              typeof (ProjectEmailProfile).Assembly);
		}

		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void ShouldThrowValidationExceptionIfServerIsEmpty()
		{
			@" Given profile 'profile_1' with no mail server specified
				When trying to save profile 'profile_1'
				Then validation errors for field 'MailServer' should be 'Server should not be empty'
			"
				.Execute();
		}

		[Test]
		public void ShouldThrowValidationExceptionIfPortIsZero()
		{
			@" Given profile 'profile_1' with port equals to 0
				When trying to save profile 'profile_1'
				Then validation errors for field 'Port' should be 'Port should not be empty'
			"
				.Execute();
		}

		[Test]
		public void ShouldThrowValidationExceptionIfLoginIsEmpty()
		{
			@" Given profile 'profile_1' with no login specified
				When trying to save profile 'profile_1'
				Then validation errors for field 'Login' should be 'Login should not be empty'
			"
				.Execute();
		}

		[Test]
		public void ShouldThrowValidationExceptionIfPasswordIsEmpty()
		{
			@" Given profile 'profile_1' with no password specified
				When trying to save profile 'profile_1'
				Then validation errors for field 'Password' should be 'Password should not be empty'
			"
				.Execute();
		}

		[Test]
		public void ShouldThrowValidationExceptionIfRulesIsEmpty()
		{
			@" Given profile 'profile_1' with no rules specified
				When trying to save profile 'profile_1'
				Then validation errors for field 'Rules' should be 'Rules should not be empty'
			"
				.Execute();
		}

		[Test]
		public void ShouldThrowValidationExceptionIfRulesAreInvalid()
		{
			@" Given profile 'profile_1' with rules 'bla bla'
				When trying to save profile 'profile_1'
				Then validation errors for field 'Rules' should be 'Invalid rules format'
			"
				.Execute();
		}

		[Test]
		public void ShouldNotThrowExceptionsForValidProfile()
		{
			@" Given valid profile 'profile_1'
				When trying to save profile 'profile_1'
				Then no validation errors should exist
			"
				.Execute();
		}

		[Given("valid profile '$profileName'")]
		public void CreateValidProfile(string profileName)
		{
			CreateProfileInitialization(Settings, profileName);
		}

		[Given("profile '$profileName' with rules '$rules'")]
		public void CreateProfileWithInvalidRule(string profileName, string rules)
		{
			var settings = Settings;
			settings.Rules = rules;

			CreateProfileInitialization(settings, profileName);
		}

		[Given("profile '$profileName' with no mail server specified")]
		public void CreateProfileWithoutmailServer(string profileName)
		{
			var settings = Settings;
			settings.MailServer = "";

			CreateProfileInitialization(settings, profileName);
		}

		[Given("profile '$profileName' with port equals to 0")]
		public void CreateProfileWithZeroPort(string profileName)
		{
			var settings = Settings;
			settings.Port = 0;

			CreateProfileInitialization(settings, profileName);
		}

		[Given("profile '$profileName' with no login specified")]
		public void CreateProfileWithoutLogin(string profileName)
		{
			var settings = Settings;
			settings.Login = null;

			CreateProfileInitialization(settings, profileName);
		}

		[Given("profile '$profileName' with no password specified")]
		public void CreateProfileWithoutPassword(string profileName)
		{
			var settings = Settings;
			settings.Password = "";

			CreateProfileInitialization(settings, profileName);
		}

		[Given("profile '$profileName' with no rules specified")]
		public void CreateProfileWithoutRules(string profileName)
		{
			var settings = Settings;
			settings.Rules = "";

			CreateProfileInitialization(settings, profileName);
		}

		[When("trying to save profile '$profileName'")]
		public void SaveProfile(string profileName)
		{
		}

		[Then("validation errors for field '$field' should be '$error'")]
		public void GetError(string field, string error)
		{
			var exceptionResponseMsg =
				_transportMock.TpQueue.GetMessages<PluginCommandResponseMessage>().Single(
					x => x.PluginCommandStatus == PluginCommandStatus.Fail);
			var errorCollection1 = exceptionResponseMsg.ResponseData.Deserialize<PluginProfileErrorCollection>();
			var errorCollection = errorCollection1;
			var fieldValidationError = errorCollection.Where(x => x.FieldName == field);

			new[] {error}.Should(Is.SubsetOf(fieldValidationError.Select(x => x.Message).ToArray()));
		}

		[Then("no validation errors should exist")]
		public void CheckThereIsNoErrors()
		{
			_transportMock.TpQueue.GetMessages<PluginCommandResponseMessage>().Where(
				x => x.PluginCommandStatus == PluginCommandStatus.Fail)
				.Should(Be.Empty);
		}

		private void CreateProfileInitialization(ProjectEmailProfile settings, string profileName)
		{
			var profile = new PluginProfileDto
			              	{
			              		Name = profileName,
			              		Settings = settings
			              	};

			var createProfileCmd = new ExecutePluginCommandCommand
			                       	{CommandName = EmbeddedPluginCommands.AddProfile, Arguments = profile.Serialize()};
			_transportMock.HandleMessageFromTp(
				new List<HeaderInfo> {new HeaderInfo {Key = BusExtensions.ACCOUNTNAME_KEY, Value = "Account"}}, createProfileCmd);
		}

		private static ProjectEmailProfile Settings
		{
			get
			{
				return new ProjectEmailProfile
				       	{
				       		Login = "login",
				       		MailServer = "mail.server",
				       		Password = "pass",
				       		Port = 123,
				       		Protocol = "pop3",
				       		Rules = "then attach to project 1 and create request in project 1",
				       		UseSSL = true
				       	};
			}
		}
	}
}