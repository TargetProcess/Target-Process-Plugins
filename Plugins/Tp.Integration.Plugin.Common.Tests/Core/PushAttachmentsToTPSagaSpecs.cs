//
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NBehave.Narrator.Framework;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Commands;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Messages.TargetProcessLifecycle;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Tests.Common;
using Tp.Integration.Plugin.Common.Tests.Common.PluginCommand;
using Tp.Integration.Testing.Common;
using Tp.Plugin.Core;
using Tp.Plugin.Core.Attachments;
using Tp.Testing.Common.NBehave;
using Tp.Testing.Common.NUnit;
using AttachmentPartAddedMessage = Tp.Integration.Messages.EntityLifecycle.Commands.AttachmentPartAddedMessage;

namespace Tp.Integration.Plugin.Common.Tests.Core
{
    [TestFixture, ActionSteps]
    [Category("PartPlugins1")]
    public class PushAttachmentsToTpSagaSpecs
    {
        private readonly Guid _outerSagaId = Guid.NewGuid();
        private const int MESSAGE_ID = 100;
        private readonly List<LocalStoredAttachment> _emailAttachments = new List<LocalStoredAttachment>();

        [BeforeScenario]
        public void BeforeScenario()
        {
            var transportMock = TransportMock.CreateWithoutStructureMapClear(typeof(SampleProfileSerialized).Assembly,
                new List<Assembly>
                    { typeof(ExceptionThrownLocalMessage).Assembly },
                new[] { typeof(WhenAddANewProfileSpecs).Assembly });

            transportMock.AddProfile("Profile_1");

            var mockBufferSize = MockRepository.GenerateStub<IBufferSize>();
            mockBufferSize.Stub(x => x.Value).Return(2);
            ObjectFactory.Configure(x => x.For<IBufferSize>().HybridHttpOrThreadLocalScoped().Use(mockBufferSize));

            var counter = 0;
            ObjectFactory.GetInstance<TransportMock>().On<AddAttachmentPartToMessageCommand>()
                .Reply(x =>
                {
                    if (!x.IsLastPart)
                    {
                        return new AttachmentPartAddedMessage { FileName = x.FileName };
                    }
                    return new AttachmentCreatedMessage
                    {
                        Dto = new AttachmentDTO { OriginalFileName = x.FileName, OwnerID = x.OwnerId, Description = x.Description, AttachmentID = ++counter}
                    };
                });
            Directory.Delete(ObjectFactory.GetInstance<PluginDataFolder>().Path, true);
        }

        [TearDown]
        public void TearDown()
        {
            Directory.Delete(ObjectFactory.GetInstance<PluginDataFolder>().Path, true);
        }

        [Test]
        public void ShouldPushAttachments()
        {
            @"Given email has attachment 'file1' with content '12345'
				And email has attachment 'file2' with content 'abcdef'
			When PushEmailAttachmentsToTpCommandInternal command arrived
			Then attachment 'file1' with content '12345' should be created
				And attachment 'file2' with content 'abcdef' should be created
				And attachment folder should be empty"
                .Execute();
        }

        [Test]
        public void ShouldClearAttachmentFolderIfTargetProcessThrowsException()
        {
            @"Given email has attachment 'file1' with content '12345'
				And email has attachment 'file2' with content 'abcdef'
				And TargetProcess is down
			When PushEmailAttachmentsToTpCommandInternal command arrived
			Then attachment folder should be empty"
                .Execute();
        }

        [Given("email has attachment '$attachmentName' with content '$attachmentContent'")]
        public void AddAttachmentToEmail(string attachmentName, string attachmentContent)
        {
            byte[] byteArray = Encoding.ASCII.GetBytes(attachmentContent);
            var stream = new MemoryStream(byteArray);

            var fileId = AttachmentFolder.Save(stream);

            var emailAttachment = new LocalStoredAttachment { FileId = fileId, FileName = attachmentName };

            _emailAttachments.Add(emailAttachment);
        }

        [Given("TargetProcess is down")]
        public void SetTargetProcessDown()
        {
            Transport.ResetAllOnMessageHandlers();
            ObjectFactory.GetInstance<TransportMock>().On<AddAttachmentPartToMessageCommand>()
                .Reply(
                    x => new TargetProcessExceptionThrownMessage()
                );
        }

        [When("PushEmailAttachmentsToTpCommandInternal command arrived")]
        public void PushEmailAttachmentsToTpCommandInternalArrived()
        {
            Transport.HandleLocalMessage(ObjectFactory.GetInstance<IProfileReadonly>(), new PushAttachmentsToTpCommandInternal
            {
                OuterSagaId = _outerSagaId,
                MessageId = MESSAGE_ID,
                LocalStoredAttachments =
                    _emailAttachments.ToArray()
            });
        }

        private static TransportMock Transport
        {
            get { return ObjectFactory.GetInstance<TransportMock>(); }
        }

        [Then("attachment '$attachmentName' with content '$attachmentContent' should be created")]
        public void AttachmentShouldBeCreated(string attachmentName, string attachmentContent)
        {
            var message = Transport.LocalQueue.GetMessages<AttachmentsPushedToTPMessageInternal>().First();
            message.AttachmentDtos.FirstOrDefault(x => x.OriginalFileName == attachmentName)
                .Should(Be.Not.Null, "message.AttachmentDtos.FirstOrDefault(x => x.OriginalFileName == attachmentName).Should(Be.Not.Null)");
            message.SagaId.Should(Be.EqualTo(_outerSagaId), "message.SagaId.Should(Be.EqualTo(_outerSagaId))");

            var parts =
                Transport.TpQueue.GetMessages<AddAttachmentPartToMessageCommand>().Where(x => x.FileName == attachmentName);

            var content = new StringBuilder();
            foreach (var part in parts)
            {
                content.Append(Encoding.ASCII.GetString(Convert.FromBase64String(part.BytesSerializedToBase64)));
            }

            content.ToString().Should(Be.EqualTo(attachmentContent), "content.ToString().Should(Be.EqualTo(attachmentContent))");
        }

        [Then("attachment folder should be empty")]
        public void AttachmentFolderShouldBeEmpty()
        {
            Directory.GetFiles(ObjectFactory.GetInstance<PluginDataFolder>().Path)
                .Should(Be.Empty, "Directory.GetFiles(ObjectFactory.GetInstance<PluginDataFolder>().Path).Should(Be.Empty)");
        }
    }
}
