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
    [Category("PartPlugins1")]
    public class PriorityChangedHandlerTest : BugzillaTestBase
    {
        [Test]
        public void ShouldCheckCreate()
        {
            @"
				Given bugzilla profile created
					And set priority with id 1 and name 'max' to storage
					And set priority with id 2 and name 'min' to storage
				When handled PriorityCreatedMessage message with priority id 3, name 'medium', entity type name 'Tp.BusinessObjects.Bug'
				Then priorities storage should contain 3 items
					And priorities storage should contain item with id 1 and name 'max'
					And priorities storage should contain item with id 2 and name 'min'
					And priorities storage should contain item with id 3 and name 'medium'
			"
                .Execute(In.Context<BugSyncActionSteps>().And<PriorityChangedHandlerTest>());
        }

        [Test]
        public void ShouldCheckUpdate()
        {
            @"
				Given bugzilla profile created
					And set priority with id 1 and name 'max' to storage
					And set priority with id 2 and name 'min' to storage
				When handled PriorityUpdatedMessage message with priority id 2, name 'medium', entity type name 'Tp.BusinessObjects.Bug'
				Then priorities storage should contain 2 items
					And priorities storage should contain item with id 1 and name 'max'
					And priorities storage should contain item with id 2 and name 'medium'
			"
                .Execute(In.Context<BugSyncActionSteps>().And<PriorityChangedHandlerTest>());
        }

        [Test]
        public void ShouldCheckDelete()
        {
            @"
				Given bugzilla profile created
					And set priority with id 1 and name 'max' to storage
					And set priority with id 2 and name 'min' to storage
				When handled PriorityDeletedMessage message with priority id 2, entity type name 'Tp.BusinessObjects.Bug'
				Then priorities storage should contain 1 items
					And priorities storage should contain item with id 1 and name 'max'
			"
                .Execute(In.Context<BugSyncActionSteps>().And<PriorityChangedHandlerTest>());
        }

        [Test]
        public void ShouldUpdateMappingOnPriorityUpdated()
        {
            @"
				Given bugzilla profile created
					And set priority with id 1 and name 'max' to storage
					And set priority with id 2 and name 'min' to storage
					And profile has following priorities mapping:
						|key|value|
						|max|{Id:1, Name:""max""}|
						|min|{Id:2, Name:""min""}|
				When handled PriorityUpdatedMessage message with priority id 1, name 'medium', entity type name 'Tp.BusinessObjects.Bug'
				Then priorities mapping contains 2 items
					And priorities mapping should be updated as following:
						|key|value|
						|max|{Id:1, Name:""medium""}|
						|min|{Id:2, Name:""min""}|
			"
                .Execute(In.Context<BugSyncActionSteps>().And<PrioritiesMappingSpecs>().And<PriorityChangedHandlerTest>());
        }

        [Test]
        public void ShouldDeleteMappingOnEntityStateDeleted()
        {
            @"
				Given bugzilla profile created
					And set priority with id 1 and name 'max' to storage
					And set priority with id 2 and name 'min' to storage
					And profile has following priorities mapping:
						|key|value|
						|max|{Id:1, Name:""max""}|
						|min|{Id:2, Name:""min""}|
				When handled PriorityDeletedMessage message with priority id 2, entity type name 'Tp.BusinessObjects.Bug'
				Then priorities mapping contains 1 items
					And priorities mapping should be updated as following:
						|key|value|
						|max|{Id:1, Name:""max""}|
			"
                .Execute(In.Context<BugSyncActionSteps>().And<PrioritiesMappingSpecs>().And<PriorityChangedHandlerTest>());
        }

        [Test]
        public void ShouldNotHandleMessageForNonBugEntityType()
        {
            @"
				Given bugzilla profile created
					And set priority with id 1 and name 'max' to storage
					And set priority with id 2 and name 'min' to storage
				When handled PriorityCreatedMessage message with priority id 3, name 'medium', entity type name 'Tp.BusinessObjects.UserStory'
				Then priorities storage should contain 2 items
					And priorities storage should contain item with id 1 and name 'max'
					And priorities storage should contain item with id 2 and name 'min'
			"
                .Execute(In.Context<BugSyncActionSteps>().And<PrioritiesMappingSpecs>().And<PriorityChangedHandlerTest>());
        }

        [Given("set priority with id $id and name '$name' to storage")]
        public void SetPriorityToStorage(int id, string name)
        {
            Profile.Get<PriorityDTO>()
                .Add(new PriorityDTO { ID = id, PriorityID = id, Name = name, EntityTypeName = "Tp.BusinessObjects.Bug" });
        }

        [When("handled PriorityCreatedMessage message with priority id $id, name '$name', entity type name '$entityTypeName'")]
        public void HandleCreate(int id, string name, string entityTypeName)
        {
            var priority = new PriorityDTO { ID = id, PriorityID = id, Name = name, EntityTypeName = entityTypeName };
            TransportMock.HandleMessageFromTp(Profile, new PriorityCreatedMessage { Dto = priority });
        }

        [When("handled PriorityUpdatedMessage message with priority id $id, name '$name', entity type name '$entityTypeName'")]
        public void HandleUpdate(int id, string name, string entityTypeName)
        {
            var priority = new PriorityDTO { ID = id, PriorityID = id, Name = name, EntityTypeName = entityTypeName };
            TransportMock.HandleMessageFromTp(Profile, new PriorityUpdatedMessage { Dto = priority });
        }

        [When("handled PriorityDeletedMessage message with priority id $id, entity type name '$entityTypeName'")]
        public void HandleDelete(int id, string entityTypeName)
        {
            var priority = new PriorityDTO { ID = id, PriorityID = id, EntityTypeName = entityTypeName };
            TransportMock.HandleMessageFromTp(Profile, new PriorityDeletedMessage { Dto = priority });
        }

        [Then("priorities mapping should be updated as following:")]
        public void CheckMapping(string key, string value)
        {
            var tpValue = JsonConvert.DeserializeObject<MappingLookup>(value);

            Profile.GetProfile<BugzillaProfile>().PrioritiesMapping
                .Single(m => m.Key == key && m.Value.Id == tpValue.Id)
                .Value.Name.Should(Be.EqualTo(tpValue.Name),
                    "Profile.GetProfile<BugzillaProfile>().PrioritiesMapping.Single(m => m.Key == key && m.Value.Id == tpValue.Id).Value.Name.Should(Be.EqualTo(tpValue.Name))");
        }

        [Then("priorities mapping contains $count items")]
        public void CheckMappingItemsCount(int count)
        {
            Profile.GetProfile<BugzillaProfile>().PrioritiesMapping
                .Count.Should(Be.EqualTo(count), "Profile.GetProfile<BugzillaProfile>().PrioritiesMapping.Count.Should(Be.EqualTo(count))");
        }
    }
}
