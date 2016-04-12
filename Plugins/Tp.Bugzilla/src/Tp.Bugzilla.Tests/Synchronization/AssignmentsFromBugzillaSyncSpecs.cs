// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Linq;
using NBehave.Narrator.Framework;
using NUnit.Framework;
using Tp.Core;
using Tp.Integration.Common;
using Tp.Integration.Plugin.Common.Mapping;
using Tp.Testing.Common.NBehave;
using Tp.Testing.Common.NUnit;

namespace Tp.Bugzilla.Tests.Synchronization
{
	[TestFixture]
	[ActionSteps]
    [Category("PartPlugins0")]
	public class AssignmentsFromBugzillaSyncSpecs : BugzillaTestBase
	{
		[Test]
		public void ShouldCreateAssignmentByEmail()
		{
			@"
				Given Role 'Developer' created in TargetProcess
					And Role 'QA Engineer' created in TargetProcess
					And Role 'Team Lead' created in TargetProcess
					And following users created in TargetProcess:
					|userLogin|userEmail|
					|Johnson|Johnson@mail.com|
					|Lansie|Lansie@mail.com|
					|Dowson|Dowson@mail.com|
					And bugzilla profile with default roles mapping created
					
					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1'
					And bug 1 has reporter 'Dowson@mail.com'
					And bug 1 has assignee 'Lansie@mail.com'
				When synchronizing bugzilla bugs
				Then bugs with following names should be created in TargetProcess: bug1
					And developer 'Lansie' should be newly assigned on TargetProcess bug with name 'bug1'
					And QA 'Dowson' should be newly assigned on TargetProcess bug with name 'bug1'
					And import should be completed
			"
				.Execute(In.Context<BugSyncActionSteps>().And<AssignmentsFromBugzillaSyncSpecs>());
		}

		[Test]
		public void ShouldNotCreateEmptyAssignments()
		{
			@"
				Given Role 'Developer' created in TargetProcess
					And Role 'QA Engineer' created in TargetProcess
					And Role 'Team Lead' created in TargetProcess
					And following users created in TargetProcess:
					|userLogin|userEmail|
					|Johnson|Johnson@mail.com|
					|Lansie|Lansie@mail.com|
					|Dowson|Dowson@mail.com|
					And bugzilla profile with default roles mapping created
					
					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1'
					And bug 1 has reporter 'Dowson@mail.com'
				When synchronizing bugzilla bugs
				Then bugs with following names should be created in TargetProcess: bug1
					And QA 'Dowson' should be newly assigned on TargetProcess bug with name 'bug1'
					And TargetProcess bug with name 'bug1' should have no developer assigned
			"
				.Execute(In.Context<BugSyncActionSteps>().And<AssignmentsFromBugzillaSyncSpecs>());
		}

		[Test]
		public void ShouldCreateAssignmentOnBugUpdated()
		{
			@"
				Given Role 'Developer' created in TargetProcess
					And Role 'QA Engineer' created in TargetProcess
					And Role 'Team Lead' created in TargetProcess
					And following users created in TargetProcess:
					|userLogin|userEmail|
					|Johnson|Johnson@mail.com|
					|Lansie|Lansie@mail.com|
					|Dowson|Dowson@mail.com|
					And bugzilla profile with default roles mapping created
					
					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1'
					And bug 1 has reporter 'Dowson@mail.com'
					And bug 1 has assignee 'Lansie@mail.com'
					And synchronizing bugzilla bugs
					
					And bug 1 has assignee 'Lansie@mail.com'
				When synchronizing bugzilla bugs
				Then bugs with following names should be updated in TargetProcess: bug1
					And TargetProcess bug with name 'bug1' should have developer 'Lansie'
					And TargetProcess bug with name 'bug1' should have QA 'Dowson'
			"
				.Execute(In.Context<BugSyncActionSteps>().And<AssignmentsFromBugzillaSyncSpecs>());
		}

