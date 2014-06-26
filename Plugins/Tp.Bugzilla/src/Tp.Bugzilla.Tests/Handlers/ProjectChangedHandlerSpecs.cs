// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NBehave.Narrator.Framework;
using NUnit.Framework;
using Tp.Bugzilla.Tests.Synchronization;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Testing.Common.NBehave;

namespace Tp.Bugzilla.Tests.Handlers
{
	[ActionSteps]
    [Category("PartPlugins0")]
	public class ProjectChangedHandlerSpecs : BugzillaTestBase
	{
		[Test]
		public void ShouldCheckUpdate()
		{
			@"
				Given bugzilla profile for project 1 created
					And set project with id 1 and name 'project 1' to storage
				When handled ProjectUpdatedMessage message with project id 1 and name 'project updated'
				Then projects storage should contain 1 items
					And projects storage should contain item with id 1 and name 'project updated'
			"
				.Execute(In.Context<BugSyncActionSteps>().And<ProjectChangedHandlerSpecs>());
		}

		[Test]
		public void ShouldCheckUpdateUnproperProject()
		{
			@"
				Given bugzilla profile for project 1 created
					And set project with id 1 and name 'project 1' to storage
				When handled ProjectUpdatedMessage message with project id 2 and name 'project 2'
				Then projects storage should contain 1 items
					And projects storage should contain item with id 1 and name 'project 1'
			"
				.Execute(In.Context<BugSyncActionSteps>().And<ProjectChangedHandlerSpecs>());
		}

		[Test]
		public void ShouldCheckDelete()
		{
			@"
				Given bugzilla profile for project 1 created
					And set project with id 1 and name 'project 1' to storage
				When handled ProjectDeletedMessage message with project id 1
				Then projects storage should contain 0 items
			"
				.Execute(In.Context<BugSyncActionSteps>().And<ProjectChangedHandlerSpecs>());
		}

		[Test]
		public void ShouldCheckDeleteUnproperProject()
		{
			@"
				Given bugzilla profile for project 1 created
					And set project with id 1 and name 'project 1' to storage
				When handled ProjectDeletedMessage message with project id 2
				Then projects storage should contain 1 items
					And projects storage should contain item with id 1 and name 'project 1'
			"
				.Execute(In.Context<BugSyncActionSteps>().And<ProjectChangedHandlerSpecs>());
		}

		[Given("set project with id $id and name '$name' to storage")]
		public void SetSeverityToStorage(int id, string name)
		{
			Profile.Get<ProjectDTO>().Add(new ProjectDTO {ID = id, ProjectID = id, Name = name});
		}

		[When("handled ProjectUpdatedMessage message with project id $id and name '$name'")]
		public void HandleUpdate(int id, string name)
		{
			var project = new ProjectDTO {ID = id, ProjectID = id, Name = name};
			TransportMock.HandleMessageFromTp(Profile, new ProjectUpdatedMessage {Dto = project});
		}

		[When("handled ProjectDeletedMessage message with project id $id")]
		public void HandleDelete(int id)
		{
			var project = new ProjectDTO {ID = id, ProjectID = id};
			TransportMock.HandleMessageFromTp(Profile, new ProjectDeletedMessage {Dto = project});
		}
	}
}