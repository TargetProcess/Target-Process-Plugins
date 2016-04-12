// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using NBehave.Narrator.Framework;
using NUnit.Framework;
using Tp.BugTracking.Commands.Dtos;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Plugin.Common.Mapping;
using Tp.Testing.Common.NBehave;
using Tp.Testing.Common.NUnit;

namespace Tp.Bugzilla.Tests.Synchronization.Mapping
{
	[ActionSteps]
	[TestFixture]
    [Category("PartPlugins0")]
	public class EntityStateMappingSpecs : MappingTestBase<EntityStateDTO>
	{
		[Test]
		public void ShouldMapStatesByNameDuringProfileCreation()
		{
			@"
				Given following states created in TargetProcess:
						|id|name|
						|1|Open|
						|2|In Progress|
						|3|Done|
					And following states created in Bugzilla:
						|name|
						|Open|
						|In Progress|
						|Resolved|
				When mapping states
				Then resulting mapping count should be equal to bugzilla states count
					And resulting mapping is the following:
						|key|value|
						|Open|{Id:1, Name:""Open""}|
						|In Progress|{Id:2, Name:""In Progress""}|
						|Resolved|null|
			"
				.Execute(In.Context<BugSyncActionSteps>().And<EntityStateMappingSpecs>());
		}

		[Test]
		public void ShouldMapStatesByNameDuringProfileEditing()
		{
			@"
				Given bugzilla profile created
					And profile has following states mapping:
						|key|value|
						|Open|{Id:1, Name:""Open""}|
						|Verifying|{Id:3, Name:""Testing""}|
					And following states created in TargetProcess:
						|id|name|
						|1|Open|
						|2|In Progress|
						|3|Testing|
						|4|Done|
					And following states created in Bugzilla:
						|name|
						|Open|
						|In Progress|
						|Verifying|
						|Resolved|
				When mapping states
				Then resulting mapping count should be equal to bugzilla states count
					And resulting mapping is the following:
						|key|value|
						|Open|{Id:1, Name:""Open""}|
						|In Progress|{Id:2, Name:""In Progress""}|
						|Verifying|{Id:3, Name:""Testing""}|
						|Resolved|null|
			"
				.Execute(In.Context<BugSyncActionSteps>().And<EntityStateMappingSpecs>());
		}

		[Test]
		public void ShouldValidateAndMapStatesByNameDuringProfileEditing()
		{
			@"
				Given bugzilla profile created
					And profile has following states mapping:
						|key|value|
						|Open|{Id:1, Name:""Open""}|
						|Verifying|{Id:3, Name:""Testing""}|
					And following states created in TargetProcess:
						|id|name|
						|1|Open|
						|2|In Progress|
						|3|Testing|
						|4|Done|	
					And following states created in Bugzilla:
						|name|
						|Open|
						|In Progress|
						|VerifyingUpdated|
						|Resolved|
				When mapping states
				Then resulting mapping count should be equal to bugzilla states count
					And resulting mapping is the following:
						|key|value|
						|Open|{Id:1, Name:""Open""}|
						|In Progress|{Id:2, Name:""In Progress""}|
						|VerifyingUpdated|null|
						|Resolved|null|
			"
				.Execute(In.Context<BugSyncActionSteps>().And<EntityStateMappingSpecs>());
		}

		[Test]
		public void ShouldClearEntityStateMappingIfProjectsProcessChanged()
		{
			@"
				Given bugzilla profile for project 1 with process 1 created
					And profile has following states mapping:
						|key|value|
						|Open|{Id:1, Name:""Open""}|
						|Verifying|{Id:3, Name:""Testing""}|
				When process of project '1' has been changed to '2' in TargetProcess
				Then entity states mapping should be cleared
			"
				.Execute(In.Context<EntityStateMappingSpecs>().And<BugSyncActionSteps>());
		}

		[Test]
		public void ShouldMapOnUpdateNotMappedValues()
		{
			@"
				Given bugzilla profile created
					And profile has following states mapping:
						|key|value|
						|New|{Id:0, Name:null}|
					And following states created in TargetProcess:
						|id|name|
						|4|New|
					And following states created in Bugzilla:
						|name|
						|New|
				When mapping states
				Then resulting mapping count should be equal to bugzilla states count
					And resulting mapping is the following:
						|key|value|
						|New|{Id:4, Name:""New""}|
			"
				.Execute(In.Context<BugSyncActionSteps>().And<EntityStateMappingSpecs>());
		}

		[Given("following states created in TargetProcess:")]
		public void CreateStatesInTargetProcess(int id, string name)
		{
			CreateEntityInTargetProcessBase(id, name);
		}

		[Given("following states created in Bugzilla:")]
		public void CreateStatesInBugzilla(string name)
		{
			CreateInBugzillaBase(name);
		}

		[Given("profile has following states mapping:")]
		public void CreateStatesStatesMappingForProfile(string key, string value)
		{
			CreateMappingForProfileBase(key, value);
		}

		[When("mapping states")]
		public void Map()
		{
			MapBase();
		}

		[When("process of project '$projectId' has been changed to '$newProcess1' in TargetProcess")]
		public void ChangeProjectsProcess(int projectId, int newProcessId)
		{
			TransportMock.HandleMessageFromTp(new ProjectUpdatedMessage
			                                  	{
			                                  		Dto = new ProjectDTO {ID = projectId, ProcessID = newProcessId},
			                                  		ChangedFields = new[] {ProjectField.ProcessID}
			                                  	});
		}

		[Then("resulting mapping count should be equal to bugzilla states count")]
		public void CheckMappingsItemsCount()
		{
			CheckMappingsItemsCountBase();
		}

		[Then("resulting mapping is the following:")]
		public void CheckMappingResults(string key, string value)
		{
			CheckMappingResultBase(key, value);
		}

		[Then("entity states mapping should be cleared")]
		public void EntityStatesMappingShouldBeEmpty()
		{
			Profile.GetProfile<BugzillaProfile>().StatesMapping.Count.Should(Be.EqualTo(0), "Profile.GetProfile<BugzillaProfile>().StatesMapping.Count.Should(Be.EqualTo(0))");
		}

		protected override MappingSourceEntry Source
		{
			get { return Context.MappingSource.States; }
		}

		protected override MappingContainer Mapping
		{
			get { return Profile.GetProfile<BugzillaProfile>().StatesMapping; }
		}

		protected override List<EntityStateDTO> StoredEntities
		{
			get { return Context.EntityStates; }
		}

		protected override MappingContainer GetFromMappings(Mappings mappings)
		{
			return mappings.States;
		}

		protected override Func<int, string, EntityStateDTO> CreateEntityForTargetProcess
		{
			get { return (id, name) => new EntityStateDTO {EntityStateID = id, Name = name}; }
		}
	}
}