// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Linq;
using NBehave.Narrator.Framework;
using NUnit.Framework;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Testing.Common.NBehave;
using Tp.Testing.Common.NUnit;

namespace Tp.Bugzilla.Tests.Synchronization
{
    [TestFixture]
    [ActionSteps]
    [Category("PartPlugins1")]
    public class AssignmentsFromTargetProcessSyncSpecs : BugzillaTestBase
    {
        [Test]
        public void ShouldReassignUserByEmail()
        {
            @"
				Given Role 'Developer' created in TargetProcess
					And Role 'QA Engineer' created in TargetProcess
					And Role 'Team Lead' created in TargetProcess
					And user 'Johnson' with email 'Johnson@mail.com' created in TargetProcess
					And user 'Lansie' with email 'Lansie@mail.com' created in TargetProcess
					And user 'Dowson' with email 'Dowson@mail.com' created in TargetProcess

					And bugzilla profile with default roles mapping created
					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1'
					And bug 1 has reporter 'Dowson@mail.com'
					And bug 1 has assignee 'Lansie@mail.com'
					And synchronizing bugzilla bugs

				When user 'Johnson' assigned to bug 'bug1' as Developer in TargetProcess
				Then bug 1 in bugzilla should have reporter 'Dowson@mail.com'
					And bug 1 in bugzilla should have assignee 'Johnson@mail.com'
			"
                .Execute(
                    In.Context<BugSyncActionSteps>().And<AssignmentsFromBugzillaSyncSpecs>().And<AssignmentsFromTargetProcessSyncSpecs>
                        ());
        }

        [Test]
        public void ShouldReassignUserByMapping()
        {
            @"
				Given Role 'Developer' created in TargetProcess
					And Role 'QA Engineer' created in TargetProcess
					And Role 'Team Lead' created in TargetProcess
					And user 'Johnson' with email 'Johnson@mail.com' created in TargetProcess
					And user 'Lansie' with email 'Lansie@mail.com' created in TargetProcess
					And user 'Dowson' with email 'Dowson@mail.com' created in TargetProcess
					And bugzilla profile with default roles mapping created
					And user mapping added:
					|bugzillaEmail|targetProcessLogin|
					|BugzillaUser@mail.com|Johnson|

					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1'
					And bug 1 has reporter 'Dowson@mail.com'
					And bug 1 has assignee 'Dowson@mail.com'
					And synchronizing bugzilla bugs

				When user 'Johnson' assigned to bug 'bug1' as Developer in TargetProcess
				Then bug 1 in bugzilla should have reporter 'Dowson@mail.com'
					And bug 1 in bugzilla should have assignee 'BugzillaUser@mail.com'
			"
                .Execute(
                    In.Context<BugSyncActionSteps>().And<AssignmentsFromBugzillaSyncSpecs>().And<AssignmentsFromTargetProcessSyncSpecs>
                        ());
        }

        [Test]
        public void ShouldNotAssignUserIfMultipleBugzillaUsersMappedToSameTpUser()
        {
            @"
				Given Role 'Developer' created in TargetProcess
					And Role 'QA Engineer' created in TargetProcess
					And user 'Johnson' with email 'Johnson@mail.com' created in TargetProcess
					And user 'Dowson' with email 'Dowson@mail.com' created in TargetProcess
					And bugzilla profile with default roles mapping created
					And user mapping added:
					|bugzillaEmail|targetProcessLogin|
					|BugzillaUser1@mail.com|Johnson|
					|BugzillaUser2@mail.com|Johnson|

					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1'
					And bug 1 has reporter 'Dowson@mail.com'
					And bug 1 has assignee 'Dowson@mail.com'
					And synchronizing bugzilla bugs

				When user 'Johnson' assigned to bug 'bug1' as Developer in TargetProcess
				Then bug 1 in bugzilla should have default assignee
			"
                .Execute(In.Context<BugSyncActionSteps>()
                    .And<AssignmentsFromBugzillaSyncSpecs>()
                    .And<AssignmentsFromTargetProcessSyncSpecs>());
        }

        [Test]
        public void ShouldNotAssignUserIfWildcardBugzillaUserMappedToTpUser()
        {
            @"
				Given Role 'Developer' created in TargetProcess
					And Role 'QA Engineer' created in TargetProcess
					And user 'Johnson' with email 'Johnson@mail.com' created in TargetProcess
					And user 'Dowson' with email 'Dowson@mail.com' created in TargetProcess
					And bugzilla profile with default roles mapping created
					And user mapping added:
					|bugzillaEmail|targetProcessLogin|
					|BugzillaUser1@mail.com|Dowson|
					|*|Johnson|

					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1'
					And bug 1 has reporter 'Dowson@mail.com'
					And bug 1 has assignee 'Dowson@mail.com'
					And synchronizing bugzilla bugs

				When user 'Johnson' assigned to bug 'bug1' as Developer in TargetProcess
				Then bug 1 in bugzilla should have default assignee
			"
                .Execute(
                    In.Context<BugSyncActionSteps>().And<AssignmentsFromBugzillaSyncSpecs>().And<AssignmentsFromTargetProcessSyncSpecs>
                        ());
        }

