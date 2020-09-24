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
using Tp.Plugin.Core;

namespace Tp.PopEmailIntegration.Sagas
{
    public class UpdateRequestDescriptionSaga
        : TpSaga<UpdateRequestDescriptionSagaData>,
          IAmStartedByMessages<UpdateRequestDescriptionCommandInternal>,
          IHandleMessages<RequestUpdatedMessage>,
          IHandleMessages<TargetProcessExceptionThrownMessage>
    {
        public override void ConfigureHowToFindSaga()
        {
            ConfigureMapping<RequestUpdatedMessage>(
                saga => saga.Id,
                message => message.SagaId
            );
            ConfigureMapping<TargetProcessExceptionThrownMessage>(
                saga => saga.Id,
                message => message.SagaId
            );
        }

        public void Handle(UpdateRequestDescriptionCommandInternal message)
        {
            Data.OuterSagaId = message.OuterSagaId;
            var requestDescriptionUpdated = message.RequestDto.Description;
            if (message.RequestAttachmentDtos != null && !string.IsNullOrEmpty(requestDescriptionUpdated))
            {
                var requestAttachmentDtos = message.RequestAttachmentDtos.OrderBy(x => x.AttachmentID).ToList();
                foreach (var attachment in message.MessageAttachmentDtos.OrderBy(x => x.AttachmentID))
                {
                    var createdAttachment =
                        requestAttachmentDtos.FirstOrDefault(
                            a => string.CompareOrdinal(a.OriginalFileName, attachment.OriginalFileName) == 0);

                    if (createdAttachment == null) continue;

                    requestAttachmentDtos.Remove(createdAttachment);

                    var url = $"~/Attachment.aspx?AttachmentID={attachment.AttachmentID}";
                    var newUrl = $"~/Attachment.aspx?AttachmentID={createdAttachment.AttachmentID}";

                    requestDescriptionUpdated = requestDescriptionUpdated.Replace(url, newUrl);
                }
            }

            if (requestDescriptionUpdated != message.RequestDto.Description)
            {
                message.RequestDto.Description = requestDescriptionUpdated;
                Send(new UpdateRequestCommand(message.RequestDto, new Enum[] { RequestField.Description }));
            }
            else
            {
                SendLocal(new RequestDescriptionUpdatedMessageInternal { RequestDto = message.RequestDto, SagaId = Data.OuterSagaId });
                MarkAsComplete();
            }
        }

        public void Handle(RequestUpdatedMessage message)
        {
            SendLocal(new RequestDescriptionUpdatedMessageInternal { RequestDto = message.Dto, SagaId = Data.OuterSagaId });
            MarkAsComplete();
        }

        public void Handle(TargetProcessExceptionThrownMessage message)
        {
            SendLocal(ExceptionThrownLocalMessage.Create(message, Data.OuterSagaId));
            MarkAsComplete();
        }
    }

    public class UpdateRequestDescriptionSagaData : ISagaEntity
    {
        public Guid Id { get; set; }
        public string Originator { get; set; }
        public string OriginalMessageId { get; set; }
        public Guid OuterSagaId { get; set; }
    }

    [Serializable]
    public class UpdateRequestDescriptionCommandInternal : IPluginLocalMessage
    {
        public Guid OuterSagaId { get; set; }
        public AttachmentDTO[] MessageAttachmentDtos { get; set; }
        public AttachmentDTO[] RequestAttachmentDtos { get; set; }
        public RequestDTO RequestDto { get; set; }
    }

    [Serializable]
    public class RequestDescriptionUpdatedMessageInternal : SagaMessage, IPluginLocalMessage
    {
        public RequestDTO RequestDto { get; set; }
    }
}
