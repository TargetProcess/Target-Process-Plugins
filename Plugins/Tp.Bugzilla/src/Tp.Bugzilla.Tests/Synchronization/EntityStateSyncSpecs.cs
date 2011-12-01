// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Linq;
using NBehave.Narrator.Framework;
using NUnit.Framework;
using StructureMap;
using Tp.Bugzilla.Tests.Mocks;
using Tp.Bugzilla.Tests.Synchronization.Mapping;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Testing.Common.NBehave;
using Tp.Testing.Common.NUnit;

namespace Tp.Bugzilla.Tests.Synchronization
{
	[TestFixture, ActionSteps]
	public class EntityStateSyncSpecs : BugzillaTestBase
	{
		[Test]
		public void ShouldUpdateBugzillaBugStateByGuessing()
		{
			@"
				Given TargetProcess contains bug entity states for project 1 : 1-assigned,2-new
					And bugzilla contains bug statuses : assigned,new
					And bugzilla profile for project 1 created 
					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1'
					And bug 1 has status 'new'
					And synchronizing bugzilla bugs
				When bug 'bug1' entity state was updated in TargetProcess to 'assigned'
				Then bug 1 status in bugzilla should be 'assigned'
			"
				.Execute(In.Context<BugSyncActionSteps>().And<BugSyncSpecs>().And<EntityStateSyncSpecs>());
		}

		[Test]
		public void ShouldUpdateBugzillaBugStateByMapping()
		{
			@"
				Given TargetProcess contains bug entity states for project 1 : 1-open,2-done,3-invalid
					And bugzilla contains bug statuses : new,closed,done
					And bugzilla profile for project 1 created 
					And profile has following states mapping:
						|key|value|
						|new|{Id:1, Name:""open""}|
					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1'
					And bug 1 has status 'done'
					And synchronizing bugzilla bugs
				When bug 'bug1' entity state was updated in TargetProcess to 'open'
				Then bug 1 status in bugzilla should be 'new'
			"
				.Execute(In.Context<BugSyncActionSteps>().And<BugSyncSpecs>().And<EntityStateSyncSpecs>().And<EntityStateMappingSpecs>());
		}

		[Test]
		public void ShouldDoNothingIfCantMapTpStateToBzState()
		{
			@"
				Given TargetProcess contains bug entity states for project 1 : 1-open,2-done,3-invalid
					And bugzilla contains bug statuses : new,closed,done
					And bugzilla profile for project 1 created 
					And profile has following states mapping:
						|key|value|
						|new|{Id:1, Name:""open""}|
					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1'
					And bug 1 has status 'done'
					And synchronizing bugzilla bugs
				When bug 'bug1' entity state was updated in TargetProcess to 'invalid'
				Then bug 1 status in bugzilla should be 'done'
			"
				.Execute(In.Context<BugSyncActionSteps>().And<BugSyncSpecs>().And<EntityStateSyncSpecs>().And<EntityStateMappingSpecs>());
		}

		[Test]
		public void ShouldUpdateBugzillaBugStateAndSetDuplicate()
		{
			@"
				Given TargetProcess contains bug entity states for project 1 : 1-assigned,2-new
					And bugzilla contains bug statuses : assigned,new
					And bugzilla contains bug resolutions : duplicate
					And bugzilla profile for project 1 created 
					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1'
					And bug 1 has status 'new'
					And bugzilla contains bug with id 2
					And bug 2 has name 'bug2'
					And synchronizing bugzilla bugs
				When bug 'bug1' entity state was updated in TargetProcess with comment 'duplicate:bug2' to 'assigned'
				Then bug 1 status in bugzilla should be 'assigned'
					And bug 1 resolution in bugzilla should be 'duplicate'
					And bug 1 duplicate id in bugzilla should be 2
			"
				.Execute(In.Context<BugSyncActionSteps>().And<BugSyncSpecs>().And<EntityStateSyncSpecs>());
		}

