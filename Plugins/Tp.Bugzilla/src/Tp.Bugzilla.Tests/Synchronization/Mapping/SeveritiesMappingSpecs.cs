// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using NBehave.Narrator.Framework;
using NUnit.Framework;
using Tp.Bugzilla.CustomCommand.Dtos;
using Tp.Integration.Common;
using Tp.Integration.Plugin.Common.Mapping;
using Tp.Testing.Common.NBehave;

namespace Tp.Bugzilla.Tests.Synchronization.Mapping
{
	[ActionSteps]
	[TestFixture]
	public class SeveritiesMappingSpecs : MappingTestBase<SeverityDTO> 
	{
		[Test]
		public void ShouldMapSeveritiesByNameDuringProfileCreation()
		{
			@"
				Given following severities created in TargetProcess:
						|id|name|
						|1|Blocking|
						|2|Normal|
						|3|Enhancement|
					And following severities created in Bugzilla:
						|name|
						|Blocking|
						|Normal|
						|Minor|
				When mapping severities
				Then resulting mapping count should be equal to bugzilla severities count
					And resulting mapping is the following:
						|key|value|
						|Blocking|{Id:1, Name:""Blocking""}|
						|Normal|{Id:2, Name:""Normal""}|
						|Minor|null|
			"
				.Execute(In.Context<BugSyncActionSteps>().And<SeveritiesMappingSpecs>());
		}

		[Test]
		public void ShouldMapSeveritiesByNameDuringProfileEditing()
		{
			@"
				Given bugzilla profile created
					And profile has following severities mapping:
						|key|value|
						|Blocking|{Id:1, Name:""Blocking""}|
						|Normal|{Id:2, Name:""Normal""}|
						|Minor|{Id:3, Name:""Enhancement""}|
					And following severities created in TargetProcess:
						|id|name|
						|1|Blocking|
						|2|Normal|
						|3|Enhancement|
						|4|Major|
					And following severities created in Bugzilla:
						|name|
						|Blocking|
						|Normal|
						|Minor|
						|Critical|
				When mapping severities
				Then resulting mapping count should be equal to bugzilla severities count
					And resulting mapping is the following:
						|key|value|
						|Blocking|{Id:1, Name:""Blocking""}|
						|Normal|{Id:2, Name:""Normal""}|
						|Minor|{Id:3, Name:""Enhancement""}|
						|Critical|null|
			"
				.Execute(In.Context<BugSyncActionSteps>().And<SeveritiesMappingSpecs>());
		}

		[Test]
		public void ShouldValidateAndMapSeveritiesByNameDuringProfileEditing()
		{
			@"
				Given bugzilla profile created
					And profile has following severities mapping:
						|key|value|
						|Open|{Id:1, Name:""Open""}|
						|Verifying|{Id:3, Name:""Testing""}|
					And following severities created in TargetProcess:
						|id|name|
						|1|Open|
						|2|In Progress|
						|3|Testing|
						|4|Done|	
					And following severities created in Bugzilla:
						|name|
						|Open|
						|In Progress|
						|VerifyingUpdated|
						|Resolved|
				When mapping severities
				Then resulting mapping count should be equal to bugzilla severities count
					And resulting mapping is the following:
						|key|value|
						|Open|{Id:1, Name:""Open""}|
						|In Progress|{Id:2, Name:""In Progress""}|
						|VerifyingUpdated|null|
						|Resolved|null|
			"
				.Execute(In.Context<BugSyncActionSteps>().And<SeveritiesMappingSpecs>());
		}

		[Given("following severities created in TargetProcess:")]
		public void CreateSeveritiesInTargetProcess(int id, string name)
		{
			CreateEntityInTargetProcessBase(id, name);
		}

		[Given("following severities created in Bugzilla:")]
		public void CreateSeveritiesInBugzilla(string name)
		{
			CreateInBugzillaBase(name);
		}

		[Given("profile has following severities mapping:")]
		public void CreateSeveritiesMappingForProfile(string key, string value)
		{
			CreateMappingForProfileBase(key, value);
		}

		[When("mapping severities")]
		public void Map()
		{
			MapBase();
		}

		[Then("resulting mapping count should be equal to bugzilla severities count")]
		public void CheckMappingsItemsCount()
		{
			CheckMappingsItemsCountBase();
		}

		[Then("resulting mapping is the following:")]
		public void CheckMappingResults(string key, string value)
		{
			CheckMappingResultBase(key, value);
		}

		protected override MappingSourceEntry Source
		{
			get { return Context.MappingSource.Severities; }
		}

		protected override MappingContainer Mapping
		{
			get { return Profile.GetProfile<BugzillaProfile>().SeveritiesMapping; }
		}

		protected override List<SeverityDTO> StoredEntities
		{
			get { return Context.Severities; }
		}

		protected override MappingContainer GetFromMappings(Mappings mappings)
		{
			return mappings.Severities;
		}

		protected override Func<int, string, SeverityDTO> CreateEntityForTargetProcess
		{
			get { return (id, name) => new SeverityDTO { SeverityID = id, Name = name }; }
		}
	}
}