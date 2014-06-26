// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using NBehave.Narrator.Framework;
using NUnit.Framework;
using Tp.BugTracking.Commands.Dtos;
using Tp.Bugzilla.CustomCommand.Dtos;
using Tp.Integration.Common;
using Tp.Integration.Plugin.Common.Mapping;
using Tp.Testing.Common.NBehave;

namespace Tp.Bugzilla.Tests.Synchronization.Mapping
{
	[ActionSteps]
	[TestFixture]
    [Category("PartPlugins0")]
	public class PrioritiesMappingSpecs : MappingTestBase<PriorityDTO>
	{
		[Test]
		public void ShouldMapPrioritiesByNameDuringProfileCreation()
		{
			@"
				Given following priorities created in TargetProcess:
						|id|name|
						|1|Must Have|
						|2|Fix ASAP|
						|3|Normal|
					And following priorities created in Bugzilla:
						|name|
						|Must Have|
						|High|
				When mapping priorities
				Then resulting mapping count should be equal to bugzilla priorities count
					And resulting mapping is the following:
						|key|value|
						|Must Have|{Id:1, Name:""Must Have""}|
						|High|null|
			"
				.Execute(In.Context<BugSyncActionSteps>().And<PrioritiesMappingSpecs>());
		}

		[Test]
		public void ShouldMapPrioritiesByNameDuringProfileEditing()
		{
			@"
				Given bugzilla profile created
					And profile has following priorities mapping:
						|key|value|
						|Must Have|{Id:1, Name:""Must Have""}|
						|Normal|{Id:2, Name:""Normal""}|
					And following priorities created in TargetProcess:
						|id|name|
						|1|Must Have|
						|2|Normal|
						|3|Great|
					And following priorities created in Bugzilla:
						|name|
						|Must Have|
						|Normal|
						|Great|
						|Average|
				When mapping priorities
				Then resulting mapping count should be equal to bugzilla priorities count
					And resulting mapping is the following:
						|key|value|
						|Must Have|{Id:1, Name:""Must Have""}|
						|Normal|{Id:2, Name:""Normal""}|
						|Great|{Id:3, Name:""Great""}|
						|Average|null|
			"
				.Execute(In.Context<BugSyncActionSteps>().And<PrioritiesMappingSpecs>());
		}

		[Test]
		public void ShouldValidateAndMapPrioritiesByNameDuringProfileEditing()
		{
			@"
				Given bugzilla profile created
					And profile has following priorities mapping:
						|key|value|
						|Must Have|{Id:1, Name:""Must Have""}|
						|Minor|{Id:3, Name:""Minor""}|
					And following priorities created in TargetProcess:
						|id|name|
						|1|Must Have|
						|2|Minor|
						|3|Average|
					And following priorities created in Bugzilla:
						|name|
						|Must Have|
						|Average|
						|Normal|
				When mapping priorities
				Then resulting mapping count should be equal to bugzilla priorities count
					And resulting mapping is the following:
						|key|value|
						|Must Have|{Id:1, Name:""Must Have""}|
						|Average|{Id:3, Name:""Average""}|
						|Normal|null|
			"
				.Execute(In.Context<BugSyncActionSteps>().And<PrioritiesMappingSpecs>());
		}

		[Given("following priorities created in TargetProcess:")]
		public void CreatePrioritiesInTargetProcess(int id, string name)
		{
			CreateEntityInTargetProcessBase(id, name);
		}

		[Given("following priorities created in Bugzilla:")]
		public void CreatePrioritiesInBugzilla(string name)
		{
			CreateInBugzillaBase(name);
		}

		[Given("profile has following priorities mapping:")]
		public void CreatePrioritiesMappingForProfile(string key, string value)
		{
			CreateMappingForProfileBase(key, value);
		}

		[When("mapping priorities")]
		public void Map()
		{
			MapBase();
		}

		[Then("resulting mapping count should be equal to bugzilla priorities count")]
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
			get { return Context.MappingSource.Priorities; }
		}

		protected override MappingContainer Mapping
		{
			get { return Profile.GetProfile<BugzillaProfile>().PrioritiesMapping; }
		}

		protected override List<PriorityDTO> StoredEntities
		{
			get { return Context.Priorities; }
		}

		protected override MappingContainer GetFromMappings(Mappings mappings)
		{
			return mappings.Priorities;
		}

		protected override Func<int, string, PriorityDTO> CreateEntityForTargetProcess
		{
			get { return (id, name) => new PriorityDTO { PriorityID = id, Name = name }; }
		}
	}
}