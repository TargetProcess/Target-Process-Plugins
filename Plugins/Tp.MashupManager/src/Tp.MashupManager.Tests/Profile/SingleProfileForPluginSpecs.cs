// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NBehave.Narrator.Framework;
using NUnit.Framework;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Plugin.Common.PluginCommand.Embedded;
using Tp.MashupManager.CustomCommands;
using Tp.Testing.Common.NBehave;
using Tp.Testing.Common.NUnit;
using log4net.Core;

namespace Tp.MashupManager.Tests.Profile
{
	[ActionSteps, TestFixture]
	public class SingleProfileForPluginSpecs : MashupManagerTestBase
	{
		[Test]
		public void ShouldReturnEmptyProfileIfNoExists()
		{
			@"
				Given no profiles created
				When handle GetProfileInfoCommand command
				Then empty profile should be returned
			".Execute();
		}

		[Test]
		public void ShouldReturnProfileIfItIsSingleCreated()
		{
			@"
				Given profile created
					And profile mashups are: mashup1, mashup2
				When handle GetProfileInfoCommand command
				Then returned profile should have following mashups: mashup1, mashup2
			"
				.Execute();
		}

		[Test]
		public void ShouldReturnFirstProfileIfThereAreFewProfiles()
		{
			@"
				Given profile 'profile1' created
					And profile 'profile1' mashups are: mashup1
					And profile 'profile2' created
				When handle GetProfileInfoCommand command
				Then returned profile 'profile1' should have following mashups: mashup1
					And error message 'There are more than one profile for Mashup Manager plugin' should be looged
			"
				.Execute();
		}

		[When("handle GetProfileInfoCommand command")]
		public void HandleGetProfileInfoCommand()
		{
			var command = new GetProfileInfoCommand();
			_response = command.Execute(null);
		}

		[Then("empty profile should be returned")]
		public void EmptyProfileShouldBeReturned()
		{
			CheckReturnedProfile(new PluginProfileDto {Name = string.Empty, Settings = new MashupManagerProfile()});
		}

		[Then(@"returned profile should have following mashups: (?<mashupNames>([^,]+,?\s*)+)")]
		public void CheckMashupNamesOfReturnedProfile(string[] mashupNames)
		{
			CheckMashupNamesOfReturnedProfile(_defaulrProfileName, mashupNames);
		}

		[Then(@"returned profile '$profileName' should have following mashups: (?<mashupNames>([^,]+,?\s*)+)")]
		public void CheckMashupNamesOfReturnedProfile(string profileName, string[] mashupNames)
		{
			var mashups = new MashupNames();
			mashups.AddRange(mashupNames);
			
			CheckReturnedProfile(new PluginProfileDto
			                     	{
			                     		Name = profileName, 
										Settings = new MashupManagerProfile {MashupNames = mashups}
			                     	});
		}

		[Then("error message '$errorMessage' should be looged")]
		public void CheckLoggedError(string errorMessage)
		{
			Logger.Messages[Level.Error].Messages.Should(Contains.Item(errorMessage));
		}

		private void CheckReturnedProfile(PluginProfileDto profileDto)
		{
			_response.ResponseData.Should(Be.EqualTo(profileDto.Serialize()));
		}
	}
}
