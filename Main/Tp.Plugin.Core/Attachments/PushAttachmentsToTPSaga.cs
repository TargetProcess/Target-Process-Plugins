// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using NServiceBus;
using NServiceBus.Saga;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Plugin.Common;

namespace Tp.Plugin.Core.Attachments
{
    public class PushAttachmentsToTpSaga
        : TpSaga<PushAttachmentsToTPSagaData>,
          IAmStartedByMessages<PushAttachmentsToTpCommandInternal>,
          IHandleMessages<AttachmentCreatedMessageInternal>,
          IHandleMessages<ExceptionThrownLocalMessage>
    {
        public override void ConfigureHowToFindSaga()
        {
            ConfigureMapping<AttachmentCreatedMessageInternal>(
                saga => saga.Id,
                message => message.SagaId
            );
            ConfigureMapping<ExceptionThrownLocalMessage>(
                saga => saga.Id,
                message => message.SagaId
            );
        }

        public void Handle(PushAttachmentsToTpCommandInternal message)
        {
            Data.OuterSagaId = message.OuterSagaId;
            Data.AttachmentsCount = message.LocalStoredAttachments.Count();
            Data.MessageId = message.MessageId;
            Data.FileIds = message.LocalStoredAttachments.Select(a => a.FileId);

            message.LocalStoredAttachments.ToList().ForEach(
                x =>
                    SendLocal(new PushAttachmentToTPCommandInternal
                    {
                        OuterSagaId = Data.Id,
                        LocalStoredAttachment = x,
                        MessageId = message.MessageId,
                        GeneralId = message.GeneralId,
                    }));

            if (message.LocalStoredAttachments.Length == 0)
            {
                SendSuccessfullMessage();
            }
        }

        public void Handle(AttachmentCreatedMessageInternal message)
        {
            var attachments = new List<AttachmentDTO>(Data.ProcessedAttachments) { message.AttachmentDto };
            Data.ProcessedAttachments = attachments.ToArray();

            var contentIds = new List<KeyValuePair<int, string>>(Data.ContentIds) { new KeyValuePair<int, string>(message.AttachmentDto.AttachmentID.Value, message.ContentId) };
            Data.ContentIds = contentIds.ToDictionary(x => x.Key, x => x.Value);

            if (Data.ProcessedAttachments.Length == Data.AttachmentsCount)
            {
                Log().Info(
                    Data.MessageId.HasValue
                        ? $"Attachment{(Data.ProcessedAttachments.Length == 1 ? "" : "s")} {Data.ProcessedAttachments.Select(x => x.OriginalFileName).ToString(",")} {(Data.ProcessedAttachments.Length == 1 ? "was" : "were")} added to message with id {Data.MessageId}"
                        : $"Attachment{(Data.ProcessedAttachments.Length == 1 ? "" : "s")} {Data.ProcessedAttachments.Select(x => x.OriginalFileName).ToString(",")} {(Data.ProcessedAttachments.Length == 1 ? "was" : "were")} added to general with id {Data.GeneralId}");

                AttachmentFolder.Delete(Data.FileIds);

                SendSuccessfullMessage();
            }
        }

        private void SendSuccessfullMessage()
        {
            SendLocal(new AttachmentsPushedToTPMessageInternal
                { SagaId = Data.OuterSagaId, AttachmentDtos = Data.ProcessedAttachments, ContentIds = Data.ContentIds });

            MarkAsComplete();
        }

        public void Handle(ExceptionThrownLocalMessage message)
        {
            message.SagaId = Data.OuterSagaId;
            AttachmentFolder.Delete(Data.FileIds);
            SendLocal(message);
            MarkAsComplete();
        }
    }

    [Serializable]
    public class PushAttachmentToTPCommandInternal : IPluginLocalMessage
    {
        public Guid OuterSagaId { get; set; }
        public int? MessageId { get; set; }
        public int? GeneralId { get; set; }
        public LocalStoredAttachment LocalStoredAttachment { get; set; }
    }

    public class PushAttachmentsToTPSagaData : ISagaEntity
    {
        public PushAttachmentsToTPSagaData()
        {
            ProcessedAttachments = new AttachmentDTO[] { };
            ContentIds = new Dictionary<int, string>();
        }

        public Guid Id { get; set; }
        public string Originator { get; set; }
        public string OriginalMessageId { get; set; }
        public Guid OuterSagaId { get; set; }
        public AttachmentDTO[] ProcessedAttachments { get; set; }
        public Dictionary<int, string> ContentIds { get; set; }
        public int AttachmentsCount { get; set; }
        public int? MessageId { get; set; }
        public int? GeneralId { get; set; }
        public IEnumerable<FileId> FileIds { get; set; }
    }

    [Serializable]
    public class AttachmentsPushedToTPMessageInternal : SagaMessage, IPluginLocalMessage
    {
        public AttachmentDTO[] AttachmentDtos { get; set; }
        public Dictionary<int, string> ContentIds { get; set; }
    }

    [Serializable]
    public class PushAttachmentsToTpCommandInternal : IPluginLocalMessage
    {
        public Guid OuterSagaId { get; set; }
        public int? MessageId { get; set; }
        public int? GeneralId { get; set; }
        public LocalStoredAttachment[] LocalStoredAttachments { get; set; }
    }
}
