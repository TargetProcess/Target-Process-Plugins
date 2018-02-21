using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Tp.Testing.Common.NBehave;

namespace Tp.Tfs.Tests.WorkItems
{
    [TestFixture]
    [Category("PartPlugins1")]
    public class WorkItemsSpecs
    {
        [Test]
        [Ignore]
        public void PluginShouldImportBugTest()
        {
            @"
				Given tfs profile 'TestProfile' created 
					And work items store contains entity with name 'bug1' and description 'important bug' and type 'Bug'
				When synchronizing work items entities
				Then bugs with following names should be created in TargetProcess: 'bug1'
					And bug in TargetProcess with name 'bug1' should have description 'important bug'
					And bug in TargetProcess with name 'bug1' should be in 'Test'"
                .Execute(In.Context<WorkItemsActionSteps>());
        }

        [Test]
        [Ignore]
        public void PluginShouldImportUserStoryTest()
        {
            @"
				Given tfs profile 'TestProfile' created 
					And work items store contains entity with name 'userStory1' and description 'important user story' and type 'User Story'
				When synchronizing work items entities
				Then user stories with following names should be created in TargetProcess: 'userStory1'
					And user story in TargetProcess with name 'userStory1' should have description 'important user story'
					And user story in TargetProcess with name 'userStory1' should be in 'Test'"
                .Execute(In.Context<WorkItemsActionSteps>());
        }

        [Test]
        [Ignore]
        public void PluginShouldImportFeatureTest()
        {
            @"
				Given tfs profile 'TestProfile' created 
					And work items store contains entity with name 'task1' and description 'important task' and type 'Task'
				When synchronizing work items entities
				Then features with following names should be created in TargetProcess: 'task1'
					And feature in TargetProcess with name 'task1' should have description 'important task'
					And feature in TargetProcess with name 'task1' should be in 'Test'"
                .Execute(In.Context<WorkItemsActionSteps>());
        }

        [Test]
        [Ignore]
        public void PluginShouldImportRequestTest()
        {
            @"
				Given tfs profile 'TestProfile' created 
					And work items store contains entity with name 'issue1' and description 'important issue' and type 'Issue'
				When synchronizing work items entities
				Then requests with following names should be created in TargetProcess: 'issue1'
					And request in TargetProcess with name 'issue1' should have description 'important issue'
					And request in TargetProcess with name 'issue1' should be in 'Test'"
                .Execute(In.Context<WorkItemsActionSteps>());
        }

        [Test]
        [Ignore]
        public void PluginShouldUpdateTpEntitiesTest()
        {
            @"
				Given tfs profile 'TestProfile' created 
					And work items store contains entity with name 'userStory1' and description 'important user story' and type 'User Story'
                    And work items were synchronized
                    And user stories with following names should be created in TargetProcess: 'userStory1'
                    And user stories in TargetProcess with name 'userStory1' should have description 'important user story'
					And user stories in TargetProcess with name 'userStory1' should be in 'Test'
                    And work item 'userStory1' field 'Description' changed for 'description changed'
                    And work item 'userStory1' field 'Title' changed for 'userStory1changed'
                When synchronizing work items entities
				Then user story with name 'userStory1' is absent in TP
                    And user stories with following names should be created in TargetProcess: 'userStory1changed'
					And user stories in TargetProcess with name 'userStory1changed' should have description 'description changed'
					And user stories in TargetProcess with name 'userStory1changed' should be in 'Test'"
                .Execute(In.Context<WorkItemsActionSteps>());
        }

        [Test]
        [Ignore]
        public void PluginShouldUpdateTpEntitiesAfterChangingInTpTest()
        {
            @"
				Given tfs profile 'TestProfile' created 
					And work items store contains entity with name 'userStory1' and description 'important user story' and type 'User Story'
                    And work items were synchronized
                    And user stories with following names should be created in TargetProcess: 'userStory1'
                    And user stories in TargetProcess with name 'userStory1' should have description 'important user story'
					And user stories in TargetProcess with name 'userStory1' should be in 'Test'
                    And work item 'userStory1' field 'Description' changed for 'description changed'
                    And work item 'userStory1' field 'Title' changed for 'userStory1changed'
                    And tp entity 'userStory1' field 'Name' changed for 'userStoryNameChanged'
                When synchronizing work items entities
				Then user story with name 'userStory1' is absent in TP
                    And user stories with following names should be created in TargetProcess: 'userStory1changed'
					And user stories in TargetProcess with name 'userStory1changed' should have description 'description changed'
					And user stories in TargetProcess with name 'userStory1changed' should be in 'Test'"
                .Execute(In.Context<WorkItemsActionSteps>());
        }
    }
}
