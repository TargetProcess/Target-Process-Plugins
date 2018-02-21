// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using NBehave.Narrator.Framework;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap;
using Tp.BugTracking.ImportToTp;
using Tp.Bugzilla.Tests.Synchronization.Mapping;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Testing.Common.NBehave;
using Tp.Testing.Common.NUnit;

namespace Tp.Bugzilla.Tests.Synchronization
{
    [TestFixture]
    [ActionSteps]
    [Category("PartPlugins1")]
    public class BugSyncSpecs : BugzillaTestBase
    {
        [Test]
        public void ShouldImportBugWithMinimalSettings()
        {
            @"
				Given bugzilla profile for project 1 created 
					And bugzilla contains bug with id 12
					And bug 12 has name 'bug1'
					And bug 12 has description 'important bug'
					And bug 12 was created on '2010-11-29 13:13'
				When synchronizing bugzilla bugs
				Then bugs with following names should be created in TargetProcess: bug1
					And bug in TargetProcess with name 'bug1' should have description 'important&nbsp;bug'
					And bug in TargetProcess with name 'bug1' should be in project 1
					And bug in TargetProcess with name 'bug1' should have creation date '2010-11-29 13:13'
					And bug in TargetProcess with name 'bug1' should have comment on changing state 'State is changed by Bugzilla plugin'
			"
                .Execute(In.Context<BugSyncActionSteps>().And<BugSyncSpecs>());
        }

        [Test]
        public void ShouldImportBugsByChunksCorrectly()
        {
            @"
				Given bugzilla profile for project 1 created 
					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1'
					And bugzilla contains bug with id 2
					And bug 2 has name 'bug2'
					And bugzilla contains bug with id 3
					And bug 3 has name 'bug3'
					And chunk size is 1
				When synchronizing bugzilla bugs
				Then 3 bugs should be created in TargetProcess"
                .Execute(In.Context<BugSyncActionSteps>().And<BugSyncSpecs>());
        }

        [Test]
        public void ShouldNotImportDuplicateBugsByChunks()
        {
            @"
				Given bugzilla profile for project 1 created
					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1'
					And bugzilla contains bug with id 2
					And bug 2 has name 'bug2'
					And profile queries return bug 1 twice
					And chunk size is 1
				When synchronizing bugzilla bugs
				Then 2 bugs should be created in TargetProcess
					And 0 bugs should be updated in TargetProcess"
                .Execute(In.Context<BugSyncActionSteps>().And<BugSyncSpecs>());
        }

        [Test]
        public void ShouldNotSyncProcessedBugs()
        {
            @"
				Given bugzilla profile for project 1 created 
					And bugzilla contains bug created on '2010-11-29' with id 12
					And bug 12 has name 'bug1'
					And bug 12 has description 'important bug'
					
					And bugzilla contains bug created on '2010-11-30' with id 13
					And bug 13 has name 'bug2'
					And bug 13 has description 'important bug2'
					
					And profile was synchronized last time on '2010-11-29'
				When synchronizing bugzilla bugs
				Then bugs with following names should be created in TargetProcess: bug2
					And bug in TargetProcess with name 'bug2' should have description 'important&nbsp;bug2'
			"
                .Execute(In.Context<BugSyncActionSteps>().And<BugSyncSpecs>());
        }

        [Test]
        public void ShouldGuessBugEntityState()
        {
            @"
				Given TargetProcess contains bug entity states for project 1 : 1-assigned,2-new
					And bugzilla profile for project 1 created 
					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1' 
					And bug 1 has status 'new'
				When synchronizing bugzilla bugs
				Then bugs with following names should be created in TargetProcess: bug1
					And bug in TargetProcess with name 'bug1' should have state 'new'
			"
                .Execute(In.Context<BugSyncActionSteps>().And<BugSyncSpecs>());
        }

        [Test]
        public void EntityStateMatchShouldNotBeCaseSensitive()
        {
            @"
				Given TargetProcess contains bug entity states for project 1 : 1-assigned,2-nEw
					And bugzilla profile for project 1 created 
					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1' 
					And bug 1 has status 'NEW'
				When synchronizing bugzilla bugs
				Then bugs with following names should be created in TargetProcess: bug1
					And bug in TargetProcess with name 'bug1' should have state 'nEw'
			"
                .Execute(In.Context<BugSyncActionSteps>().And<BugSyncSpecs>());
        }

        [Test]
        public void ShouldNotUpdateEntityStateInTpIfStateDoesntHaveAMatchInStatesMapping()
        {
            @"
				Given TargetProcess contains bug entity states for project 1 : 1-Open,2-In Progress,3-Testing,4-Done
					And bugzilla profile for project 1 created
					And profile has following states mapping:
						|key|value|
						|Open|{Id:1, Name:""Open""}|
					And following states created in Bugzilla:
						|name|
						|Open|
						|In Progress|
						|Verifying|
						|Resolved|
					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1' 
					And bug 1 has status 'Open'
					And synchronizing bugzilla bugs
					And bug 1 has status 'Verifying'
				When synchronizing bugzilla bugs
				Then message sent to TargetProcess with name 'bug1' should not contain state
			"
                .Execute(In.Context<BugSyncActionSteps>().And<BugSyncSpecs>().And<EntityStateMappingSpecs>());
        }