        [Test]
        public void ShouldRemoveAssignee()
        {
            @"
				Given Role 'Developer' created in TargetProcess
					And Role 'QA Engineer' created in TargetProcess
					And Role 'Team Lead' created in TargetProcess
					And user 'Johnson' with email 'Johnson@mail.com' created in TargetProcess
					And user 'Lansie' with email 'Lansie@mail.com' created in TargetProcess
					And user 'Dowson' with email 'Dowson@mail.com' created in TargetProcess

					And bugzilla profile with default roles mapping created
					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1'
					And bug 1 has reporter 'Dowson@mail.com'
					And bug 1 has assignee 'Lansie@mail.com'
					And synchronizing bugzilla bugs

				When user reassign Developer from bug 'bug1' in TargetProcess
				Then bug 1 in bugzilla should have default assignee 
			"
                .Execute(
                    In.Context<BugSyncActionSteps>().And<AssignmentsFromBugzillaSyncSpecs>().And<AssignmentsFromTargetProcessSyncSpecs>
                        ());
        }

        [When("user '$userLogin' assigned to bug '$bugName' as Developer in TargetProcess")]
        public void AssignDeveloper(string userLogin, string bugName)
        {
            BugDTO bug = Context.TpBugs.Single(b => b.Name == bugName);
            RoleDTO developerRole = Context.Roles.Single(r => r.Name == "Developer");
            UserDTO user = Context.Users.Single(u => u.Login == userLogin);

            var existingDeveloperTeam =
                Context.TpTeams.Where(t => t.AssignableID == bug.ID)
                    .Where(t => t.RoleID == developerRole.ID)
                    .Single();

            TransportMock.HandleMessageFromTp(new TeamCreatedMessage
            {
                Dto =
                    new TeamDTO
                    {
                        ID = Context.GetNextId(),
                        AssignableID = bug.ID,
                        RoleID = developerRole.ID,
                        RoleName = developerRole.Name,
                        UserID = user.ID
                    }
            });

            existingDeveloperTeam.RoleName = developerRole.Name;
            TransportMock.HandleMessageFromTp(new TeamDeletedMessage { Dto = existingDeveloperTeam });
        }

        [When("user reassign Developer from bug '$bugName' in TargetProcess")]
        public void ReassignDeveloper(string bugName)
        {
            BugDTO bug = Context.TpBugs.Single(b => b.Name == bugName);
            RoleDTO developerRole = Context.Roles.Single(r => r.Name == "Developer");

            var existingDeveloperTeam =
                Context.TpTeams.Where(t => t.AssignableID == bug.ID)
                    .Where(t => t.RoleID == developerRole.ID)
                    .Single();

            existingDeveloperTeam.RoleName = developerRole.Name;
            TransportMock.HandleMessageFromTp(new TeamDeletedMessage { Dto = existingDeveloperTeam });
        }

        [Then("bug $bugId in bugzilla should have reporter '$email'")]
        public void CheckReporter(string bugId, string email)
        {
            Context.BugzillaBugs.Single(b => b.bug_id == bugId)
                .reporter.Should(Be.EqualTo(email), "Context.BugzillaBugs.Single(b => b.bug_id == bugId).reporter.Should(Be.EqualTo(email))");
        }

        [Then("bug $bugId in bugzilla should have assignee '$email'")]
        public void CheckAssignee(string bugId, string email)
        {
            Context.BugzillaBugs.Single(b => b.bug_id == bugId)
                .assigned_to.Should(Be.EqualTo(email),
                    "Context.BugzillaBugs.Single(b => b.bug_id == bugId).assigned_to.Should(Be.EqualTo(email))");
        }

        [Then("bug $bugId in bugzilla should have default assignee")]
        public void CheckDefaultReporter(string bugId)
        {
            Context.BugzillaBugs.Single(b => b.bug_id == bugId)
                .assigned_to.Should(Be.Null.Or.Empty,
                    "Context.BugzillaBugs.Single(b => b.bug_id == bugId).assigned_to.Should(Be.Null.Or.Empty)");
        }
    }
}
