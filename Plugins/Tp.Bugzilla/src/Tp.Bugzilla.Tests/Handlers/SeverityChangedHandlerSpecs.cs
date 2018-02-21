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
    public class SeverityChangedHandlerSpecs : BugzillaTestBase
    {
        [Test]
        public void ShouldCheckCreate()
        {
            @"
				Given bugzilla profile created
					And set severity with id 1 and name 'must' to storage
					And set severity with id 2 and name 'nice to have' to storage
				When handled SeverityCreatedMessage message with severity id 3, name 'medium'
				Then severities storage should contain 3 items
					And severities storage should contain item with id 1 and name 'must'
					And severities storage should contain item with id 2 and name 'nice to have'
					And severities storage should contain item with id 3 and name 'medium'
			"
                .Execute(In.Context<BugSyncActionSteps>().And<SeverityChangedHandlerSpecs>());
        }

        [Test]
        public void ShouldCheckUpdate()
        {
            @"
				Given bugzilla profile created
					And set severity with id 1 and name 'must' to storage
					And set severity with id 2 and name 'nice to have' to storage
				When handled SeverityUpdatedMessage message with severity id 2, name 'medium'
				Then severities storage should contain 2 items
					And severities storage should contain item with id 1 and name 'must'
					And severities storage should contain item with id 2 and name 'medium'
			"
                .Execute(In.Context<BugSyncActionSteps>().And<SeverityChangedHandlerSpecs>());
        }

        [Test]
        public void ShouldCheckDelete()
        {
            @"
				Given bugzilla profile created
					And set severity with id 1 and name 'must' to storage
					And set severity with id 2 and name 'nice to have' to storage
				When handled SeverityDeletedMessage message with severity id 2
				Then severities storage should contain 1 items
					And severities storage should contain item with id 1 and name 'must'
			"
                .Execute(In.Context<BugSyncActionSteps>().And<SeverityChangedHandlerSpecs>());
        }

        [Test]
        public void ShouldUpdateMappingOnPriorityUpdated()
        {
            @"
				Given bugzilla profile created
					And set severity with id 1 and name 'must' to storage
					And set severity with id 2 and name 'nice to have' to storage
					And profile has following severities mapping:
						|key|value|
						|must|{Id:1, Name:""must""}|
						|nice to have|{Id:2, Name:""nice to have""}|
				When handled SeverityUpdatedMessage message with severity id 2, name 'medium'
				Then severities mapping contains 2 items
					And severities mapping should be updated as following:
						|key|value|
						|must|{Id:1, Name:""must""}|
						|nice to have|{Id:2, Name:""medium""}|
			"
                .Execute(In.Context<BugSyncActionSteps>().And<SeveritiesMappingSpecs>().And<SeverityChangedHandlerSpecs>());
        }

        [Test]
        public void ShouldDeleteMappingOnEntityStateDeleted()
        {
            @"
				Given bugzilla profile created
					And set severity with id 1 and name 'must' to storage
					And set severity with id 2 and name 'nice to have' to storage
					And profile has following severities mapping:
						|key|value|
						|must|{Id:1, Name:""must""}|
						|nice to have|{Id:2, Name:""nice to have""}|
				When handled SeverityDeletedMessage message with severity id 2
				Then severities mapping contains 1 items
					And severities mapping should be updated as following:
						|key|value|
						|must|{Id:1, Name:""must""}|
			"
                .Execute(In.Context<BugSyncActionSteps>().And<SeveritiesMappingSpecs>().And<SeverityChangedHandlerSpecs>());
        }

        [Given("set severity with id $id and name '$name' to storage")]
        public void SetSeverityToStorage(int id, string name)
        {
            Profile.Get<SeverityDTO>().Add(new SeverityDTO { ID = id, SeverityID = id, Name = name });
        }

        [When("handled SeverityCreatedMessage message with severity id $id, name '$name'")]
        public void HandleCreate(int id, string name)
        {
            var severity = new SeverityDTO { ID = id, SeverityID = id, Name = name };
            TransportMock.HandleMessageFromTp(Profile, new SeverityCreatedMessage { Dto = severity });
        }

        [When("handled SeverityUpdatedMessage message with severity id $id, name '$name'")]
        public void HandleUpdate(int id, string name)
        {
            var severity = new SeverityDTO { ID = id, SeverityID = id, Name = name };
            TransportMock.HandleMessageFromTp(Profile, new SeverityUpdatedMessage { Dto = severity });
        }

        [When("handled SeverityDeletedMessage message with severity id $id")]
        public void HandleDelete(int id)
        {
            var severity = new SeverityDTO { ID = id, SeverityID = id };
            TransportMock.HandleMessageFromTp(Profile, new SeverityDeletedMessage { Dto = severity });
        }

        [Then("severities mapping should be updated as following:")]
        public void CheckMapping(string key, string value)
        {
            var tpValue = JsonConvert.DeserializeObject<MappingLookup>(value);

            Profile.GetProfile<BugzillaProfile>().SeveritiesMapping
                .Single(m => m.Key == key && m.Value.Id == tpValue.Id)
                .Value.Name.Should(Be.EqualTo(tpValue.Name),
                    "Profile.GetProfile<BugzillaProfile>().SeveritiesMapping.Single(m => m.Key == key && m.Value.Id == tpValue.Id).Value.Name.Should(Be.EqualTo(tpValue.Name))");
        }

        [Then("severities mapping contains $count items")]
        public void CheckMappingItemsCount(int count)
        {
            Profile.GetProfile<BugzillaProfile>().SeveritiesMapping
                .Count.Should(Be.EqualTo(count), "Profile.GetProfile<BugzillaProfile>().SeveritiesMapping.Count.Should(Be.EqualTo(count))");
        }
    }
}