		[Test]
		public void ShouldUpdateBugzillaBugStateWithResolution()
		{
			@"
				Given TargetProcess contains bug entity states for project 1 : 1-assigned,2-new
					And bugzilla contains bug statuses : assigned,new
					And bugzilla contains bug resolutions : duplicate, closed, fixed
					And bugzilla profile for project 1 created 
					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1'
					And bug 1 has status 'new'
					And bugzilla contains bug with id 2
					And bug 2 has name 'bug2'
					And synchronizing bugzilla bugs
				When bug 'bug1' entity state was updated in TargetProcess with comment 'closed' to 'assigned'
				Then bug 1 status in bugzilla should be 'assigned'
					And bug 1 resolution in bugzilla should be 'closed'
			"
				.Execute(In.Context<BugSyncActionSteps>().And<BugSyncSpecs>().And<EntityStateSyncSpecs>());
		}

		[Test]
		public void ShouldNotUpdateStatusInBugzillaIfStateWasNotChanged()
		{
			@"
				Given TargetProcess contains bug entity states for project 1 : 1-assigned,2-new
					And bugzilla contains bug statuses : assigned,new
					And bugzilla profile for project 1 created 
					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1'
					And bug 1 has status 'new'
					And synchronizing bugzilla bugs
				When bug 'bug1' description was updated in TargetProcess to 'bug2'
				Then bug 1 status in bugzilla should be 'new'
					And bug 1 should not be updated in Bugzilla
			"
				.Execute(In.Context<BugSyncActionSteps>().And<BugSyncSpecs>().And<EntityStateSyncSpecs>());
		}

		[Test]
		public void ShouldNotProcessUnsyncronizedBug()
		{
			@"
				Given TargetProcess contains bug entity states for project 1 : 1-assigned,2-new
					And bugzilla contains bug statuses : assigned,new
					And bugzilla profile for project 1 created 
					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1'
					And bug 1 has status 'new'
					And synchronizing bugzilla bugs
				When unsynchronized bug 'bug2' entity state was updated in TargetProcess to 'assigned'
				Then bug 1 status in bugzilla should be 'new'
					And bug 1 should not be updated in Bugzilla
			"
				.Execute(In.Context<BugSyncActionSteps>().And<BugSyncSpecs>().And<EntityStateSyncSpecs>());
		}

		[Test]
		public void ShouldNotProcessBugThatWasJustUpdatedByProfile()
		{
			@"
				Given TargetProcess contains bug entity states for project 1 : 1-assigned,2-new
					And bugzilla contains bug statuses : assigned,new
					And bugzilla profile for project 1 created 
					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1'
					And bug 1 has status 'new'
					And synchronizing bugzilla bugs

					And bug 1 has status 'assigned'
				When synchronizing bugzilla bugs
				Then bug in TargetProcess with name 'bug1' should have state 'assigned'
					And bug 1 should not be updated in Bugzilla
			"
				.Execute(In.Context<BugSyncActionSteps>().And<BugSyncSpecs>().And<EntityStateSyncSpecs>());
		}

		[Test]
		public void ShouldNotChangeStateIfMultipleBugzillaStatesMappedToTheSameTpState()
		{
			@"
				Given TargetProcess contains bug entity states for project 1 : 1-assigned,2-new
					And bugzilla profile for project 1 created 
					And bugzilla contains bug statuses : assigned,new,open
					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1'
					And bug 1 has status 'assigned'
					And profile has following states mapping:
						|key|value|
						|new|{Id:2, Name:""new""}|
						|open|{Id:2,Name:""new""}|
						|assigned|{Id:1, Name:""assigned""}|
					And synchronizing bugzilla bugs
				When bug 'bug1' entity state was updated in TargetProcess with comment 'closed' to 'new'
				Then bug in TargetProcess with name 'bug1' should have state 'new'
					And bug 1 should not be updated in Bugzilla
			"
				.Execute(In.Context<BugSyncActionSteps>().And<BugSyncSpecs>().And<EntityStateSyncSpecs>().And<EntityStateMappingSpecs>());
		}

		[Given(@"bugzilla contains bug statuses : (?<statuses>([^,]+,?\s*)+)")]
		public void InitBugzillaStatuses(string[] statuses)
		{
			ObjectFactory.GetInstance<BugzillaServiceMock>().Statuses.AddRange(statuses);
		}

