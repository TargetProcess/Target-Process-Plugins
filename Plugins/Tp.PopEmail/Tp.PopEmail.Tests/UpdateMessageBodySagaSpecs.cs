// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Linq;
using NBehave.Narrator.Framework;
using NServiceBus.Saga;
using NUnit.Framework;
using StructureMap;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Messages.TargetProcessLifecycle;
using Tp.Integration.Testing.Common.Persisters;
using Tp.Plugin.Core;
using Tp.PopEmailIntegration.Sagas;
using Tp.Testing.Common.NBehave;
using Tp.Testing.Common.NUnit;

namespace Tp.PopEmailIntegration
{
    [TestFixture, ActionSteps]
    [Category("PartPlugins1")]
    public class UpdateMessageBodySagaSpecs
    {
        private const string EXCEPTION_STRING = "Exception";

        private static UpdateMessageBodySagaContext Context
        {
            get { return ObjectFactory.GetInstance<UpdateMessageBodySagaContext>(); }
        }

        [Test]
        public void ShouldUpdateMessageBodyIfAttachmentFound()
        {
            Context.Transport.On<UpdateCommand>(x => x.Dto is MessageDTO).Reply(
                x => new MessageUpdatedMessage { Dto = x.Dto as MessageDTO });

            @"Given message with body '<img src=""cid:image1.gif@01CBF375.60E6C210"">'
				And attachment with id 20 and file name 'image1.gif@01CBF375.60E6C210' with content Id 'image1.gif@01CBF375.60E6C210' was created in TP
			When UpdateMessageBodyCommandInternal received
			Then message body should be updated to '<img src=""~/Attachment.aspx?AttachmentID=20"">'"
                .Execute();
        }

        [Test]
        public void ShouldEscapeSpecialCharactersInAttachmentName()
        {
            Context.Transport.On<UpdateCommand>(x => x.Dto is MessageDTO).Reply(
                x => new MessageUpdatedMessage { Dto = x.Dto as MessageDTO });

            @"Given message with body '<img src=""cid:image1.gif@01CBF3???????75.60E6C210"">'
				And attachment with id 20 and file name 'image1.gif' with content Id 'image1.gif@01CBF3???????75.60E6C210' was created in TP
			When UpdateMessageBodyCommandInternal received
			Then message body should be updated to '<img src=""~/Attachment.aspx?AttachmentID=20"">'"
                .Execute();
        }

        [Test]
        public void ShouldNotUpdateMessageBodyIfBodyIsEmpty()
        {
            @"Given message with empty body
				And attachment with id 20 and file name 'image1.gif' with content Id 'image1.gif@01CBF375.60E6C210' was created in TP
			When UpdateMessageBodyCommandInternal received
			Then body should not be updated"
                .Execute();
        }

        [SetUp]
        public void BeforeScenarioInit()
        {
            ObjectFactory.Initialize(x => { });
        }

        [Test]
        public void ShouldNotUpdatebodyIfNoAttachmentsFound()
        {
            Context.Transport.On<UpdateCommand>(x => x.Dto is MessageDTO).Reply(
                x => new MessageUpdatedMessage { Dto = x.Dto as MessageDTO });

            @"Given message with body '<img src=""cid:image1.gif@01CBF375.60E6C210"">'
			When UpdateMessageBodyCommandInternal received
			Then message body should remain '<img src=""cid:image1.gif@01CBF375.60E6C210"">'"
                .Execute();
        }

        [Test]
        public void ShouldHandleExceptionCorrectly()
        {
            @"Given message with body '<img src=""cid:image1.gif@01CBF375.60E6C210"">'
				And attachment with id 20 and file name 'image1.gif' with content Id 'image1.gif@01CBF375.60E6C210' was created in TP
				And message body cannot be updated
			When UpdateMessageBodyCommandInternal received
			Then exception message should be sent"
                .Execute();
        }

        [Given(@"message with body '$messageBody'")]
        public void SetMessageBody(string messageBody)
        {
            Context.Command.MessageDto.Body = messageBody;
        }

        [Given("message with empty body")]
        public void SetEmptyBody()
        {
            Context.Command.MessageDto.Body = null;
        }