		[Test]
		public void ShouldUpdateAssignmentOnBugUpdated()
		{
			@"
				Given Role 'Developer' created in TargetProcess
					And Role 'QA Engineer' created in TargetProcess
					And Role 'Team Lead' created in TargetProcess
					And following users created in TargetProcess:
					|userLogin|userEmail|
					|Johnson|Johnson@mail.com|
					|Lansie|Lansie@mail.com|
					|Dowson|Dowson@mail.com|
					And bugzilla profile with default roles mapping created

					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1'
					And bug 1 has reporter 'Dowson@mail.com'
					And bug 1 has assignee 'Dowson@mail.com'
					And synchronizing bugzilla bugs
					
					And bug 1 has assignee 'Lansie@mail.com'
				When synchronizing bugzilla bugs
				Then bugs with following names should be updated in TargetProcess: bug1
					And TargetProcess bug with name 'bug1' should have developer 'Lansie'
					And TargetProcess bug with name 'bug1' should have QA 'Dowson'
			"
				.Execute(In.Context<BugSyncActionSteps>().And<AssignmentsFromBugzillaSyncSpecs>());
		}

		[Test]
		public void ShouldDeleteExistingAssignmentOnBugUpdated()
		{
			@"
				Given Role 'Developer' created in TargetProcess
					And Role 'QA Engineer' created in TargetProcess
					And Role 'Team Lead' created in TargetProcess
					And following users created in TargetProcess:
					|userLogin|userEmail|
					|Johnson|Johnson@mail.com|
					|Lansie|Lansie@mail.com|
					|Dowson|Dowson@mail.com|
					And bugzilla profile with default roles mapping created

					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1'
					And bug 1 has assignee 'Lansie@mail.com'
					And bug 1 has reporter 'Dowson@mail.com'
					And synchronizing bugzilla bugs
					
					And bug 1 has empty assignee
				When synchronizing bugzilla bugs
				Then bugs with following names should be updated in TargetProcess: bug1
					And TargetProcess bug with name 'bug1' should have no developer assigned
					And TargetProcess bug with name 'bug1' should have QA 'Dowson'
			"
				.Execute(In.Context<BugSyncActionSteps>().And<AssignmentsFromBugzillaSyncSpecs>());
		}

		[Test]
		public void ShouldNotAssignAnyPersonIfNoMathingRolesCanBeFoundInTargetProcess()
		{
			@"
				Given Role 'Team Lead' created in TargetProcess
					And following users created in TargetProcess:
					|userLogin|userEmail|
					|Lansie|Lansie@mail.com|
					|Dowson|Dowson@mail.com|
					And bugzilla profile created

					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1'
					And bug 1 has assignee 'Lansie@mail.com'
					And bug 1 has reporter 'Dowson@mail.com'

				When synchronizing bugzilla bugs
				Then bugs with following names should be created in TargetProcess: bug1
					And TargetProcess bug with name 'bug1' should have no developer assigned
					And TargetProcess bug with name 'bug1' should no QA assigned
			"
				.Execute(In.Context<BugSyncActionSteps>().And<AssignmentsFromBugzillaSyncSpecs>());
		}

		[Test]
		public void ShouldNotAssignAnyPersonIfNoMathingUsersCanBeFoundInTargetProcess()
		{
			@"
				Given Role 'Developer' created in TargetProcess
					And Role 'QA Engineer' created in TargetProcess
					And following users created in TargetProcess:
					|userLogin|userEmail|
					|Lansie|Lansie@mail.com|
					|Dowson|Dowson@mail.com|
					And bugzilla profile created

					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1'
					And bug 1 has assignee 'Marge@mail.com'
					And bug 1 has reporter 'Joy@mail.com'

				When synchronizing bugzilla bugs
				Then bugs with following names should be created in TargetProcess: bug1
					And TargetProcess bug with name 'bug1' should have no developer assigned
					And TargetProcess bug with name 'bug1' should no QA assigned
			"
				.Execute(In.Context<BugSyncActionSteps>().And<AssignmentsFromBugzillaSyncSpecs>());
		}

		[Test]
		public void ShouldUnassignPersonOnUpdateIfNoMathingUsersCanBeFoundInTargetProcess()
		{
			@"
				Given Role 'Developer' created in TargetProcess
					And Role 'QA Engineer' created in TargetProcess
					And following users created in TargetProcess:
					|userLogin|userEmail|
					|Lansie|Lansie@mail.com|
					|Dowson|Dowson@mail.com|
					And bugzilla profile with default roles mapping created

					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1'
					And bug 1 has assignee 'Lansie@mail.com'
					And bug 1 has reporter 'Dowson@mail.com'
					And synchronizing bugzilla bugs
					And bug 1 has assignee 'Jane@mail.com'

				When synchronizing bugzilla bugs
				Then bugs with following names should be created in TargetProcess: bug1
					And TargetProcess bug with name 'bug1' should have no developer assigned
					And TargetProcess bug with name 'bug1' should have QA 'Dowson'
			"
				.Execute(In.Context<BugSyncActionSteps>().And<AssignmentsFromBugzillaSyncSpecs>());
		}

