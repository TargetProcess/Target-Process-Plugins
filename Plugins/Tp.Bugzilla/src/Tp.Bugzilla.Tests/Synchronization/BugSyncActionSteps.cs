// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using NBehave.Narrator.Framework;
using NServiceBus.Saga;
using StructureMap;
using Tp.Bugzilla.Synchronizer;
using Tp.Core;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Messages.Ticker;
using Tp.Integration.Plugin.Common.Mapping;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Testing.Common;
using Tp.Integration.Testing.Common.Persisters;
using Tp.Testing.Common.NUnit;

namespace Tp.Bugzilla.Tests.Synchronization
{
    [ActionSteps]
    public class BugSyncActionSteps
    {
        protected BugzillaContext Context
        {
            get { return ObjectFactory.GetInstance<BugzillaContext>(); }
        }

        protected static IProfileReadonly Profile
        {
            get { return ObjectFactory.GetInstance<IProfileReadonly>(); }
        }

        [Given("bugzilla profile for project $projectId with process $processId created")]
        public void CreateProfileWithProject(int projectId, int processId)
        {
            Context.CreateDefaultRolesIfNecessary();
            var project = new ProjectDTO { ID = projectId, ProjectID = projectId, ProcessID = processId };

            Context.AddProfileWithDefaultRolesMapping(projectId);
            Profile.Get<ProjectDTO>().Add(project);
        }

        [Given("bugzilla profile for project $projectId created")]
        public void CreateProfile(int projectId)
        {
            Context.CreateDefaultRolesIfNecessary();
            Context.AddProfileWithDefaultRolesMapping(projectId);
        }

        [Given("bugzilla profile created")]
        public void CreateDefaultProfile()
        {
            Context.AddProfile(1);
        }

        [Given("bugzilla profile with default roles mapping created")]
        public void CreateProfileWithDefaultRolesMapping()
        {
            Context.AddProfileWithDefaultRolesMapping(1);
        }

        [Given(@"TargetProcess contains following severities : (?<severities>([^,]+,?\s*)+)")]
        public void SetupSeveritiesInTargetProcess(string[] severities)
        {
            Context.Severities.AddRange(severities.Select(name =>
            {
                var pair = name.Split('-');

                return new SeverityDTO
                {
                    ID = Int32.Parse(pair[0]),
                    SeverityID = Int32.Parse(pair[0]),
                    Name = pair[1]
                };
            }));
        }

        [Given(@"TargetProcess contains following priorities : (?<priorities>([^,]+,?\s*)+)")]
        public void SetupPrioritiesForTargetProcess(string[] priorities)
        {
            Context.Priorities.AddRange(priorities.Select(name =>
            {
                var pair = name.Split('-');

                return new PriorityDTO
                {
                    ID = Int32.Parse(pair[0]),
                    PriorityID = Int32.Parse(pair[0]),
                    Name = pair[1]
                };
            }));
        }

        [Given(@"TargetProcess contains bug entity states for project $projectId : (?<entityStates>([^,]+,?\s*)+)")]
        public void SetupBugStatesInTargetProcess(int projectId, string[] entityStates)
        {
            SetupEntityStatesInTargetProcess(entityStates, BugzillaProfileInitializationSaga.BugEntityTypeName);
        }

        private void SetupEntityStatesInTargetProcess(IEnumerable<string> entityStates, string entityTypeName)
        {
            Context.EntityStates.AddRange(entityStates.Select(name =>
            {
                var pair = name.Split('-');

                return new EntityStateDTO
                {
                    ID = Int32.Parse(pair[0]),
                    EntityStateID = Int32.Parse(pair[0]),
                    Name = pair[1],
                    EntityTypeName = entityTypeName
                };
            }));
        }

        [Given(@"TargetProcess contains user story entity states for project $projectId : (?<entityStates>([^,]+,?\s*)+)")]
        public void SetupUserStoryStatesInTargetProcess(int projectId, string[] entityStates)
        {
            SetupEntityStatesInTargetProcess(entityStates, "Tp.BusinessObjects.UserStory");
        }

        [Given(@"following users created in TargetProcess:")]
        [Given("user '$userLogin' with email '$userEmail' created in TargetProcess")]
        public void SetUser(string userLogin, string userEmail)
        {
            Context.Users.Add(new UserDTO { Login = userLogin, Email = userEmail, ID = Context.GetNextId(), IsActive = true });
        }

