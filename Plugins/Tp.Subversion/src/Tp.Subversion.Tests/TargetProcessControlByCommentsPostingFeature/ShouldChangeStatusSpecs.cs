// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NBehave.Narrator.Framework;
using NUnit.Framework;
using StructureMap;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Testing.Common;
using Tp.SourceControl.Comments;
using Tp.Subversion.Context;
using Tp.Subversion.StructureMap;
using Tp.Subversion.UserMappingFeature;
using Tp.Testing.Common.NBehave;
using Tp.Testing.Common.NUnit;

namespace Tp.Subversion.TargetProcessControlByCommentsPostingFeature
{
    [TestFixture, ActionSteps]
    [Category("PartPlugins1")]
    public class ShouldChangeStatusSpecs
    {
        [SetUp]
        public void Init()
        {
            ObjectFactory.Initialize(x => x.AddRegistry<VcsMockEnvironmentRegistry>());
            ObjectFactory.Configure(
                x =>
                    x.For<TransportMock>().Use(TransportMock.CreateWithoutStructureMapClear(typeof(SubversionPluginProfile).Assembly,
                        new List<Assembly> { typeof(Command).Assembly })));
        }

        [Test]
        public void ShouldChangeStatus()
        {
            @"Given tp user 'tpuser' with id 5
					And vcs user 'svnuser' mapped as 'tpuser'
					And vcs commit is: {Id:1, Comment:""#123 status:fixed"", Author:""svnuser""}
				When plugin started up
				Then entity 123 status should be changed to 'fixed' with default comment 'State is changed by 'Subversion' plugin' by the 'tpuser'"
                .Execute(
                    In.Context<VcsPluginActionSteps>()
                        .And<WhenCommitMadeByTpUserSpecs>()
                        .And<ShouldChangeStatusSpecs>()
                        .And<UserMappingFeatureActionSteps>());
        }

        [Test]
        public void ShouldChangeStatusWhenExplicitCommentSpecifiedAndCommentProvided()
        {
            @"Given tp user 'tpuser' with id 5
					And vcs user 'svnuser' mapped as 'tpuser'
					And vcs commit is: {Id:1, Comment:""#123 status:fixed comment:bla-bla comment"", Author:""svnuser""}
				When plugin started up
				Then entity 123 status should be changed to 'fixed' with comment 'bla-bla comment' by the 'tpuser'
					And no additional comments should be posted"
                .Execute(
                    In.Context<VcsPluginActionSteps>().And<WhenCommitMadeByTpUserSpecs>().And<ShouldChangeStatusSpecs>().And
                        <ShouldPostCommentSpecs>().And<UserMappingFeatureActionSteps>());
        }

        [Test]
        public void ShouldChangeStatusWhenExplicitCommentSpecifiedAndCommentProvided1()
        {
            @"Given tp user 'tpuser' with id 5
					And vcs user 'svnuser' mapped as 'tpuser'
					And vcs commit is: {Id:1, Comment:""#123 status:fixed, comment:bla-bla comment"", Author:""svnuser""}
				When plugin started up
				Then entity 123 status should be changed to 'fixed' with comment 'bla-bla comment' by the 'tpuser'
					And no additional comments should be posted"
                .Execute(
                    In.Context<VcsPluginActionSteps>().And<WhenCommitMadeByTpUserSpecs>().And<ShouldChangeStatusSpecs>().And
                        <ShouldPostCommentSpecs>().And<UserMappingFeatureActionSteps>());
        }

        [Test]
        public void ShouldChangeStatusWhenImplicitCommentSpecifiedAndNoExplicitCommentProvided()
        {
            @"Given tp user 'tpuser' with id 5
					And vcs user 'svnuser' mapped as 'tpuser'
					And vcs commit is: {Id:1, Comment:""#123 status:fixed"", Author:""svnuser""}
				When plugin started up
				Then entity 123 status should be changed to 'fixed' with default comment 'State is changed by 'Subversion' plugin' by the 'tpuser'"
                .Execute(
                    In.Context<VcsPluginActionSteps>()
                        .And<WhenCommitMadeByTpUserSpecs>()
                        .And<ShouldChangeStatusSpecs>()
                        .And<UserMappingFeatureActionSteps>());
        }

        [Test]
        public void ShouldChangeStatusToMultipleEntities()
        {
            @"Given tp user 'tpuser' with id 5
					And vcs user 'svnuser' mapped as 'tpuser'
					And vcs commit is: {Id:1, Comment:""#123 status:ready to be fixed #33 status: updated"", Author:""svnuser""}
				When plugin started up
				Then entity 123 status should be changed to 'ready to be fixed' by the 'tpuser'
					And entity 33 status should be changed to 'updated' by the 'tpuser'"
                .Execute(
                    In.Context<VcsPluginActionSteps>()
                        .And<WhenCommitMadeByTpUserSpecs>()
                        .And<ShouldChangeStatusSpecs>()
                        .And<UserMappingFeatureActionSteps>());
        }

        [Test]
        public void ShouldNotChangeStatusIfParametersMissed()
        {
            @"Given vcs commit is: {Id:1, Comment:""#123 status:fixed #33"", Author:""svnuser""}
				When plugin started up
				Then status should not be changed"
                .Execute(
                    In.Context<VcsPluginActionSteps>()
                        .And<WhenCommitMadeByTpUserSpecs>()
                        .And<ShouldChangeStatusSpecs>()
                        .And<UserMappingFeatureActionSteps>());
        }

        [Test]
        public void ShouldNotChangeStatusIfCommentHasTypo()
        {
            @"Given tp user 'tpuser' with id 5
					And vcs user 'svnuser' mapped as 'tpuser'
					And vcs commit is: {Id:1, Comment:""#123 statuss:fixed"", Author:""svnuser""}
				When plugin started up
				Then status should not be changed"
                .Execute(
                    In.Context<VcsPluginActionSteps>()
                        .And<WhenCommitMadeByTpUserSpecs>()
                        .And<ShouldChangeStatusSpecs>()
                        .And<UserMappingFeatureActionSteps>());
        }

        [Test]
        public void ShouldChangeStatusIfStatusConsistsOfMultipleWords()
        {
            @"Given tp user 'tpuser' with id 5
					And vcs user 'svnuser' mapped as 'tpuser'
					And vcs commit is: {Id:1, Comment:""#11 state:Ready to test"", Author:""svnuser""}
				When plugin started up
				Then entity 11 status should be changed to 'Ready to test' with default comment 'State is changed by 'Subversion' plugin' by the 'tpuser'"
                .Execute(
                    In.Context<VcsPluginActionSteps>()
                        .And<WhenCommitMadeByTpUserSpecs>()
                        .And<ShouldChangeStatusSpecs>()
                        .And<UserMappingFeatureActionSteps>());
        }


        [Then("entity $entityId status should be changed to '(?<status>[^\']+)' by the '$tpuser'")]
        public void EntityStatusShouldBeChanged(int entityId, string status, string tpuser)
        {
            var command =
                Context.Transport.TpQueue.GetMessages<ChangeEntityStateCommand>().Where(x => x.EntityId == entityId).Single();
            AssertCommand(entityId, status, tpuser, command);
        }

        private static void AssertCommand(int entityId, string status, string tpuser, ChangeEntityStateCommand command)
        {
            command.EntityId.Should(Be.EqualTo(entityId), "command.EntityId.Should(Be.EqualTo(entityId))");

            var user = Context.GetTpUserByName(tpuser);

            command.UserID.Should(Be.EqualTo(user.Id), "command.UserID.Should(Be.EqualTo(user.Id))");
            command.State.Should(Be.EqualTo(status), "command.State.Should(Be.EqualTo(status))");
        }

        [Then("entity $entityId status should be changed to '$status.*?' with comment '$comment.*?' by the '$tpuser'")]
        public void EntityStatusShouldBeChangedWithComment(int entityId, string status, string comment, string tpuser)
        {
            var command = Context.Transport.TpQueue.GetMessages<ChangeEntityStateCommand>().Where(x => x.EntityId == entityId).Single();

            AssertCommand(entityId, status, tpuser, command);
            command.Comment.Should(Be.EqualTo(comment), "command.Comment.Should(Be.EqualTo(comment))");
        }

        [Then("entity $entityId status should be changed to '$status.*?' with default comment '$comment.*?' by the '$tpuser'")
        ]
        public void EntityStatusShouldBeChangedWithDefaultComment(int entityId, string status, string comment, string tpuser)
        {
            var command = Context.Transport.TpQueue.GetMessages<ChangeEntityStateCommand>().Where(x => x.EntityId == entityId).Single();

            AssertCommand(entityId, status, tpuser, command);
            command.Comment.Should(Be.Null, "command.Comment.Should(Be.Null)");
        }

        [Then("status should not be changed")]
        public void StatusShouldNotBeChanged()
        {
            Context.Transport.TpQueue.GetMessages<ChangeEntityStateCommand>()
                .ToArray()
                .Should(Be.Empty, "Context.Transport.TpQueue.GetMessages<ChangeEntityStateCommand>().ToArray().Should(Be.Empty)");
        }

        private static VcsPluginContext Context
        {
            get { return ObjectFactory.GetInstance<VcsPluginContext>(); }
        }
    }
}