		[Test]
		public void ShouldCreateAssignmentByMapping()
		{
			@"
				Given Role 'Developer' created in TargetProcess
					And Role 'QA Engineer' created in TargetProcess
					And Role 'Team Lead' created in TargetProcess
					And following users created in TargetProcess:
					|userLogin|userEmail|
					|Johnson|Johnson@mail.com|
					|Lansie|Lansie@mail.com|
					|Dowson|Dowson@mail.com|

					And bugzilla profile with default roles mapping created
					And user mapping added:
					|bugzillaEmail|targetProcessLogin|
					|BugzillaUser@mail.com|Dowson|
					
					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1'
					And bug 1 has assignee 'BugzillaUser@mail.com'
				When synchronizing bugzilla bugs
				Then bugs with following names should be created in TargetProcess: bug1
					And developer 'Dowson' should be newly assigned on TargetProcess bug with name 'bug1'
					And import should be completed
			"
				.Execute(In.Context<BugSyncActionSteps>().And<AssignmentsFromBugzillaSyncSpecs>());
		}

		[Test]
		public void ShouldUpdateAssignmentByMapping()
		{
			@"
				Given Role 'Developer' created in TargetProcess
					And Role 'QA Engineer' created in TargetProcess
					And Role 'Team Lead' created in TargetProcess
					And following users created in TargetProcess:
					|userLogin|userEmail|
					|Johnson|Johnson@mail.com|
					|Lansie|Lansie@mail.com|
					|Dowson|Dowson@mail.com|
					And bugzilla profile with default roles mapping created
					And user mapping added:
					|bugzillaEmail|targetProcessLogin|
					|BugzillaUser1@mail.com|Dowson|
					|BugzillaUser2@mail.com|Johnson|
					
					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1'
					And bug 1 has assignee 'BugzillaUser1@mail.com'
					And synchronizing bugzilla bugs
					
					And bug 1 has assignee 'BugzillaUser2@mail.com'
				When synchronizing bugzilla bugs
				Then bugs with following names should be created in TargetProcess: bug1
					And developer 'Johnson' should be newly assigned on TargetProcess bug with name 'bug1'
					And import should be completed
			"
				.Execute(In.Context<BugSyncActionSteps>().And<AssignmentsFromBugzillaSyncSpecs>());
		}

		[Test]
		public void ShouldCreateAssignmentByWildcardMapping()
		{
			@"
				Given Role 'Developer' created in TargetProcess
					And Role 'QA Engineer' created in TargetProcess
					And following users created in TargetProcess:
					|userLogin|userEmail|
					|Johnson|Johnson@mail.com|
					|Lansie|Lansie@mail.com|
					|Dowson|Dowson@mail.com|

					And bugzilla profile with default roles mapping created
					And user mapping added:
					|bugzillaEmail|targetProcessLogin|
					|BugzillaUser@mail.com|Dowson|
					|*|Johnson|
					
					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1'
					And bug 1 has assignee 'BugzillaUser@mail.com'
					And bug 1 has reporter 'Lansie@mail.com'
				When synchronizing bugzilla bugs
				Then bugs with following names should be created in TargetProcess: bug1
					And TargetProcess bug with name 'bug1' should have developer 'Dowson'
					And TargetProcess bug with name 'bug1' should have QA 'Johnson'
					And import should be completed
			"
				.Execute(In.Context<BugSyncActionSteps>().And<AssignmentsFromBugzillaSyncSpecs>());
		}

		[Test]
		public void ShouldAssignUsersIgnoreCase()
		{
			@"
				Given Role 'Developer' created in TargetProcess
					And Role 'QA Engineer' created in TargetProcess
					
					And following users created in TargetProcess:
					|userLogin|userEmail|
					|Johnson|Johnson@mail.com|
					|Lansie|Lansie@mail.com|
					
					And bugzilla profile with default roles mapping created
					And user mapping added:
					|bugzillaEmail|targetProcessLogin|
					|BugzillaUser@mail.com|Lansie|
					
					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1'
					And bug 1 has assignee 'johnson@mail.com'
					And bug 1 has reporter 'bugzillauser@mail.com'
				When synchronizing bugzilla bugs
				Then bugs with following names should be created in TargetProcess: bug1
					And TargetProcess bug with name 'bug1' should have developer 'Johnson'
					And TargetProcess bug with name 'bug1' should have QA 'Lansie'
					And import should be completed
			"
				.Execute(In.Context<BugSyncActionSteps>().And<AssignmentsFromBugzillaSyncSpecs>());
		}