        [Given("bug $bugId has reporter '$reporterEmail'")]
        public void SetReporter(int bugId, string reporterEmail)
        {
            Context.BugzillaBugs.SetBugReporter(bugId, reporterEmail);
        }

        [Given("bug $bugId has assignee '$assigneeEmail'")]
        public void SetAssignee(int bugId, string assigneeEmail)
        {
            Context.BugzillaBugs.SetBugAssignee(bugId, assigneeEmail);
        }

        [Given("bugzilla contains bug with id $bugId")]
        public void CreateBug(int bugId)
        {
            Context.BugzillaBugs.Add(bugId, CurrentDate.Value);
        }

        [Given("bug $bugId has empty assignee")]
        public void SetEmptyAssignee(int bugId)
        {
            Context.BugzillaBugs.SetBugAssignee(bugId, string.Empty);
        }

        [Given("bug $bugId has name '$bugName'")]
        public void SetBugName(int bugId, string bugName)
        {
            Context.BugzillaBugs.GetById(bugId).short_desc = bugName;
        }

        [Given("Role '$roleName' created in TargetProcess")]
        public void CreateRole(string roleName)
        {
            Context.CreateRole(roleName);
        }

        [Given(@"user mapping added:")]
        public void AddUserMapping(string bugzillaEmail, string targetProcessLogin)
        {
            var user = Context.Users.Single(u => u.Login == targetProcessLogin);

            if (Profile.GetProfile<BugzillaProfile>().UserMapping == null)
            {
                Profile.GetProfile<BugzillaProfile>().UserMapping = new MappingContainer();
            }

            Profile.GetProfile<BugzillaProfile>().UserMapping.Add(new MappingElement
                {
                    Key = bugzillaEmail,
                    Value =
                        new MappingLookup { Id = user.ID.Value, Name = user.Login }
                }
            );
        }

        [When("synchronizing bugzilla bugs")]
        public void SynchronizeBugs()
        {
            var lastSyncDate = ObjectFactory.GetInstance<IStorageRepository>().Get<LastSyncDate>().FirstOrDefault();

            DateTime? date;

            if (lastSyncDate == null)
            {
                date = null;
            }
            else
            {
                date = lastSyncDate.Value;
            }

            ObjectFactory.GetInstance<TransportMock>().HandleLocalMessage(Profile, new TickMessage(date));
        }

        [Then("entity states storage should contain $count items")]
        public void CheckEntityStatesCount(int count)
        {
            Profile.Get<EntityStateDTO>()
                .Count()
                .Should(Be.EqualTo(count), "Profile.Get<EntityStateDTO>().Count().Should(Be.EqualTo(count))");
        }

        [Then("entity states storage should contain item '$name' with id $id and process id $processId")]
        public void CheckEntityStateExists(string name, int id, int processId)
        {
            Profile.Get<EntityStateDTO>()
                .Where(s => s.ID == id)
                .Where(s => s.EntityStateID == id)
                .Where(s => s.ProcessID == processId)
                .Any(s => s.Name == name)
                .Should(Be.True,
                    "Profile.Get<EntityStateDTO>().Where(s => s.ID == id).Where(s => s.EntityStateID == id).Where(s => s.ProcessID == processId).Any(s => s.Name == name).Should(Be.True)");
        }

        [Then("entity states storage should contain item '$name' with id $id")]
        public void CheckEntityStateExists(string name, int id)
        {
            Profile.Get<EntityStateDTO>()
                .Where(s => s.Name == name)
                .Where(s => s.ID == id)
                .Any(s => s.EntityStateID == id)
                .Should(Be.True,
                    "Profile.Get<EntityStateDTO>().Where(s => s.Name == name).Where(s => s.ID == id).Any(s => s.EntityStateID == id).Should(Be.True)");
        }

        [Then("priorities storage should contain $count items")]
        public void CheckPrioritiesCount(int count)
        {
            Profile.Get<PriorityDTO>().Count().Should(Be.EqualTo(count), "Profile.Get<PriorityDTO>().Count().Should(Be.EqualTo(count))");
        }

        [Then("priorities storage should contain item with id $id and name '$name'")]
        public void CheckPriorityExists(int id, string name)
        {
            Profile.Get<PriorityDTO>()
                .Where(s => s.ID == id)
                .Where(s => s.PriorityID == id)
                .Any(s => s.Name == name)
                .Should(Be.True,
                    "Profile.Get<PriorityDTO>().Where(s => s.ID == id).Where(s => s.PriorityID == id).Any(s => s.Name == name).Should(Be.True)");
        }

