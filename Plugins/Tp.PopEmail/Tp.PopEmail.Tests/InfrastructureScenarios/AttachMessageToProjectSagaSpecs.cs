// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Linq;
using NBehave.Narrator.Framework;
using NUnit.Framework;
using StructureMap;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Commands;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.PopEmailIntegration.Rules.ThenClauses;
using Tp.Testing.Common.NBehave;
using Tp.Testing.Common.NUnit;

namespace Tp.PopEmailIntegration.InfrastructureScenarios
{
    public class AttachMessageToProjectSagaContext : PopEmailIntegrationContext
    {
        public AttachMessageToProjectSagaContext()
        {
            ObjectFactory.Configure(x => x.For<AttachMessageToProjectSagaContext>().Use(this));

            Transport.On<AttachMessageToGeneralCommand>().Reply(
                message => new MessageAttachedToGeneralMessage { GeneralId = message.GeneralId, MessageId = message.MessageId });
        }
    }

    [TestFixture, ActionSteps]
    [Category("PartPlugins1")]
    public class AttachMessageToProjectSagaSpecs
    {
        [Test]
        public void ShouldAttachMessageToProject()
        {
            @"Given message 2
				When AttachMessageToProjectCommand with project 1 and the message received
				Then the message should be attached to project 1
			"
                .Execute();
        }

        [Given("message $messageId")]
        public void CreateMessage(int messageId)
        {
            Context.Message = new MessageDTO { MessageID = messageId };
        }

        private static PopEmailIntegrationContext Context
        {
            get { return ObjectFactory.GetInstance<AttachMessageToProjectSagaContext>(); }
        }

        [When("AttachMessageToProjectCommand with project $projectId and the message received")]
        public void AttachMessageToProject(int projectId)
        {
            Context.Transport.HandleLocalMessage(Context.Storage,
                new AttachMessageToProjectCommand { MessageDto = Context.Message, ProjectId = projectId });
        }

        [Then("the message should be attached to project $projectId")]
        public void MessageShouldBeAttachedToProject(int projectId)
        {
            var message = Context.Transport.TpQueue.GetMessages<AttachMessageToGeneralCommand>().First();
            message.MessageId.Should(Be.EqualTo(Context.Message.ID), "message.MessageId.Should(Be.EqualTo(Context.Message.ID))");
            message.GeneralId.Should(Be.EqualTo(projectId), "message.GeneralId.Should(Be.EqualTo(projectId))");
        }
    }
}
