// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NUnit.Framework;
using Tp.Testing.Common.NBehave;

namespace Tp.PopEmailIntegration.LegacyProfileConversion
{
	[TestFixture]
    [Category("PartPlugins0")]
	public class LegacyProfileConverterSpecs
	{
		[Test]
		public void ShouldConvertAllProjectEmailSettingsIntoProjectEmailPluginProfiles()
		{
			@"Given project 'Pr' created
					And account name is 'Account'
					And project 'Pr' InboundMailCreateRequests email setting is set to true
					And project 'Pr' IsInboundMailEnabled email setting is set to true
					And project 'Pr' InboundMailReplyAddress is set to 'address@address.com'
					And project 'Pr' InboundMailAutoCheck is set to true
					And project 'Pr' InboundMailServer is set to 'company.pop3.server'
					And project 'Pr' InboundMailPort is set to 123
					And project 'Pr' InboundMailUseSSL is set to false
					And project 'Pr' InboundMailLogin is set to 'login'
					And project 'Pr' InboundMailPassword is set to 'password'
					And project 'Pr' InboundMailProtocol is set to 'POP3'
				When email settings from Target Process converted to e-mail plugin profile
				Then plugin 'Project Email Integration' should have account 'Account'
					And 1 plugin profile should be created
					And InboundMailProtocol email plugin profile should be 'POP3'
					And InboundMailServer email plugin profile should be 'company.pop3.server'
					And InboundMailLogin email plugin profile should be 'login'
					And InboundMailPort email plugin profile should be 123
					And InboundMailUseSSL email plugin profile should be false
					And InboundMailPassword email plugin profile should be 'password'
					And SyncInterval email plugin profile should be 5'
					And email plugin profile should have exact rule : 'then attach to project ProjectId and create request in project ProjectId' where ProjectId is id of project 'Pr'"
				.Execute(In.Context<LegacyProfileConverterActionSteps>());
		}

		[Test]
		public void ShouldMigrateProfilesForProjectsWithEmailIntegrationEnabledOnly()
		{
			@"Given project 'Pr' created
					And account name is 'Account'
					And project 'Pr' IsInboundMailEnabled email setting is set to false
				When email settings from Target Process converted to e-mail plugin profile
				Then 0 plugin profile should be created"
				.Execute(In.Context<LegacyProfileConverterActionSteps>());
		}

		[Test]
		public void ShouldAddProfilesWhenConverterRunTheSecondTime()
		{
			@"Given project 'Pr' created
					And account name is 'Account'
					And project 'Pr' IsInboundMailEnabled email setting is set to true
					And email settings from Target Process converted to e-mail plugin profile
					And email settings from Target Process converted to e-mail plugin profile
				When email settings from Target Process converted to e-mail plugin profile
				Then 3 plugin profile should be created
					And plugin profile names should be unique"
				.Execute(In.Context<LegacyProfileConverterActionSteps>());
		}

		[Test]
		public void ShouldMigrateUsersAndProjectsToProfile()
		{
			@"Given project 'Pr' created
					And account name is 'Account'
					And user 'User' created
					And requester 'Requester' created
					And project 'Pr' IsInboundMailEnabled email setting is set to true
				When email settings from Target Process converted to e-mail plugin profile
				Then 1 plugin profile should be created
					And profile storage should contain user 'User'
					And profile storage should contain requester 'Requester'
					And profile storage should contain project 'Pr'"
				.Execute(In.Context<LegacyProfileConverterActionSteps>());
		}

		[Test]
		public void ShouldMigrateUidsToProfile()
		{
			@"Given project 'Pr' created
					And project 'Pr' InboundMailServer is set to 'company.pop3.server'
					And project 'Pr' InboundMailLogin is set to 'login'
					And project 'Pr' IsInboundMailEnabled email setting is set to true
					And account name is 'Account'
					And Message with uid 'uid1' for server 'company.pop3.server' and login 'login' exists in tp
					And Message with uid 'uid2' for server 'company.pop3.server' and login 'otherlogin' exists in tp
				When email settings from Target Process converted to e-mail plugin profile
				Then 1 plugin profile should be created
					And profile storage should contain messageUid with uid 'uid1' for server 'company.pop3.server' and login 'login'
					And profile storage should not contain messageUid with uid 'uid2'"
				.Execute(In.Context<LegacyProfileConverterActionSteps>());
		}