        [Then("projects storage should contain $count items")]
        public void CheckProjectsCount(int count)
        {
            Profile.Get<ProjectDTO>().Count().Should(Be.EqualTo(count), "Profile.Get<ProjectDTO>().Count().Should(Be.EqualTo(count))");
        }

        [Then("projects storage should contain item with id $id and name '$name'")]
        public void CheckProjectExists(int id, string name)
        {
            Profile.Get<ProjectDTO>()
                .Where(s => s.ID == id)
                .Where(s => s.ProjectID == id)
                .Any(s => s.Name == name)
                .Should(Be.True,
                    "Profile.Get<ProjectDTO>().Where(s => s.ID == id).Where(s => s.ProjectID == id).Any(s => s.Name == name).Should(Be.True)");
        }

        [Then("projects storage should contain item with id $id and process $processId")]
        public void CheckProjectExists(int id, int processId)
        {
            Profile.Get<ProjectDTO>()
                .Where(s => s.ID == id)
                .Where(s => s.ProjectID == id)
                .Any(s => s.ProcessID == processId)
                .Should(Be.True,
                    "Profile.Get<ProjectDTO>().Where(s => s.ID == id).Where(s => s.ProjectID == id).Any(s => s.ProcessID == processId).Should(Be.True)");
        }

        [Then("severities storage should contain $count items")]
        public void CheckSeveritiesCount(int count)
        {
            Profile.Get<SeverityDTO>().Count().Should(Be.EqualTo(count), "Profile.Get<SeverityDTO>().Count().Should(Be.EqualTo(count))");
        }

        [Then("severities storage should contain item with id $id and name '$name'")]
        public void CheckSeverityExists(int id, string name)
        {
            Profile.Get<SeverityDTO>()
                .Where(s => s.ID == id)
                .Where(s => s.SeverityID == id)
                .Any(s => s.Name == name)
                .Should(Be.True,
                    "Profile.Get<SeverityDTO>().Where(s => s.ID == id).Where(s => s.SeverityID == id).Any(s => s.Name == name).Should(Be.True)");
        }

        [Then(@"bugs with following names should be created in TargetProcess: (?<bugNames>([^,]+,?\s*)+)")]
        public void BugShouldBeCreated(string[] bugNames)
        {
            var bugs =
                ObjectFactory.GetInstance<TransportMock>().TpQueue.GetMessages<CreateCommand>().Where(x => x.Dto is BugDTO).Select(
                        x => ((BugDTO) x.Dto).Name).
                    ToList();
            bugs.Should(Be.EquivalentTo(bugNames), "bugs.Should(Be.EquivalentTo(bugNames))");
        }

        [Then(@"bugs with following names should be updated in TargetProcess: (?<bugNames>([^,]+,?\s*)+)")]
        public void BugShouldBeUpdated(string[] bugNames)
        {
            var bugs =
                ObjectFactory.GetInstance<TransportMock>().TpQueue.GetMessages<UpdateCommand>().Where(x => x.Dto is BugDTO).Select(
                        x => ((BugDTO) x.Dto).Name).
                    ToList();
            bugs.Should(Be.EquivalentTo(bugNames), "bugs.Should(Be.EquivalentTo(bugNames))");
        }

        [Then("no bugs should be added in TargetProcess")]
        public void CheckNoBugsAdded()
        {
            var created =
                ObjectFactory.GetInstance<TransportMock>().TpQueue.GetMessages<CreateCommand>().Where(x => x.Dto is BugDTO).Select(
                        x => ((BugDTO) x.Dto).Name).
                    ToList();

            created.Any().Should(Be.False, "created.Any().Should(Be.False)");
        }

        [Then("no bugs should be updated in TargetProcess")]
        public void CheckNoBugsUpdated()
        {
            var updated =
                ObjectFactory.GetInstance<TransportMock>().TpQueue.GetMessages<UpdateCommand>().Where(x => x.Dto is BugDTO).Select(
                        x => ((BugDTO) x.Dto).Name).
                    ToList();

            updated.Any().Should(Be.False, "updated.Any().Should(Be.False)");
        }

        [Then("import should be completed")]
        public void ImportShouldBeCompleted()
        {
            ObjectFactory.GetInstance<TpInMemorySagaPersister>()
                .Get<ISagaEntity>()
                .Should(Be.Empty, "ObjectFactory.GetInstance<TpInMemorySagaPersister>().Get<ISagaEntity>().Should(Be.Empty)");
        }
    }
}
