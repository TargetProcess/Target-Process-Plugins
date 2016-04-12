// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Linq;
using NBehave.Narrator.Framework;
using NUnit.Framework;
using Newtonsoft.Json;
using Tp.Bugzilla.Tests.Synchronization;
using Tp.Bugzilla.Tests.Synchronization.Mapping;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Plugin.Common.Mapping;
using Tp.Testing.Common.NBehave;
using Tp.Testing.Common.NUnit;

namespace Tp.Bugzilla.Tests.Handlers
{
	[ActionSteps]
    [Category("PartPlugins0")]
	public class EntityStateChangedHandlerSpecs : BugzillaTestBase
	{
		[Test]
		public void ShouldCheckCreate()
		{
			@"
				Given bugzilla profile for project 1 with process 12 created
					And set entity state 'Open' with id 1 and process id 12 to storage
					And set entity state 'Done' with id 2 and process id 12 to storage
				When handled EntityStateCreatedMessage message with entity state id 3, process id 12, name 'New', entityTypeName 'Tp.BusinessObjects.Bug'
				Then entity states storage should contain 3 items
					And entity states storage should contain item 'Open' with id 1 and process id 12
					And entity states storage should contain item 'Done' with id 2 and process id 12
					And entity states storage should contain item 'New' with id 3 and process id 12
			"
				.Execute(In.Context<BugSyncActionSteps>().And<EntityStateChangedHandlerSpecs>());
		}

		[Test]
		public void ShouldCheckCreateUnproperEntityState()
		{
			@"
				Given bugzilla profile for project 1 with process 12 created
					And set entity state 'Open' with id 1 and process id 12 to storage
					And set entity state 'Done' with id 2 and process id 12 to storage
				When handled EntityStateCreatedMessage message with entity state id 3, process id 11, name 'New', entityTypeName 'Tp.BusinessObjects.Bug'
				Then entity states storage should contain 2 items
					And entity states storage should contain item 'Open' with id 1 and process id 12
					And entity states storage should contain item 'Done' with id 2 and process id 12
			"
				.Execute(In.Context<BugSyncActionSteps>().And<EntityStateChangedHandlerSpecs>());
		}

		[Test]
		public void ShouldNotSaveEntityStatesForUserStory()
		{
			@"
				Given bugzilla profile for project 1 with process 12 created
					And set entity state 'Open' with id 1 and process id 12 to storage
					And set entity state 'Done' with id 2 and process id 12 to storage
				When handled EntityStateCreatedMessage message with entity state id 3, process id 12, name 'New', entityTypeName 'Tp.BusinessObjects.UserStory'
				Then entity states storage should contain 2 items
					And entity states storage should contain item 'Open' with id 1 and process id 12
					And entity states storage should contain item 'Done' with id 2 and process id 12
			"
				.Execute(In.Context<BugSyncActionSteps>().And<EntityStateChangedHandlerSpecs>());
		}

		[Test]
		public void ShouldCheckUpdate()
		{
			@"
				Given bugzilla profile for project 1 with process 12 created
					And set entity state 'Open' with id 1 and process id 12 to storage
					And set entity state 'Done' with id 2 and process id 12 to storage
				When handled EntityStateUpdatedMessage message with entity state id 2, process id 12, name 'New', entityTypeName 'Tp.BusinessObjects.Bug'
				Then entity states storage should contain 2 items
					And entity states storage should contain item 'Open' with id 1 and process id 12
					And entity states storage should contain item 'New' with id 2 and process id 12
			"
				.Execute(In.Context<BugSyncActionSteps>().And<EntityStateChangedHandlerSpecs>());
		}

		[Test]
		public void ShouldCheckUpdateUnproperEntityState()
		{
			@"
				Given bugzilla profile for project 1 with process 12 created
					And set entity state 'Open' with id 1 and process id 12 to storage
					And set entity state 'Done' with id 2 and process id 12 to storage
				When handled EntityStateUpdatedMessage message with entity state id 2, process id 13, name 'New', entityTypeName 'Tp.BusinessObjects.Bug'
				Then entity states storage should contain 2 items
					And entity states storage should contain item 'Open' with id 1 and process id 12
					And entity states storage should contain item 'Done' with id 2 and process id 12
			"
				.Execute(In.Context<BugSyncActionSteps>().And<EntityStateChangedHandlerSpecs>());
		}

		[Test]
		public void ShouldCheckDelete()
		{
			@"
				Given bugzilla profile for project 1 with process 12 created
					And set entity state 'Open' with id 1 and process id 12 to storage
					And set entity state 'Done' with id 2 and process id 12 to storage
				When handled EntityStateDeletedMessage message with entity state id 2, process id 12, entityTypeName 'Tp.BusinessObjects.Bug'
				Then entity states storage should contain 1 items
					And entity states storage should contain item 'Open' with id 1 and process id 12
			"
				.Execute(In.Context<BugSyncActionSteps>().And<EntityStateChangedHandlerSpecs>());
		}

