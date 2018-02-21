// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using NBehave.Narrator.Framework;
using NUnit.Framework;
using Tp.BugTracking.Commands.Dtos;
using Tp.Bugzilla.CustomCommand.Dtos;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Plugin.Common.Mapping;
using Tp.Testing.Common.NBehave;
using Tp.Testing.Common.NUnit;

namespace Tp.Bugzilla.Tests.Synchronization.Mapping
{
    [TestFixture, ActionSteps]
    [Category("PartPlugins1")]
    public class RolesMappingSpecs : MappingTestBase<RoleDTO>
    {
        [Test]
        public void ShouldCreateAssignmentWhenRolesMapped()
        {
            @"
				Given Role 'Developer' created in TargetProcess
					And Role 'QA Engineer' created in TargetProcess
					And Role 'Team Lead' created in TargetProcess
					And following users created in TargetProcess:
						|userLogin|userEmail|
						|Johnson|Johnson@mail.com|
						|Lansie|Lansie@mail.com|
						|Dowson|Dowson@mail.com|
					And bugzilla profile created
					And profile role mapping is the following:
						|key|value|
						|Assignee|Developer|
						|Reporter|QA Engineer|
					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1'
					And bug 1 has reporter 'Dowson@mail.com'
					And bug 1 has assignee 'Lansie@mail.com'
				When synchronizing bugzilla bugs
				Then bugs with following names should be created in TargetProcess: bug1
					And developer 'Lansie' should be newly assigned on TargetProcess bug with name 'bug1'
					And QA 'Dowson' should be newly assigned on TargetProcess bug with name 'bug1'
					And import should be completed
			"
                .Execute(In.Context<BugSyncActionSteps>().And<AssignmentsFromBugzillaSyncSpecs>().And<RolesMappingSpecs>());
        }

        [Test]
        public void ShouldNotCreateAssingmentWhenRoleIsNotMapped()
        {
            @"
				Given Role 'QA Engineer' created in TargetProcess
					And following users created in TargetProcess:
						|userLogin|userEmail|
						|Johnson|Johnson@mail.com|
						|Lansie|Lansie@mail.com|
						|Dowson|Dowson@mail.com|
					And bugzilla profile created
					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1'
					And bug 1 has reporter 'Dowson@mail.com'
					And bug 1 has assignee 'Lansie@mail.com'
				When synchronizing bugzilla bugs
				Then bugs with following names should be created in TargetProcess: bug1
					And TargetProcess bug with name 'bug1' should have no developer assigned
					And TargetProcess bug with name 'bug1' should no QA assigned
					And import should be completed
			"
                .Execute(In.Context<BugSyncActionSteps>().And<AssignmentsFromBugzillaSyncSpecs>().And<RolesMappingSpecs>());
        }

        [Test]
        public void ShouldUpdateRolesMappingIfRoleIsUpdated()
        {
            @"
				Given Role 'Developer' created in TargetProcess
					And Role 'QA Engineer' created in TargetProcess
					And bugzilla profile created
					And profile role mapping is the following:
						|key|value|
						|Assignee|Developer|
						|Reporter|QA Engineer|
				When role 'QA Engineer' has been renamed to 'Verifier' in TargetProcess
				Then resulting mapping is the following:
						|key|value|
						|Assignee|Developer|
						|Reporter|Verifier|
			"
                .Execute(In.Context<BugSyncActionSteps>().And<AssignmentsFromBugzillaSyncSpecs>().And<RolesMappingSpecs>());
        }

        [Test]
        public void ShouldDeleteRolesMappingIfRoleIsDeleted()
        {
            @"
				Given Role 'Developer' created in TargetProcess
					And Role 'QA Engineer' created in TargetProcess
					And bugzilla profile created
					And profile role mapping is the following:
						|key|value|
						|Assignee|Developer|
						|Reporter|QA Engineer|
				When role 'QA Engineer' has been removed in TargetProcess
				Then resulting mapping count should be equal to bugzilla roles count
					And resulting mapping is the following:
						|key|value|
						|Assignee|Developer|
			"
                .Execute(In.Context<BugSyncActionSteps>().And<AssignmentsFromBugzillaSyncSpecs>().And<RolesMappingSpecs>());
        }

        #region MappingTestBase

        protected override MappingSourceEntry Source
        {
            get { return Context.MappingSource.Roles; }
        }

        protected override MappingContainer Mapping
        {
            get { return Profile.GetProfile<BugzillaProfile>().RolesMapping; }
        }

        protected override List<RoleDTO> StoredEntities
        {
            get { return Context.Roles; }
        }

        protected override MappingContainer GetFromMappings(Mappings mappings)
        {
            return null;
        }

        protected override Func<int, string, RoleDTO> CreateEntityForTargetProcess
        {
            get { return (id, name) => new RoleDTO { ID = id, Name = name }; }
        }

        #endregion

        [Given("profile role mapping is the following:")]
        public void CreateRoleMapping(string key, string value)
        {
            var role = Context.Roles
                .Where(x => x.Name == value)
                .Select(x => new MappingLookup { Id = x.ID.Value, Name = value })
                .Single();
            Profile.GetProfile<BugzillaProfile>().RolesMapping.Add(new MappingElement { Key = key, Value = role });
        }

        [When("role '$roleName' has been renamed to '$newRoleName' in TargetProcess")]
        public void RenameRole(string roleName, string newRoleName)
        {
            var role = Context.Roles.Where(x => x.Name == roleName).Single();
            TransportMock.HandleMessageFromTp(new RoleUpdatedMessage
            {
                Dto = new RoleDTO { ID = role.ID, Name = newRoleName },
                ChangedFields = new[] { RoleField.Name }
            });
        }

        [When("role '$roleName' has been removed in TargetProcess")]
        public void RemoveRole(string roleName)
        {
            var role = Context.Roles.Where(x => x.Name == roleName).Single();
            Context.Roles.Remove(role);
            TransportMock.HandleMessageFromTp(new RoleDeletedMessage
            {
                Dto = role
            });
        }

        [Then("resulting mapping is the following:")]
        public void CheckMappingResults(string key, string value)
        {
            Profile.GetProfile<BugzillaProfile>().RolesMapping[key].Name.Should(Be.EqualTo(value),
                "Profile.GetProfile<BugzillaProfile>().RolesMapping[key].Name.Should(Be.EqualTo(value))");
        }

        [Then("resulting mapping count should be equal to bugzilla roles count")]
        public void CheckMappingsItemsCount()
        {
            Profile.GetProfile<BugzillaProfile>()
                .RolesMapping.Count.Should(Be.EqualTo(Context.Roles.Count),
                    "Profile.GetProfile<BugzillaProfile>().RolesMapping.Count.Should(Be.EqualTo(Context.Roles.Count))");
        }
    }
}
