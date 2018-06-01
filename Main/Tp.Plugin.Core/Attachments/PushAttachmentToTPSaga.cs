// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.IO;
using NServiceBus;
using NServiceBus.Saga;
using StructureMap;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Messages.EntityLifecycle.Commands;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Messages.TargetProcessLifecycle;
using Tp.Integration.Plugin.Common;
using AttachmentPartAddedMessage = Tp.Integration.Messages.EntityLifecycle.Commands.AttachmentPartAddedMessage;

namespace Tp.Plugin.Core.Attachments
{
    public class PushAttachmentToTPSaga
        : TpSaga<PushAttachmentToTPSagaData>,
          IAmStartedByMessages<PushAttachmentToTPCommandInternal>,
          IHandleMessages<AttachmentPartAddedMessage>,
          IHandleMessages<AttachmentCreatedMessage>,
          IHandleMessages<TargetProcessExceptionThrownMessage>
    {
        public override void ConfigureHowToFindSaga()
        {
            ConfigureMapping<AttachmentPartAddedMessage>(
                saga => saga.Id,
                message => message.SagaId
            );
            ConfigureMapping<AttachmentCreatedMessage>(
                saga => saga.Id,
                message => message.SagaId
            );
            ConfigureMapping<TargetProcessExceptionThrownMessage>(
                saga => saga.Id,
                message => message.SagaId
            );
        }

        public void Handle(PushAttachmentToTPCommandInternal message)
        {
            Data.OuterSagaId = message.OuterSagaId;
            Data.MessageId = message.MessageId;
            Data.GeneralId = message.GeneralId;
            Data.LocalStoredAttachment = message.LocalStoredAttachment;

            PushAttachmentPartToTP(String.Empty);
        }

        private void PushAttachmentPartToTP(string tpStorageFileName)
        {
            Send(GetAddAttachmentPartToMessageCommand(tpStorageFileName));
        }

        public void Handle(AttachmentPartAddedMessage message)
        {
            PushAttachmentPartToTP(message.TPStorageFileName);
        }

        public void Handle(AttachmentCreatedMessage message)
        {
            SendLocal(new AttachmentCreatedMessageInternal { SagaId = Data.OuterSagaId, AttachmentDto = message.Dto, ContentId = Data.LocalStoredAttachment.ContentId });
            MarkAsComplete();
        }

        public void Handle(TargetProcessExceptionThrownMessage message)
        {
            SendLocal(ExceptionThrownLocalMessage.Create(message, Data.OuterSagaId));
            MarkAsComplete();
        }

        private AddAttachmentPartToMessageCommand GetAddAttachmentPartToMessageCommand(string tpStorageFileName)
        {
            var addAttachmentPartToMessageCommand = new AddAttachmentPartToMessageCommand
            {
                FileName = Data.LocalStoredAttachment.FileName,
                TPStorageFileName = tpStorageFileName,
                MessageId = Data.MessageId,
                GeneralId = Data.GeneralId,
                OwnerId = Data.LocalStoredAttachment.OwnerId,
                Description = Data.LocalStoredAttachment.Description,
                CreateDate = Data.LocalStoredAttachment.CreateDate
            };

            var maxBufferSize = ObjectFactory.GetInstance<IBufferSize>().Value;

            using (var fileStream = new FileStream(AttachmentFolder.GetAttachmentFileFullPath(Data.LocalStoredAttachment.FileId),
                FileMode.Open, FileAccess.Read,
                FileShare.None))
            {
                var bytesCountLeftToSend = fileStream.Length - Data.CurrentPosition;

                var size = bytesCountLeftToSend > maxBufferSize ? maxBufferSize : (int) bytesCountLeftToSend;
                var bytes = new byte[size];
                fileStream.Seek(Data.CurrentPosition, SeekOrigin.Begin);

                fileStream.Read(bytes, 0, size);

                addAttachmentPartToMessageCommand.BytesSerializedToBase64 = Convert.ToBase64String(bytes);
                addAttachmentPartToMessageCommand.IsLastPart = fileStream.Position == fileStream.Length;

                Data.CurrentPosition = fileStream.Position;
            }
            return addAttachmentPartToMessageCommand;
        }
    }

    [Serializable]
    public class AttachmentCreatedMessageInternal : SagaMessage, IPluginLocalMessage
    {
        public AttachmentDTO AttachmentDto { get; set; }
        public string ContentId { get; set; }
    }

    public class PushAttachmentToTPSagaData : ISagaEntity
    {
        public Guid Id { get; set; }
        public string Originator { get; set; }
        public string OriginalMessageId { get; set; }
        public Guid OuterSagaId { get; set; }
        public int? MessageId { get; set; }
        public int? GeneralId { get; set; }
        public LocalStoredAttachment LocalStoredAttachment { get; set; }
        public long CurrentPosition { get; set; }
    }
}
