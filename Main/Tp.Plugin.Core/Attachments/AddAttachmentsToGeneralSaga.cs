// 
// Copyright (c) 2005-2019 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using NServiceBus;
using NServiceBus.Saga;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Messages.EntityLifecycle.Commands;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Messages.TargetProcessLifecycle;
using Tp.Integration.Plugin.Common;

namespace Tp.Plugin.Core.Attachments
{
    public class AddAttachmentsToGeneralSaga
        : TpSaga<AddAttachmentToGeneralSagaData>,
          IAmStartedByMessages<AddAttachmentsToGeneralCommandInternal>,
          IHandleMessages<AttachmentCreatedMessage>,
          IHandleMessages<TargetProcessExceptionThrownMessage>
    {
        public override void ConfigureHowToFindSaga()
        {
            ConfigureMapping<AttachmentCreatedMessage>(
                saga => saga.Id,
                message => message.SagaId
            );
            ConfigureMapping<TargetProcessExceptionThrownMessage>(
                saga => saga.Id,
                message => message.SagaId
            );
        }

        public void Handle(AddAttachmentsToGeneralCommandInternal message)
        {
            Data.Attachments = message.Attachments;
            Data.GeneralId = message.GeneralId;
            Data.OuterSagaId = message.OuterSagaId;
            Data.CreatedAttachments = new AttachmentDTO[] { };
            foreach (var attachmentDto in message.Attachments)
            {
                var clonedAttachment = Clone(attachmentDto);
                if (message.GeneralId != null)
                {
                    clonedAttachment.GeneralID = message.GeneralId;
                    clonedAttachment.MessageID = null;
                }
                Send(new CloneAttachmentCommand { Dto = clonedAttachment });
            }

            if (!message.Attachments.Any())
            {
                SendAttachmentsAddedMessage(new AttachmentDTO[] { });
            }
        }

        private static AttachmentDTO Clone(AttachmentDTO attachmentDto)
        {
            return new AttachmentDTO
            {
                OriginalFileName = attachmentDto.OriginalFileName,
                UniqueFileName = attachmentDto.UniqueFileName,
                CreateDate = attachmentDto.CreateDate,
                Description = attachmentDto.Description,
                OwnerID = attachmentDto.OwnerID,
                AttachmentFileID = attachmentDto.AttachmentFileID,
                MessageID = attachmentDto.MessageID
            };
        }

        public void Handle(AttachmentCreatedMessage message)
        {
            Log().InfoFormat("Attachment {0} was added to general with id {1}",
                message.Dto.OriginalFileName, message.Dto.GeneralID);
            Data.ProcessedAttachmentsCount++;
            Data.CreatedAttachments = Data.CreatedAttachments.Concat(new[] { message.Dto }).ToArray();

            if (Data.Attachments.Count() == Data.ProcessedAttachmentsCount)
            {
                SendAttachmentsAddedMessage(Data.CreatedAttachments);
            }
        }

        private void SendAttachmentsAddedMessage(AttachmentDTO[] attachments)
        {
            SendLocal(new AttachmentsAddedToGeneralMessage { SagaId = Data.OuterSagaId, Attachments = attachments });
            MarkAsComplete();
        }

        public void Handle(TargetProcessExceptionThrownMessage message)
        {
            Log().Error($"Failed to add attachments to general with id {Data.GeneralId}", message.GetException());
            MarkAsComplete();
        }
    }

    [Serializable]
    public class AddAttachmentsToGeneralCommandInternal : IPluginLocalMessage
    {
        public AttachmentDTO[] Attachments { get; set; }
        public int? GeneralId { get; set; }
        public Guid OuterSagaId { get; set; }
    }

    [Serializable]
    public class AttachmentsAddedToGeneralMessage : SagaMessage, IPluginLocalMessage
    {
        public AttachmentDTO[] Attachments { get; set; }
    }

    public class AddAttachmentToGeneralSagaData : ISagaEntity
    {
        public Guid Id { get; set; }
        public string Originator { get; set; }
        public string OriginalMessageId { get; set; }
        public Guid OuterSagaId { get; set; }
        public int ProcessedAttachmentsCount { get; set; }
        public AttachmentDTO[] Attachments { get; set; }
        public AttachmentDTO[] CreatedAttachments { get; set; }
        public int? GeneralId { get; set; }
    }
}