        [Test]
        public void ShouldGuessBugSeverity()
        {
            @"
				Given TargetProcess contains following severities : 1-blocking,2-critical,3-small
					And bugzilla profile for project 1 created 
					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1' 
					And bug 1 has severity 'Critical'
				When synchronizing bugzilla bugs
				Then bugs with following names should be created in TargetProcess: bug1
					And bug in TargetProcess with name 'bug1' should have severity 'critical'
			"
                .Execute(In.Context<BugSyncActionSteps>().And<BugSyncSpecs>());
        }

        [Test]
        public void ShouldGuessBugPriority()
        {
            @"
				Given TargetProcess contains following priorities : 1-great,2-good,3-must have
					And bugzilla profile for project 1 created 
					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1' 
					And bug 1 has priority 'Great'
				When synchronizing bugzilla bugs
				Then bugs with following names should be created in TargetProcess: bug1
					And bug in TargetProcess with name 'bug1' should have priority 'great'
			"
                .Execute(In.Context<BugSyncActionSteps>().And<BugSyncSpecs>());
        }

        [Test]
        public void ShouldUpdateBugFieldsOnBugUpdated()
        {
            @"
				Given bugzilla profile for project 1 created
					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1'
					And bug 1 has description 'bug description'
					And synchronizing bugzilla bugs
					
					And bug 1 has description 'bug updated description'
				When synchronizing bugzilla bugs
				Then bugs with following names should be updated in TargetProcess: bug1
					And bug in TargetProcess with name 'bug1' should have description 'bug&nbsp;updated&nbsp;description'
			"
                .Execute(In.Context<BugSyncActionSteps>().And<BugSyncSpecs>());
        }

        [Test]
        public void ShouldSetOwnerAsDefaultUserIfNoMappingAndCantGuess()
        {
            @"
				Given Role 'Developer' created in TargetProcess
					And Role 'QA Engineer' created in TargetProcess
					And bugzilla profile for project 1 created 
					And bugzilla contains bug with id 12
					And bug 12 has name 'bug1'
					And bug 12 has reporter 'bugzilla@mail.com'
				When synchronizing bugzilla bugs
				Then bugs with following names should be created in TargetProcess: bug1
					And bug in TargetProcess with name 'bug1' should have no owner
			"
                .Execute(In.Context<BugSyncActionSteps>().And<BugSyncSpecs>());
        }

        [Test]
        public void ShouldSetOwnerByEmail()
        {
            @"
				Given Role 'Developer' created in TargetProcess
					And Role 'QA Engineer' created in TargetProcess
					And user 'Jane' with email 'jane@mail.com' created in TargetProcess
					And bugzilla profile for project 1 created 
					And bugzilla contains bug with id 12
					And bug 12 has name 'bug1'
					And bug 12 has reporter 'jane@mail.com'
				When synchronizing bugzilla bugs
				Then bugs with following names should be created in TargetProcess: bug1
					And bug in TargetProcess with name 'bug1' should have owner 'Jane'
			"
                .Execute(In.Context<BugSyncActionSteps>().And<BugSyncSpecs>());
        }

        [Given("bug $bugId has description '$bugDescription'")]
        public void SetBugDescription(int bugId, string bugDescription)
        {
            Context.BugzillaBugs.SetBugDescription(bugId, bugDescription);
        }

        [Given("bugzilla contains bug created on '$createdDate' with id $bugId")]
        public void CreateBugWithDate(string createDate, int bugId)
        {
            Context.BugzillaBugs.Add(bugId, DateTime.Parse(createDate));
        }

        [Given("profile was synchronized last time on '$lastSyncDate'")]
        public void SetLastSyncDate(string lastSyncDate)
        {
            ObjectFactory.GetInstance<IStorageRepository>().Get<Tp.Integration.Messages.Ticker.LastSyncDate>().ReplaceWith(
                new Tp.Integration.Messages.Ticker.LastSyncDate(DateTime.Parse(lastSyncDate)));
        }

        [Given("bug $bugId has status '$bugStatus'")]
        public void SetBugStatus(int bugId, string bugStatus)
        {
            Context.BugzillaBugs.SetBugStatus(bugId, bugStatus);
        }

        [Given("bug $bugId has severity '$severity'")]
        public void SetBugSeverity(int bugId, string severity)
        {
            Context.BugzillaBugs.SetBugSeverity(bugId, severity);
        }

        [Given("bug $bugId has priority '$priority' ")]
        public void SetBugPriority(int bugId, string priority)
        {
            Context.BugzillaBugs.SetBugPriority(bugId, priority);
        }

        [Given("chunk size is $size")]
        public void SetChunkSize(int size)
        {
            var chunkSize = ObjectFactory.GetInstance<IBugChunkSize>();
            chunkSize.BackToRecord(BackToRecordOptions.Expectations);
            chunkSize.Replay();
            chunkSize.Stub(x => x.Value).Return(size);
        }

