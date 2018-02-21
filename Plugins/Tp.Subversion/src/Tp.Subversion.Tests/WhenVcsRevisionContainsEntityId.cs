// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NBehave.Narrator.Framework;
using NUnit.Framework;
using StructureMap;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Messages.TargetProcessLifecycle;
using Tp.Integration.Testing.Common;
using Tp.SourceControl.Comments;
using Tp.Subversion.Context;
using Tp.Subversion.StructureMap;
using Tp.Testing.Common.NBehave;
using Tp.Testing.Common.NUnit;

namespace Tp.Subversion
{
    [TestFixture, ActionSteps]
    [Category("PartPlugins1")]
    public class WhenVcsRevisionContainsEntityId
    {
        [SetUp]
        public void Init()
        {
            ObjectFactory.Initialize(x => x.AddRegistry<VcsMockEnvironmentRegistry>());
            ObjectFactory.Configure(
                x =>
                    x.For<TransportMock>()
                        .Use(TransportMock.CreateWithoutStructureMapClear(typeof(SubversionPluginProfile).Assembly,
                            new List<Assembly> { typeof(Command).Assembly })));
        }

        [Test]
        public void ShouldAttachRevisionToEntity()
        {
            @"Given vcs history is:
					|commit|
					|{Id:1, Comment:""id:1"", Author:""John"",Time:""10.01.2000"", Entries:[{Path:""/root/file1.txt"", Action:""Add""}]}|
					And profile Start Revision is 1
				When plugin started up
				Then revision 1 should be attached to TP entity 1
					And there should be no uncompleted attach to entity sagas"
                .Execute(In.Context<VcsPluginActionSteps>().And<WhenVcsRevisionContainsEntityId>());
        }

        [Test]
        public void ShouldAttachRevisionToEntityIgnoreCase()
        {
            @"Given vcs history is:
					|commit|
					|{Id:1, Comment:""ID:1"", Author:""John"",Time:""10.01.2000"", Entries:[{Path:""/root/file1.txt"", Action:""Add""}]}|
					And profile Start Revision is 1
				When plugin started up
				Then revision 1 should be attached to TP entity 1
					And there should be no uncompleted attach to entity sagas"
                .Execute(In.Context<VcsPluginActionSteps>().And<WhenVcsRevisionContainsEntityId>());
        }

        [Test]
        public void ShouldAttachRevisionToSeveralEntities()
        {
            @"Given vcs history is:
					|commit|
					|{Id:1, Comment:""user story id:1 and bug id:2 should be affected"", Author:""John"",Time:""10.01.2000"", Entries:[{Path:""/root/file1.txt"", Action:""Add""}]}|
					And profile Start Revision is 1
				When plugin started up
				Then revision 1 should be attached to TP entity 1
					And revision 1 should be attached to TP entity 2
					And there should be no uncompleted attach to entity sagas"
                .Execute(In.Context<VcsPluginActionSteps>().And<WhenVcsRevisionContainsEntityId>());
        }

        [Test]
        public void ShouldAttachRevisionToSeveralEntitiesDelimitedByComma()
        {
            @"Given vcs history is:
					|commit|
					|{Id:1, Comment:""#33971, #34396, #722, #721"", Author:""John"",Time:""10.01.2000"", Entries:[{Path:""/root/file1.txt"", Action:""Add""}]}|
					And profile Start Revision is 1
				When plugin started up
				Then revision 1 should be attached to TP entity 33971
					And revision 1 should be attached to TP entity 34396
					And revision 1 should be attached to TP entity 722
					And revision 1 should be attached to TP entity 721
					And there should be no uncompleted attach to entity sagas"
                .Execute(In.Context<VcsPluginActionSteps>().And<WhenVcsRevisionContainsEntityId>());
        }

        [Test]
        public void ShouldHandleFailureOfAttachRevisionToEntityOperation()
        {
            @"Given vcs history is:
					|commit|
					|{Id:1, Comment:""id:1"", Author:""John"",Time:""10.01.2000"", Entries:[{Path:""/root/file1.txt"", Action:""Add""}]}|
					And profile Start Revision is 1
					And TP will fail to attach revision to TP entity
				When plugin started up
				Then there should be no uncompleted attach to entity sagas"
                .Execute(In.Context<VcsPluginActionSteps>().And<WhenVcsRevisionContainsEntityId>());
        }

        [Test]
        public void ShouldHandleParserFailure()
        {
            @"Given vcs history is:
					|commit|
					|{Id:1, Comment:"""", Author:""John"",Time:""10.01.2000"", Entries:[{Path:""/root/file1.txt"", Action:""Add""}]}|
					And profile Start Revision is 1
				When plugin started up
				Then there should be no uncompleted attach to entity sagas"
                .Execute(In.Context<VcsPluginActionSteps>().And<WhenVcsRevisionContainsEntityId>());
        }

        private static VcsPluginContext Context
        {
            get { return ObjectFactory.GetInstance<VcsPluginContext>(); }
        }

        [Then("revision $revisionId should be attached to TP entity $entityId")]
        public void RevisionShouldBeAttachedToEntity(string revisionId, int entityId)
        {
            var revisionDto = Context.Transport.LocalQueue.GetMessages<RevisionCreatedMessage>().Select(msg => msg.Dto)
                .Single(x => x.SourceControlID.ToString() == revisionId);

            var assignableDtos = Context.Transport.TpQueue.GetCreatedDtos<RevisionAssignableDTO>()
                .Where(x => x.RevisionID == revisionDto.ID).ToList();

            assignableDtos
                .Should(Be.Not.Empty, "Revision {0} wasn't attached to any entity", revisionId);

            assignableDtos
                .SingleOrDefault(x => x.AssignableID == entityId)
                .Should(Be.Not.Null, "Revision {0} wasn't attach to entity {1}", revisionId, entityId);
        }

        [Given("TP will fail to attach revision to TP entity")]
        public void TpShouldFailToAttachRevisionToTpEntity()
        {
            Context.On.CreateCommand<RevisionAssignableDTO>().Reply(x => new TargetProcessExceptionThrownMessage(new Exception()));
        }
    }
}