		[Given(@"bugzilla contains bug resolutions : (?<resolutions>([^,]+,?\s*)+)")]
		public void InitBugzillaResolutions(string[] resolutions)
		{
			ObjectFactory.GetInstance<BugzillaServiceMock>().Resolutions.AddRange(resolutions);
		}

		[Given("bug '$bugName' description was updated in TargetProcess to '$bugDescription'")]
		public void UpdateDescriptionInTp(string bugName, string bugDescription)
		{
			var bug = Context.TpBugs.Single(b => b.Name == bugName);
			bug.Description = bugDescription;
			TransportMock.HandleMessageFromTp(new BugUpdatedMessage
			                                  	{
			                                  		Dto = bug,
			                                  		ChangedFields = new[] {BugField.Description}
			                                  	});
		}

		[When("bug '$bugName' entity state was updated in TargetProcess with comment '$resolutionComment' to '$entityState'")]
		public void UpdateBugEntityStateAndResolution(string bugName, string resolutionComment, string entityState)
		{
			var state = Context.EntityStates.Single(s => s.Name == entityState);

			var bug = Context.TpBugs.Single(b => b.Name == bugName);
			bug.EntityStateID = state.ID;
			bug.EntityStateName = state.Name;

			if (!string.IsNullOrEmpty(resolutionComment))
			{
				if (resolutionComment.Contains(':'))
				{
					var pair = resolutionComment.Split(':');
					var duplicateBug = Context.TpBugs.Single(b => b.Name == pair[1]);
					resolutionComment = resolutionComment.Replace(pair[1], "#" + duplicateBug.ID);
				}

				bug.CommentOnChangingState = resolutionComment;
			}

			TransportMock.HandleMessageFromTp(new BugUpdatedMessage
			                                  	{
			                                  		Dto = bug,
			                                  		ChangedFields = new[] {BugField.EntityStateID}
			                                  	});
		}

		[When("bug '$bugName' entity state was updated in TargetProcess to '$entityState'")]
		public void UpdateBugEntityState(string bugName, string entityState)
		{
			UpdateBugEntityStateAndResolution(bugName, null, entityState);
		}

		[When("unsynchronized bug '$bugName' entity state was updated in TargetProcess to '$entityState'")]
		public void UpdateEntityStateOfUnsyncBug(string bugName, string entityState)
		{
			var state = Context.EntityStates.Single(s => s.Name == entityState);

			TransportMock.HandleMessageFromTp(new BugUpdatedMessage
			                                  	{
			                                  		Dto = new BugDTO
			                                  		      	{
			                                  		      		EntityStateID = state.ID,
			                                  		      		EntityStateName = state.Name,
			                                  		      		ID = Context.GetNextId(),
			                                  		      		Name = bugName
			                                  		      	},
			                                  		ChangedFields = new[] {BugField.EntityStateID}
			                                  	});
		}

		[Then("bug $bugId status in bugzilla should be '$status'")]
		public void CheckBugzillaStatus(int bugId, string status)
		{
			Context.BugzillaBugs.GetById(bugId).bug_status.Should(Be.EqualTo(status));
		}

		[Then("bug $bugId resolution in bugzilla should be '$resolution'")]
		public void CheckResolution(int bugId, string resolution)
		{
			Context.BugzillaBugs.GetById(bugId).resolution.Should(Be.EqualTo(resolution));
		}

		[Then("bug $bugId duplicate id in bugzilla should be $duplicateBugId")]
		public void CheckDuplicateId(int bugId, string duplicateBugId)
		{
			Context.BugzillaBugs.GetById(bugId).dup_id.Should(Be.EqualTo(duplicateBugId));
		}

		[Then("bug $bugId should not be updated in Bugzilla")]
		public void BugShouldNotBeUpdated(string bugId)
		{
			ObjectFactory.GetInstance<BugzillaServiceMock>().BugUpdateCalls.ContainsKey(bugId).Should(Be.False);
		}
	}
}