		[Test]
		public void ShouldCheckDeleteUnproperEntityState()
		{
			@"
				Given bugzilla profile for project 1 with process 12 created
					And set entity state 'Open' with id 1 and process id 12 to storage
					And set entity state 'Done' with id 2 and process id 12 to storage
				When handled EntityStateDeletedMessage message with entity state id 3, process id 13, entityTypeName 'Tp.BusinessObjects.Bug'
				Then entity states storage should contain 2 items
					And entity states storage should contain item 'Open' with id 1 and process id 12
					And entity states storage should contain item 'Done' with id 2 and process id 12
			"
				.Execute(In.Context<BugSyncActionSteps>().And<EntityStateChangedHandlerSpecs>());
		}

		[Test]
		public void ShouldUpdateMappingOnEntityStateUpdated()
		{
			@"
				Given bugzilla profile for project 1 with process 12 created
					And following states created in TargetProcess:
						|id|name|
						|1|Open|
					And profile has following states mapping:
						|key|value|
						|Open|{Id:1, Name:""Open""}|
				When handled EntityStateUpdatedMessage message with entity state id 1, process id 12, name 'New', entityTypeName 'Tp.BusinessObjects.Bug'
				Then states mapping should be updated as following:
						|key|value|
						|Open|{Id:1, Name:""New""}|
			"
				.Execute(In.Context<BugSyncActionSteps>().And<EntityStateChangedHandlerSpecs>().And<EntityStateMappingSpecs>());
		}

		[Test]
		public void ShouldDeleteMappingOnEntityStateDeleted()
		{
			@"
				Given bugzilla profile for project 1 with process 12 created
					And following states created in TargetProcess:
						|id|name|
						|1|Open|
						|2|Done|
					And profile has following states mapping:
						|key|value|
						|Open|{Id:1, Name:""Open""}|
						|Resolved|{Id:2, Name:""Done""}|
				When handled EntityStateDeletedMessage message with entity state id 2, process id 12, entityTypeName 'Tp.BusinessObjects.Bug'
				Then states mapping should contain 1 item
					And states mapping should be updated as following:
						|key|value|
						|Open|{Id:1, Name:""Open""}|
			"
				.Execute(In.Context<BugSyncActionSteps>().And<EntityStateChangedHandlerSpecs>().And<EntityStateMappingSpecs>());
		}
		
		[Given("set entity state with id $id and process id $processId to storage")]
		public void SetEntityStateToStorage(int id, int processId)
		{
			Profile.Get<EntityStateDTO>().Add(new EntityStateDTO {ID = id, EntityStateID = id, ProcessID = processId});
		}

		[Given("set entity state '$name' with id $id and process id $processId to storage")]
		public void SetEntityStateToStorage(string name, int id, int processId)
		{
			Profile.Get<EntityStateDTO>().Add(new EntityStateDTO
			                                  	{ID = id, EntityStateID = id, ProcessID = processId, Name = name});
		}

		[When("handled EntityStateCreatedMessage message with entity state id $id, process id $processId, name '$name', entityTypeName '$entityTypeName'")]
		public void HandleCreate(int id, int processId, string name, string entityTypeName)
		{
			var entityState = new EntityStateDTO { ID = id, EntityStateID = id, ProcessID = processId, Name = name, EntityTypeName = entityTypeName };
			TransportMock.HandleMessageFromTp(Profile, new EntityStateCreatedMessage {Dto = entityState});
		}

		[When("handled EntityStateUpdatedMessage message with entity state id $id, process id $processId, name '$name', entityTypeName '$entityTypeName'")]
		public void HandleUpdate(int id, int processId, string name, string entityTypeName)
		{
			var entityState = new EntityStateDTO { ID = id, EntityStateID = id, ProcessID = processId, Name = name, EntityTypeName = entityTypeName };
			TransportMock.HandleMessageFromTp(Profile, new EntityStateUpdatedMessage {Dto = entityState});
		}

		[When("handled EntityStateDeletedMessage message with entity state id $id, process id $processId, entityTypeName '$entityTypeName'")]
		public void HandleDelete(int id, int processId, string entityTypeName)
		{
			var entityState = new EntityStateDTO { ID = id, EntityStateID = id, ProcessID = processId, EntityTypeName = entityTypeName };
			TransportMock.HandleMessageFromTp(Profile, new EntityStateDeletedMessage {Dto = entityState});
		}

		[Then("states mapping should contain $count item")]
		public void CheckStatesMappingCount(int count)
		{
			Profile.GetProfile<BugzillaProfile>().StatesMapping.Count.Should(Be.EqualTo(count), "Profile.GetProfile<BugzillaProfile>().StatesMapping.Count.Should(Be.EqualTo(count))");
		}

		[Then("states mapping should be updated as following:")]
		public void CheckMapping(string key, string value)
		{
			var tpValue = JsonConvert.DeserializeObject<MappingLookup>(value);

			Profile.GetProfile<BugzillaProfile>().StatesMapping.Single(
				m => m.Key == key && m.Value.Id == tpValue.Id).Value.Name.Should(Be.EqualTo(tpValue.Name), "Profile.GetProfile<BugzillaProfile>().StatesMapping.Single(m => m.Key == key && m.Value.Id == tpValue.Id).Value.Name.Should(Be.EqualTo(tpValue.Name))");
		}
	}
}