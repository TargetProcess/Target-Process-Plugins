using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using NBehave.Narrator.Framework;
using StructureMap;
using Tp.Integration.Common;
using Tp.Integration.Messages.Ticker;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Mapping;
using Tp.Integration.Plugin.Common.Storage.Persisters;
using Tp.LegacyProfileConvertsion.Common;
using Tp.Tfs.Tests.Context;
using Tp.Testing.Common.NUnit;
using Tp.Integration.Testing.Common;

namespace Tp.Tfs.Tests.WorkItems
{
    [ActionSteps]
    public class WorkItemsActionSteps
    {
        private WorkItemsContext _context;

        protected WorkItemsContext Context
        {
            get
            {
                if (_context == null)
                    _context = ObjectFactory.GetInstance<WorkItemsContext>();

                return _context;
            }
        }

        [BeforeScenario]
        public void OnBeforeScenario()
        {
            try
            {
                Context.ClearWorkItems();
                Context.ProjectsMapping.Add(new MappingElement { Key = "testProject", Value = new MappingLookup() { Id = 1, Name = "Test" } });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [AfterScenario]
        public void OnAfterScenario()
        {
            Context.ClearWorkItems();
            Context.DisposeTfs();
        }

        [When("synchronizing work items entities")]
        public void SynchronizeWorkItems()
		{
		    var lastSyncDate = ObjectFactory.GetInstance<IStorageRepository>().Get<LastSyncDate>().FirstOrDefault();

			DateTime? date;

			if (lastSyncDate == null)
				date = null;
			else
				date = lastSyncDate.Value;

			ObjectFactory.GetInstance<TransportMock>().HandleLocalMessage(Context.Profile, new TickMessage(date));
		}
        
        [Then("bugs with following names should be created in TargetProcess: '$entityName'")]
        public void BugShouldBeCreated(string entityName)
		{
            Context.TpBugs.Count(x => x.Name == entityName).Should(Be.EqualTo(1), "Context.TpBugs.Count(x => x.Name == entityName).Should(Be.EqualTo(1))");
		}

        [Then("bug in TargetProcess with name '$entityName' should have description '$description'")]
        public void CheckBugDescription(string entityName, string description)
        {
            Context.TpBugs.First(x => x.Name == entityName).Description.Should(Be.EqualTo(description), "Context.TpBugs.First(x => x.Name == entityName).Description.Should(Be.EqualTo(description))");
        }

        [Then("bug in TargetProcess with name '$entityName' should be in '$project'")]
        public void CheckBugProject(string entityName, string projectName)
        {
            Context.TpBugs.First(x => x.Name == entityName).ProjectName.Should(Be.EqualTo(projectName), "Context.TpBugs.First(x => x.Name == entityName).ProjectName.Should(Be.EqualTo(projectName))");
        }

        [Then("user stories with following names should be created in TargetProcess: '$entityName'")]
        public void UserStoryShouldBeCreated(string entityName)
        {
            Context.TpUserStories.Count(x => x.Name == entityName).Should(Be.EqualTo(1), "Context.TpUserStories.Count(x => x.Name == entityName).Should(Be.EqualTo(1))");
        }

        [Given("user stories with following names should be created in TargetProcess: '$entityName'")]
        public void UserStoryShouldBeCreated2(string entityName)
        {
            Context.TpUserStories.Count(x => x.Name == entityName).Should(Be.EqualTo(1), "Context.TpUserStories.Count(x => x.Name == entityName).Should(Be.EqualTo(1))");
        }

        [Then("user story in TargetProcess with name '$entityName' should have description '$description'")]
        public void CheckUserStoryDescription(string entityName, string description)
        {
            Context.TpUserStories.First(x => x.Name == entityName).Description.Should(Be.EqualTo(description), "Context.TpUserStories.First(x => x.Name == entityName).Description.Should(Be.EqualTo(description))");
        }

        [Given("user stories in TargetProcess with name '$entityName' should have description '$description'")]
        public void CheckUserStoryDescription2(string entityName, string description)
        {
            Context.TpUserStories.First(x => x.Name == entityName).Description.Should(Be.EqualTo(description), "Context.TpUserStories.First(x => x.Name == entityName).Description.Should(Be.EqualTo(description))");
        }

        [Then("user story in TargetProcess with name '$entityName' should be in '$project'")]
        public void CheckUserStoryProject(string entityName, string projectName)
        {
            Context.TpUserStories.First(x => x.Name == entityName).ProjectName.Should(Be.EqualTo(projectName), "Context.TpUserStories.First(x => x.Name == entityName).ProjectName.Should(Be.EqualTo(projectName))");
        }

        [Given("user stories in TargetProcess with name '$entityName' should be in '$project'")]
        public void CheckUserStoryProject2(string entityName, string projectName)
        {
            Context.TpUserStories.First(x => x.Name == entityName).ProjectName.Should(Be.EqualTo(projectName), "Context.TpUserStories.First(x => x.Name == entityName).ProjectName.Should(Be.EqualTo(projectName))");
        }

        [Then("features with following names should be created in TargetProcess: '$entityName'")]
        public void FeatureShouldBeCreated(string entityName)
        {
            Context.TpFeatures.Count(x => x.Name == entityName).Should(Be.EqualTo(1), "Context.TpFeatures.Count(x => x.Name == entityName).Should(Be.EqualTo(1))");
        }

        [Then("feature in TargetProcess with name '$entityName' should have description '$description'")]
        public void CheckFeatureDescription(string entityName, string description)
        {
            Context.TpFeatures.First(x => x.Name == entityName).Description.Should(Be.EqualTo(description), "Context.TpFeatures.First(x => x.Name == entityName).Description.Should(Be.EqualTo(description))");
        }

        [Then("feature in TargetProcess with name '$entityName' should be in '$project'")]
        public void CheckFeatureProject(string entityName, string projectName)
        {
            Context.TpFeatures.First(x => x.Name == entityName).ProjectName.Should(Be.EqualTo(projectName), "Context.TpFeatures.First(x => x.Name == entityName).ProjectName.Should(Be.EqualTo(projectName))");
        }

        [Then("requests with following names should be created in TargetProcess: '$entityName'")]
        public void RequestShouldBeCreated(string entityName)
        {
            Context.TpRequests.Count(x => x.Name == entityName).Should(Be.EqualTo(1), "Context.TpRequests.Count(x => x.Name == entityName).Should(Be.EqualTo(1))");
        }

        [Then("request in TargetProcess with name '$entityName' should have description '$description'")]
        public void CheckRequestDescription(string entityName, string description)
        {
            Context.TpRequests.First(x => x.Name == entityName).Description.Should(Be.EqualTo(description), "Context.TpRequests.First(x => x.Name == entityName).Description.Should(Be.EqualTo(description))");
        }

        [Then("request in TargetProcess with name '$entityName' should be in '$project'")]
        public void CheckRequestProject(string entityName, string projectName)
        {
            Context.TpRequests.First(x => x.Name == entityName).ProjectName.Should(Be.EqualTo(projectName), "Context.TpRequests.First(x => x.Name == entityName).ProjectName.Should(Be.EqualTo(projectName))");
        }


        [Given("work item '$entityName' field '$field' changed for '$newFieldValue'")]
        public void ChangeUserStoryTitle(string entityName, string field, string newFieldValue)
        {
            Context.ChangeWorkItem(entityName, new Dictionary<string, string> { { field, newFieldValue } });
        }
        

        [Then("user story with name '$entityName' is absent in TP")]
        public void UserStoryShouldBeAbsentInTP(string entityName)
        {
             Context.TpUserStories.Count(x => x.Name == entityName).Should(Be.EqualTo(0), "Context.TpUserStories.Count(x => x.Name == entityName).Should(Be.EqualTo(0))");
        }

        ///////////////////////////////////////////////////
        
        [Given("tfs profile '$TestProfile' created")]
        public void CreateProfile(string profileName)
        {
            Context.AddProfile(profileName);
        }

        [Given("work items store contains entity with name '$entityName' and description '$description' and type '$type'")]
        public void CreateWorkItem(string entityName, string description, string type)
        {
            Context.AddWorkItem(entityName, description, type);
            var workItem = Context.GetWorkItem(type, entityName, Context.TeamProject);

            workItem.Title.Should(Be.EqualTo(entityName), "workItem.Title.Should(Be.EqualTo(entityName))");
            workItem.Description.Should(Be.EqualTo(description), "workItem.Description.Should(Be.EqualTo(description))");
            workItem.Type.Name.Should(Be.EqualTo(type), "workItem.Type.Name.Should(Be.EqualTo(type))");
        }

        [Given("work items were synchronized")]
        public void SynchronizeWorkItems2()
        {
            var lastSyncDate = ObjectFactory.GetInstance<IStorageRepository>().Get<LastSyncDate>().FirstOrDefault();

            DateTime? date;

            if (lastSyncDate == null)
                date = null;
            else
                date = lastSyncDate.Value;

            ObjectFactory.GetInstance<TransportMock>().HandleLocalMessage(Context.Profile, new TickMessage(date));
        }

        [Given("tp entity '$entityName' field '$fieldName' changed for '$newEntityName'")]
        public void ChangeTpEntity(string entityName, string field, string newEntityName)
        {
            UserStoryDTO tpEntity = Context.TpUserStories.First(x => x.Name == entityName);
            var nameProperty = typeof (UserStoryDTO).GetProperty(field);
            nameProperty.SetValue(tpEntity, newEntityName, null);

            tpEntity.Name.Should(Be.EqualTo(newEntityName), "tpEntity.Name.Should(Be.EqualTo(newEntityName))");
        }
    }
}