		[Test]
		public void ShouldCreateEmptyAssignmentByEmailIfUserIsInactive()
		{
			@"
				Given Role 'Developer' created in TargetProcess
					And Role 'QA Engineer' created in TargetProcess
					And Role 'Team Lead' created in TargetProcess
					And following users created in TargetProcess:
					|userLogin|userEmail|
					|Johnson|Johnson@mail.com|
					|Lansie|Lansie@mail.com|
					|Dowson|Dowson@mail.com|
					And inactive user 'Inactive' with email 'inactive@mail.com' created
					And bugzilla profile created
					
					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1'
					And bug 1 has assignee 'inactive@mail.com'
				When synchronizing bugzilla bugs
				Then bugs with following names should be created in TargetProcess: bug1
					And TargetProcess bug with name 'bug1' should have no developer assigned
					And import should be completed
			"
				.Execute(In.Context<BugSyncActionSteps>().And<AssignmentsFromBugzillaSyncSpecs>());
		}

		[Test]
		public void ShouldCreateEmptyAssignmentByEmailIfUserIsDeleted()
		{
			@"
				Given Role 'Developer' created in TargetProcess
					And Role 'QA Engineer' created in TargetProcess
					And Role 'Team Lead' created in TargetProcess
					And following users created in TargetProcess:
					|userLogin|userEmail|
					|Johnson|Johnson@mail.com|
					|Lansie|Lansie@mail.com|
					|Dowson|Dowson@mail.com|
					And deleted user 'Deleted' with email 'deleted@mail.com' created
					And bugzilla profile created
					
					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1'
					And bug 1 has assignee 'inactive@mail.com'
				When synchronizing bugzilla bugs
				Then bugs with following names should be created in TargetProcess: bug1
					And TargetProcess bug with name 'bug1' should have no developer assigned
					And import should be completed
			"
				.Execute(In.Context<BugSyncActionSteps>().And<AssignmentsFromBugzillaSyncSpecs>());
		}

		[Test]
		public void ShouldCreateEmptyAssignmentByEmailIfFewTpUsersArePresentWithSuchEmail()
		{
			@"
				Given Role 'Developer' created in TargetProcess
					And Role 'QA Engineer' created in TargetProcess
					And Role 'Team Lead' created in TargetProcess
					And following users created in TargetProcess:
					|userLogin|userEmail|
					|Johnson|Johnson@mail.com|
					|Lansie|Johnson@mail.com|
					And bugzilla profile created
					
					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1'
					And bug 1 has assignee 'Johnson@mail.com'
				When synchronizing bugzilla bugs
				Then bugs with following names should be created in TargetProcess: bug1
					And TargetProcess bug with name 'bug1' should have no developer assigned
					And import should be completed
			"
				.Execute(In.Context<BugSyncActionSteps>().And<AssignmentsFromBugzillaSyncSpecs>());
		}

		[Test]
		public void ShouldCreateEmptyAssignmentByMappingIfMappedUserIsInactiveInTp()
		{
			@"
				Given Role 'Developer' created in TargetProcess
					And Role 'QA Engineer' created in TargetProcess
					And Role 'Team Lead' created in TargetProcess
					And following users created in TargetProcess:
					|userLogin|userEmail|
					|Lansie|Lansie@mail.com|
					And TargetProcess user 'Lansie' is inactive
					And bugzilla profile created

					And user mapping added:
					|bugzillaEmail|targetProcessLogin|
					|BugzillaUser@mail.com|Lansie|
					
					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1'
					And bug 1 has assignee 'BugzillaUser@mail.com'
				When synchronizing bugzilla bugs
				Then bugs with following names should be created in TargetProcess: bug1
					And TargetProcess bug with name 'bug1' should have no developer assigned
					And import should be completed
			"
				.Execute(In.Context<BugSyncActionSteps>().And<AssignmentsFromBugzillaSyncSpecs>());
		}