        [Given("bug $bugId was created on '$creationDate'")]
        public void SetBugCreationDate(int bugId, string creationDate)
        {
            Context.BugzillaBugs.SetBugCreationDate(bugId, creationDate);
        }

        [Given("profile queries return bug $bugId twice")]
        public void DuplicateBugInStorage(int bugId)
        {
            var bug = Context.BugzillaBugs.GetById(bugId);
            Context.BugzillaBugs.Add(bug);
        }

        [Then("bug in TargetProcess with name '$bugName' should have description '$bugDescription'")]
        public void CheckBugDescription(string bugName, string bugDescription)
        {
            GetBug(bugName).Description.Should(Be.EqualTo(bugDescription), "GetBug(bugName).Description.Should(Be.EqualTo(bugDescription))");
        }

        [Then("$bugsCount bugs should be created in TargetProcess")]
        public void BugsShouldBeCreated(int bugsCount)
        {
            TransportMock.TpQueue.GetMessages<CreateCommand>()
                .Where(x => x.Dto is BugDTO)
                .Count()
                .Should(Be.EqualTo(bugsCount),
                    "TransportMock.TpQueue.GetMessages<CreateCommand>().Where(x => x.Dto is BugDTO).Count().Should(Be.EqualTo(bugsCount))");
        }

        [Then("$count bugs should be updated in TargetProcess")]
        public void BugsShouldBeUpdated(int count)
        {
            TransportMock.TpQueue.GetMessages<UpdateCommand>()
                .Where(x => x.Dto is BugDTO)
                .Count()
                .Should(Be.EqualTo(count),
                    "TransportMock.TpQueue.GetMessages<UpdateCommand>().Where(x => x.Dto is BugDTO).Count().Should(Be.EqualTo(count))");
        }

        [Then("bug in TargetProcess with name '$bugName' should be in project $projectId")]
        public void CheckProject(string bugName, int projectId)
        {
            GetBug(bugName).ProjectID.Should(Be.EqualTo(projectId), "GetBug(bugName).ProjectID.Should(Be.EqualTo(projectId))");
        }

        [Then("bug in TargetProcess with name '$bugName' should have state '$bugState'")]
        public void CheckState(string bugName, string bugState)
        {
            var state = Context.EntityStates.Single(s => s.Name == bugState);

            GetBug(bugName).EntityStateID.Should(Be.EqualTo(state.ID), "GetBug(bugName).EntityStateID.Should(Be.EqualTo(state.ID))");
        }

        [Then("bug in TargetProcess with name '$bugName' should have severity '$severityName'")]
        public void CheckSeverity(string bugName, string severityName)
        {
            var severity = Context.Severities.Single(s => s.Name == severityName);

            GetBug(bugName).SeverityID.Should(Be.EqualTo(severity.ID), "GetBug(bugName).SeverityID.Should(Be.EqualTo(severity.ID))");
        }

        [Then("bug in TargetProcess with name '$bugName' should have priority '$priorityName'")]
        public void CheckPriority(string bugName, string priorityName)
        {
            var priority = Context.Priorities.Single(s => s.Name == priorityName);

            GetBug(bugName).PriorityID.Should(Be.EqualTo(priority.ID), "GetBug(bugName).PriorityID.Should(Be.EqualTo(priority.ID))");
        }

        [Then("bug in TargetProcess with name '$bugName' should have creation date '$creationDate'")]
        public void CheckCreationDate(string bugName, string creationDate)
        {
            GetBug(bugName)
                .CreateDate.Should(Be.EqualTo(DateTime.Parse(creationDate).ToLocalTime()),
                    "GetBug(bugName).CreateDate.Should(Be.EqualTo(DateTime.Parse(creationDate).ToLocalTime()))");
        }

        [Then("bug in TargetProcess with name '$bugName' should have comment on changing state '$comment'")]
        public void CheckComment(string bugName, string comment)
        {
            GetBug(bugName)
                .CommentOnChangingState.Should(Be.EqualTo(comment), "GetBug(bugName).CommentOnChangingState.Should(Be.EqualTo(comment))");
        }

        [Then("bug in TargetProcess with name '$bugName' should have no owner")]
        public void ShouldCheckThatOwnerDoesntSet(string bugName)
        {
            GetBug(bugName).OwnerID.Should(Be.Null, "GetBug(bugName).OwnerID.Should(Be.Null)");
        }

        [Then("bug in TargetProcess with name '$bugName' should have owner '$ownerLogin'")]
        public void CheckBugOwner(string bugName, string ownerLogin)
        {
            GetBug(bugName)
                .OwnerID.Should(Be.EqualTo(Context.Users.Single(u => u.Login == ownerLogin).ID),
                    "GetBug(bugName).OwnerID.Should(Be.EqualTo(Context.Users.Single(u => u.Login == ownerLogin).ID))");
        }

        [Then("message sent to TargetProcess with name '$bugName' should not contain state")]
        public void CheckState(string bugName)
        {
            GetBug(bugName).EntityStateID.Should(Be.Null, "GetBug(bugName).EntityStateID.Should(Be.Null)");
        }
    }
}
