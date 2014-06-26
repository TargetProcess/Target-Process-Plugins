// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NUnit.Framework;
using Tp.Testing.Common.NBehave;

namespace Tp.Bugzilla.Tests.LegacyProfileConversion
{
	[TestFixture]
    [Category("PartPlugins0")]
	public class ConvertProfileSpecs
	{
		private const string FULL_MAPPING_TEMPLATE =
			@"Given {0}
					And profile name is 'ProfileName'
					And project 'p1' for the first process created
					And user 'tpuser1' created
					And user 'tpuser2' created
					And state 'Open' created
					And state 'Done' created
					And priority 'High' created
					And priority 'Low' created
					And severity 'Important' created
					And severity 'Valuable' created
					And role 'Programmer' created
					And bugzilla url is 'http://host/bugzilla363'
					And sync interval is 15
					And bugzilla login is 'login'
					And bugzilla password is 'password'
					And bugzilla project is 'p1'
					And bugzilla queries are 'bz query'
					And bugzilla assignee role is 'Programmer'
					And bugzilla reporter role is 'QA Engineer'
					And user mapping is:
					|bugzilla|targetprocess|
					|bzuser1|tpuser1|
					|bzuser2|tpuser2|
					And state mapping is:
					|bugzilla|targetprocess|
					|open|Open|
					|completed|Done|
					And priority mapping is:
					|bugzilla|targetprocess|
					|P1|High|
					|P2|Low|
					And severity mapping is:
					|bugzilla|targetprocess|
					|S1|Important|
					|S2|Valuable|
				When convert bugzilla legacy profile 'ProfileName'
				Then {1}'ProfileName' plugin profile should be created
					And legacy profile 'ProfileName' should be disabled
					And bugzilla url should be 'http://host/bugzilla363'
					And bugzilla login should be 'login'
					And bugzilla password should be 'password'
					And bugzilla project should be 'p1'
					And bugzilla queries should be 'bz query'
					And bugzilla assignee role should be 'Programmer'
					And bugzilla reporter role should be 'QA Engineer'
					And mapped users count shoud be 2
					And user mapping should be:
					|bugzilla|targetprocess|
					|bzuser1|tpuser1|
					|bzuser2|tpuser2|
					And mapped states count shoud be 2
					And state mapping should be:
					|bugzilla|targetprocess|
					|open|Open|
					|completed|Done|
					And mapped priorities count shoud be 2
					And priority mapping should be:
					|bugzilla|targetprocess|
					|P1|High|
					|P2|Low|
					And mapped severities count shoud be 2
					And severity mapping should be:
					|bugzilla|targetprocess|
					|S1|Important|
					|S2|Valuable|
					And profile should have tp users: tpuser1, tpuser2
					And profile should have tp states: Open, Done
					And profile should have tp priorities: High, Low
					And profile should have tp severities: Important, Valuable
					And profile should have tp roles: Programmer, QA Engineer
					And profile should have tp projects: p1
					And profile should be initialized";