		[Test]
		public void ShouldUseTpDatabaseAsPluginWhenNoPluginDbProvided()
		{
			@"Given project 'Pr' created
					And account name is 'Account'
					And project 'Pr' IsInboundMailEnabled email setting is set to true
				When email settings from Target Process converted to e-mail plugin profile without Plugin DB specified
				Then 1 plugin profile should be created"
				.Execute(In.Context<LegacyProfileConverterActionSteps>());
		}

		[Test]
		public void ShouldMigrateDataFromBindEmailToProjectPlugin()
		{
			@"Given project 'Pr' created
					And account name is 'Account'
					And project 'Pr' IsInboundMailEnabled email setting is set to true
					And bind email plugin has active profile 'Profile1'
					And bind email plugin profile 'Profile1' has key 'Pr' and value 'Jira1,Jira2'
					And bind email plugin has active profile 'Profile2'
					And bind email plugin profile 'Profile2' has key 'Pr' and value 'Jira4'
					And bind email plugin has inactive profile 'Profile3'
					And bind email plugin profile 'Profile3' has key 'Pr' and value 'Jira5'
				When email settings from Target Process converted to e-mail plugin profile
				Then plugin 'Project Email Integration' should have account 'Account'
					And 1 plugin profile should be created
					And email plugin profile should contains rule : 'when subject contains 'Jira1,Jira2,Jira4' then attach to project ProjectId' where ProjectId is id of project 'Pr'
					And email plugin profile should contains rule : 'then attach to project ProjectId' where ProjectId is id of project 'Pr'"
				.Execute(In.Context<LegacyProfileConverterActionSteps>());
		}

		[Test]
		public void ShouldMigrateBinderProfilesToEachPopProfile()
		{
			@"Given project 'Pr1' created
					And project 'Pr2' created
					And account name is 'Account'
					And project 'Pr1' IsInboundMailEnabled email setting is set to true
					And project 'Pr1' InboundMailCreateRequests email setting is set to true
					And project 'Pr2' IsInboundMailEnabled email setting is set to false
					And bind email plugin has active profile 'Profile1'
					And bind email plugin profile 'Profile1' has key 'Pr2' and value 'Jira1'
				When email settings from Target Process converted to e-mail plugin profile
				Then plugin 'Project Email Integration' should have account 'Account'
					And 1 plugin profile should be created
					And email plugin profile should contains rule : 'when subject contains 'Jira1' then attach to project ProjectId and create request in project ProjectId' where ProjectId is id of project 'Pr2'
					And email plugin profile should contains rule : 'then attach to project ProjectId' where ProjectId is id of project 'Pr1'"
				.Execute(In.Context<LegacyProfileConverterActionSteps>());
		}

		[Test]
		public void ShouldMergeProfilesIfMappedToTheSameInbox()
		{
			@"Given project 'Pr1' created
					And project 'Pr2' created
					And account name is 'Account'
					And project 'Pr1' InboundMailServer is set to 'company.pop3.server'
					And project 'Pr1' InboundMailLogin is set to 'login'
					And project 'Pr1' IsInboundMailEnabled email setting is set to true
					And project 'Pr1' InboundMailCreateRequests email setting is set to true
					And project 'Pr2' InboundMailServer is set to 'company.pop3.server'
					And project 'Pr2' InboundMailLogin is set to 'login'
					And project 'Pr2' IsInboundMailEnabled email setting is set to true
					And project 'Pr2' InboundMailCreateRequests email setting is set to false
					And bind email plugin has active profile 'Profile1'
					And bind email plugin profile 'Profile1' has key 'Pr2' and value 'Jira1'
				When email settings from Target Process converted to e-mail plugin profile
				Then plugin 'Project Email Integration' should have account 'Account'
					And 1 plugin profile should be created
					And email plugin profile should contains rule : 'when subject contains 'Jira1' then attach to project ProjectId and create request in project ProjectId' where ProjectId is id of project 'Pr2'
					And email plugin profile should contains rule : 'then attach to project ProjectId and create request in project ProjectId' where ProjectId is id of project 'Pr1'
					And email plugin profile should contains rule : 'then attach to project ProjectId' where ProjectId is id of project 'Pr2'"
				.Execute(In.Context<LegacyProfileConverterActionSteps>());
		}
	}
}