		[Given(
			"bugzilla profile user mapping created for bugzilla user '$bugzillaUserEmail' and target process user '$targetProcessUserLogin'"
			)]
		public void MapBugzillaAndTargetProcessUser(string bugzillaUserEmail, string targetProcessUserLogin)
		{
			var user = Context.Users.Single(u => u.Login == targetProcessUserLogin);
			Profile.GetProfile<BugzillaProfile>().UserMapping = new MappingContainer
			                                                    	{
			                                                    		new MappingElement
			                                                    			{
			                                                    				Key = bugzillaUserEmail,
			                                                    				Value =
			                                                    					new MappingLookup {Id = user.ID.Value, Name = user.Login}
			                                                    			}
			                                                    	};
		}

		[Given("inactive user '$userLogin' with email '$userEmail' created")]
		public void CreateInactiveUser(string userLogin, string userEmail)
		{
			Context.Users.Add(new UserDTO {Login = userLogin, Email = userEmail, ID = Context.GetNextId(), IsActive = false});
		}

		[Given("deleted user '$userLogin' with email '$userEmail' created")]
		public void CreateDeletedUser(string userLogin, string userEmail)
		{
			Context.Users.Add(new UserDTO
			                  	{
			                  		Login = userLogin,
			                  		Email = userEmail,
			                  		ID = Context.GetNextId(),
			                  		IsActive = true,
			                  		DeleteDate = CurrentDate.Value
			                  	});
		}

		[Given("TargetProcess user '$tpLogin' is inactive")]
		public void SetUserInactive(string tpLogin)
		{
			var user = Context.Users.Single(u => u.Login == tpLogin);
			user.IsActive = false;
		}

		[Then("TargetProcess bug with name '$bugName' should have developer '$login'")]
		public void CheckDeveloper(string bugName, string login)
		{
			CheckAssignable(bugName, login, "Developer");
		}

		[Then("TargetProcess bug with name '$bugName' should have QA '$login'")]
		public void CheckQa(string bugName, string login)
		{
			CheckAssignable(bugName, login, "QA Engineer");
		}

		[Then("developer '$developerLogin' should be newly assigned on TargetProcess bug with name '$bugName'")]
		public void CheckAddedDeveloper(string developerLogin, string bugName)
		{
			CheckAssignable(bugName, developerLogin, "Developer");
		}

		[Then("QA '$qaLogin' should be newly assigned on TargetProcess bug with name '$bugName'")]
		public void CheckQA(string qaLogin, string bugName)
		{
			CheckAssignable(bugName, qaLogin, "QA Engineer");
		}

		[Then("TargetProcess bug with name '$bugName' should have no developer assigned")]
		public void CheckThereIsNoDeveloperAssignment(string name)
		{
			var roleId = Context.Roles.Where(x => x.Name == "Developer").Select(x => x.ID).SingleOrDefault();
			Profile.Get<TeamDTO>().Where(x => x.RoleID == roleId).Should(Be.Empty, "Profile.Get<TeamDTO>().Where(x => x.RoleID == roleId).Should(Be.Empty)");
		}

		[Then("TargetProcess bug with name '$bugName' should no QA assigned")]
		public void CheckThereIsNoQaAssigned(string bugName)
		{
			var roleId = Context.Roles.Where(x => x.Name == "QA Engineer").Select(x => x.ID).SingleOrDefault();
			Profile.Get<TeamDTO>().Where(x => x.RoleID == roleId).Should(Be.Empty, "Profile.Get<TeamDTO>().Where(x => x.RoleID == roleId).Should(Be.Empty)");
		}

		private void CheckAssignable(string bugName, string tpLogin, string role)
		{
			var teams = Profile.Get<TeamDTO>();

			teams.Where(x => x.RoleID == Context.Roles.Single(r => r.Name == role).ID)
				.Where(x => x.UserID == Context.Users.Single(y => y.Login == tpLogin).UserID)
				.Where(x => x.AssignableID == Context.TpBugs.Single(y => y.Name == bugName).BugID)
				.Count().Should(Be.EqualTo(1), "teams.Where(x => x.RoleID == Context.Roles.Single(r => r.Name == role).ID).Where(x => x.UserID == Context.Users.Single(y => y.Login == tpLogin).UserID).Where(x => x.AssignableID == Context.TpBugs.Single(y => y.Name == bugName).BugID).Count().Should(Be.EqualTo(1))");
		}
	}
}