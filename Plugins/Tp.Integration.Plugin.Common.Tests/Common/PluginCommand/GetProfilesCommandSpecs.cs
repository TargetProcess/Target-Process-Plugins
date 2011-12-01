// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NBehave.Narrator.Framework;
using NUnit.Framework;
using StructureMap;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.PluginCommand.Embedded;
using Tp.Testing.Common.NBehave;
using Tp.Testing.Common.NUnit;

namespace Tp.Integration.Plugin.Common.Tests.Common.PluginCommand
{
	[TestFixture, ActionSteps]
	public class GetProfilesCommandSpecs
	{
		private PluginCommandResponseMessage _response;

		[SetUp]
		public void SetUp()
		{
			ObjectFactory.Initialize(x => x.AddRegistry<PluginStorageWithInMemoryPersisterMockRegistry>());
		}

		[Test]
		public void ShouldRetriveAllProfiles()
		{
			@"Given account 'Account' has profiles: Profile1,Profile2
				When GetProfiles command received for acccount 'Account'
				Then the following serialized profiles should be returned: Profile1,Profile2"
				.Execute();
		}

		[Test]
		public void ShouldRetrieveProfileByName()
		{
			@"Given account 'Account' has profiles: Profile1,Profile2
				When GetProfile command received for acccount 'Account' for profile 'Profile1'
				Then the following serialized profiles should be returned: Profile1
					And the following serialized profiles should not be returned: Profile2"
				.Execute();
		}

		[Given(@"account '$accountName' has profiles: (?<profileNames>([^,]+,?\s*)+)")]
		public void SetProfilesForAccount(string accountName, string[] profileNames)
		{
			var account = ObjectFactory.GetInstance<IAccountCollection>().GetOrCreate(accountName);
			foreach(var profileName in profileNames)
			{
				account.Profiles.Add(new ProfileCreationArgs(profileName, new object()));
			}
		}

		[When("GetProfiles command received for acccount '$accountName'")]
		public void GetProfilesCommandReceived(string accountName)
		{
			ObjectFactory.GetInstance<PluginContextMock>().AccountName = accountName;
			_response = ObjectFactory.GetInstance<GetProfilesCommand>().Execute(string.Empty);
		}

		[When("GetProfile command received for acccount '$accountName' for profile '$profileName'")]
		public void GetProfileCommandReceived(string accountName, string profileName)
		{
			ObjectFactory.GetInstance<PluginContextMock>().AccountName = accountName;
			_response = ObjectFactory.GetInstance<GetProfileCommand>().Execute(profileName);
		}

		[Then(@"the following serialized profiles should be returned: (?<profileNames>([^,]+,?\s*)+)")]
		public void ProfilesShouldBeReturned(string[] profileNames)
		{
			_response.PluginCommandStatus.Should(Be.EqualTo(PluginCommandStatus.Succeed));
			foreach (var profileName in profileNames)
			{
				_response.ResponseData.Should(Be.StringContaining(profileName));
			}
		}

		[Then(@"the following serialized profiles should not be returned: (?<profileNames>([^,]+,?\s*)+)")]
		public void ProfilesShouldNotBeReturned(string[] profileNames)
		{
			foreach (var profileName in profileNames)
			{
				_response.ResponseData.Should(Be.Not.StringContaining(profileName));
			}
		}
	}
}