		[Test]
		public void ConvertProfileWithAllMappingsForAccount()
		{
			string.Format(FULL_MAPPING_TEMPLATE, 
				"create account 'TargetProcessTest.tpondemand.com'", @"plugin 'Bugzilla' should have account 'TargetProcessTest.tpondemand.com'
				                                                     And ")
				.Execute(In.Context<LegacyProfileConverterActionSteps>());
		}

		[Test]
		public void ConvertProfileWithAllMappingsForEmptyAccount()
		{
			string.Format(FULL_MAPPING_TEMPLATE,
				"account is empty", "")
				.Execute(In.Context<LegacyProfileConverterActionSteps>());
		}

		[Test]
		public void ShouldSkipDeletedInactiveUsers()
		{
			@"Given create account 'TargetProcessTest.tpondemand.com'
					And profile name is 'ProfileName'
					And project 'p1' for the first process created
					And bugzilla url is 'http://host/bugzilla363'
					And sync interval is 15
					And bugzilla login is 'login'
					And bugzilla password is 'password'
					And bugzilla project is 'p1'
					And bugzilla queries are 'bz query'
					And user 'tpuser1' created
					And deleted user 'tpuser2' created
					And inactive user 'tpuser3' created
					And user mapping is:
						|bugzilla|targetprocess|
						|bzuser1|tpuser1|
						|bzuser2|tpuser2|
						|bzuser3|tpuser3|
				When convert bugzilla legacy profile 'ProfileName'
				Then user mapping should be:
					|bugzilla|targetprocess|
					|bzuser1|tpuser1|"
				.Execute(In.Context<LegacyProfileConverterActionSteps>());
		}

		[Test]
		public void ShouldNotMapUsersNotInTeam()
		{
			@"Given create account 'TargetProcessTest.tpondemand.com'
					And profile name is 'ProfileName'
					And project 'p1' for the first process created
					And bugzilla url is 'http://host/bugzilla363'
					And sync interval is 15
					And bugzilla login is 'login'
					And bugzilla password is 'password'
					And bugzilla project is 'p1'
					And bugzilla queries are 'bz query'
					And user 'tpuser1' created
					And user 'tpuser2' created
					And user 'tpuser2' removed from project team
					And user mapping is:
						|bugzilla|targetprocess|
						|bzuser1|tpuser1|
						|bzuser2|tpuser2|
				When convert bugzilla legacy profile 'ProfileName'
				Then user mapping should be:
					|bugzilla|targetprocess|
					|bzuser1|tpuser1|
					And mapped users count shoud be 1"
				.Execute(In.Context<LegacyProfileConverterActionSteps>());
		}

		[Test]
		public void StoreExternalDataWhenConvertProfile()
		{
			@"
				Given create account 'TargetProcessTest.tpondemand.com'
					And profile name is 'ProfileName'
					And project 'p1' for the first process created
					And bugzilla url is 'http://host/bugzilla363'
					And sync interval is 15
					And bugzilla login is 'login'
					And bugzilla password is 'password'
					And bugzilla project is 'p1'
					And bugzilla queries are 'bz query'
					And bugzilla bug 'bug1' with external id 1 for project 'p1' stored in legacy profile
					And bugzilla bug 'bug2' with external id 2 for project 'p1' stored in legacy profile
					And bugzilla bug 'bug3' with external id 3 for project 'p1' stored in legacy profile
					And bugzilla bug 'bug2' have comments: comment2
					And bugzilla bug 'bug3' have comments: comment3, comment3-1
					And bugzilla bug 'bug3' have attachments: attachment3
				When convert bugzilla legacy profile 'ProfileName'
				Then plugin 'Bugzilla' should have account 'TargetProcessTest.tpondemand.com'
					And 'ProfileName' plugin profile should be created
					And legacy profile 'ProfileName' should be disabled
					And bugzilla url should be 'http://host/bugzilla363'
					And bugzilla login should be 'login'
					And bugzilla password should be 'password'
					And bugzilla project should be 'p1'
					And bugzilla queries should be 'bz query'
					And profile should have 3 saved bugs
					And bugzilla bug 'bug1' with external id 1 for project 'p1' should be stored in new profile
					And bugzilla bug 'bug2' with external id 2 for project 'p1' should be stored in new profile
					And bugzilla bug 'bug3' with external id 3 for project 'p1' should be stored in new profile
					And bugzilla bug 'bug2' should have comments stored in profile: comment2
					And bugzilla bug 'bug3' should have comments stored in profile: comment3, comment3-1
					And bugzilla bug 'bug3' should have attachments stored in profile: attachment3
					And profile should be initialized

			".Execute(In.Context<LegacyProfileConverterActionSteps>());
		}

		[Test]
		public void DontStoreNonExistingBugs()
		{
			@"
				Given create account 'TargetProcessTest.tpondemand.com'
					And profile name is 'ProfileName'
					And project 'p1' for the first process created
					And bugzilla url is 'http://host/bugzilla363'
					And sync interval is 15
					And bugzilla login is 'login'
					And bugzilla password is 'password'
					And bugzilla project is 'p1'
					And bugzilla queries are 'bz query'
					And bugzilla bug 'bug1' with external id 1 for project 'p1' stored in legacy profile
					And bugzilla bug 'bug2' with external id 2 for project 'p1' stored in legacy profile
					And delete bug 'bug2'
				When convert bugzilla legacy profile 'ProfileName'
				Then plugin 'Bugzilla' should have account 'TargetProcessTest.tpondemand.com'
					And 'ProfileName' plugin profile should be created
					And legacy profile 'ProfileName' should be disabled
					And bugzilla url should be 'http://host/bugzilla363'
					And bugzilla login should be 'login'
					And bugzilla password should be 'password'
					And bugzilla project should be 'p1'
					And bugzilla queries should be 'bz query'
					And profile should have 1 saved bugs
					And bugzilla bug 'bug1' with external id 1 for project 'p1' should be stored in new profile
					And profile should be initialized

			".Execute(In.Context<LegacyProfileConverterActionSteps>());
		}

		[Test]
		public void ShouldImportDuplicatedBugzillaBugExternalReferenceOnce()
		{
			@"
				Given create account 'TargetProcessTest.tpondemand.com'
					And profile name is 'ProfileName'
					And project 'p1' for the first process created
					And bugzilla url is 'http://host/bugzilla363'
					And sync interval is 15
					And bugzilla login is 'login'
					And bugzilla password is 'password'
					And bugzilla project is 'p1'
					And bugzilla queries are 'bz query'
					And bugzilla bug 'bug1' with external id 1 for project 'p1' stored in legacy profile
					And bugzilla bug 'bug2' with external id 2 for project 'p1' stored in legacy profile
					And bugzilla bug 'bug2' with external id 2 for project 'p1' stored in legacy profile
				When convert bugzilla legacy profile 'ProfileName'
				Then plugin 'Bugzilla' should have account 'TargetProcessTest.tpondemand.com'
					And 'ProfileName' plugin profile should be created
					And legacy profile 'ProfileName' should be disabled
					And bugzilla url should be 'http://host/bugzilla363'
					And bugzilla login should be 'login'
					And bugzilla password should be 'password'
					And bugzilla project should be 'p1'
					And bugzilla queries should be 'bz query'
					And profile should have 2 saved bugs
					And bugzilla bug 'bug1' with external id 1 for project 'p1' should be stored in new profile
					And bugzilla bug 'bug2' with external id 2 for project 'p1' should be stored in new profile
					And profile should be initialized

			".Execute(In.Context<LegacyProfileConverterActionSteps>());
		}
	}
}