        [Given("attachment with id $attachmentId and file name '$attachmentFileName' with content Id '$attachmentContentId' was created in TP")]
        public void CreateAttachmentInTP(int attachmentId, string attachmentFileName, string attachmentContentId)
        {
            var attachments = new List<AttachmentDTO>(Context.Command.AttachmentDtos)
                { new AttachmentDTO { AttachmentID = attachmentId, OriginalFileName = attachmentFileName } };
            Context.Command.AttachmentDtos = attachments.ToArray();
            Context.Command.ContentIds = new Dictionary<int, string> { { attachmentId, attachmentContentId } };
        }

        [Given("message body cannot be updated")]
        public void MessageBodyCannotBeUpdated()
        {
            Context.Transport.On<UpdateCommand>(x => x.Dto is MessageDTO).Reply(
                x => new TargetProcessExceptionThrownMessage { ExceptionString = EXCEPTION_STRING });
        }

        [When("UpdateMessageBodyCommandInternal received")]
        public void SendUpdateMessageBodyCommandInternal()
        {
            Context.Transport.HandleLocalMessage(Context.Storage, Context.Command);
        }

        [Then(@"message body should be updated to '$messageBodyUpdated'")]
        public void MessageBodyShouldBeUpdated(string messageBodyUpdated)
        {
            Context.Transport.TpQueue
                .GetMessages<UpdateCommand>()
                .Count(x => x.Dto is MessageDTO)
                .Should(Be.EqualTo(1),
                    "Context.Transport.TpQueue.GetMessages<UpdateCommand>().Count(x => x.Dto is MessageDTO).Should(Be.EqualTo(1)");

            var message = Context.Transport.LocalQueue.GetMessages<MessageBodyUpdatedMessageInternal>().First();
            message.MessageDto.Body.Should(Be.EqualTo(messageBodyUpdated), "message.MessageDto.Body.Should(Be.EqualTo(messageBodyUpdated))");
            message.SagaId.Should(Be.EqualTo(Context.OuterSagaId), "message.SagaId.Should(Be.EqualTo(Context.OuterSagaId))");
        }

        [Then(@"message body should remain '$messageBody'")]
        public void MessageBodyShouldNotBeUpdated(string messageBody)
        {
            Context.Transport.TpQueue.GetMessages<UpdateCommand>()
                .Where(x => x.Dto is MessageDTO)
                .Should(Be.Empty, "Context.Transport.TpQueue.GetMessages<UpdateCommand>().Where(x => x.Dto is MessageDTO).Should(Be.Empty)");

            var message = Context.Transport.LocalQueue.GetMessages<MessageBodyUpdatedMessageInternal>().First();
            message.MessageDto.Body.Should(Be.EqualTo(messageBody), "message.MessageDto.Body.Should(Be.EqualTo(messageBody))");
            message.SagaId.Should(Be.EqualTo(Context.OuterSagaId), "message.SagaId.Should(Be.EqualTo(Context.OuterSagaId))");
        }

        [Then("exception message should be sent")]
        public void ExceptionShouldBeSent()
        {
            var exceptionMessage = Context.Transport.LocalQueue.GetMessages<ExceptionThrownLocalMessage>().First();
            exceptionMessage.ExceptionString.Should(Be.EqualTo(EXCEPTION_STRING),
                "exceptionMessage.ExceptionString.Should(Be.EqualTo(EXCEPTION_STRING))");
            exceptionMessage.SagaId.Should(Be.EqualTo(Context.OuterSagaId),
                "exceptionMessage.SagaId.Should(Be.EqualTo(Context.OuterSagaId))");
        }

        [Then("body should not be updated")]
        public void BodyShouldNotBeUpdated()
        {
            Context.Transport.TpQueue.GetMessages<UpdateCommand>()
                .Where(x => x.Dto is MessageDTO)
                .Should(Be.Empty, "Context.Transport.TpQueue.GetMessages<UpdateCommand>().Where(x => x.Dto is MessageDTO).Should(Be.Empty)");
            var message = Context.Transport.LocalQueue.GetMessages<MessageBodyUpdatedMessageInternal>().First();
            message.MessageDto.Body.Should(Be.EqualTo(null), "message.MessageDto.Body.Should(Be.EqualTo(null))");
            ObjectFactory.GetInstance<TpInMemorySagaPersister>().Get<ISagaEntity>().Should(Be.Empty, "Saga is not completed");
        }
    }
}
