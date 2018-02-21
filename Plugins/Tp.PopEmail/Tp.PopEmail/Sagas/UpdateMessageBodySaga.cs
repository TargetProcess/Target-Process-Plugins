// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Text.RegularExpressions;
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
    public class UpdateMessageBodySaga
        : TpSaga<UpdateMessageBodySagaData>,
          IAmStartedByMessages<UpdateMessageBodyCommandInternal>,
          IHandleMessages<MessageUpdatedMessage>,
          IHandleMessages<TargetProcessExceptionThrownMessage>
    {
        public override void ConfigureHowToFindSaga()
        {
            ConfigureMapping<MessageUpdatedMessage>(
                saga => saga.Id,
                message => message.SagaId
            );
            ConfigureMapping<TargetProcessExceptionThrownMessage>(
                saga => saga.Id,
                message => message.SagaId
            );
        }

        public void Handle(UpdateMessageBodyCommandInternal message)
        {
            Data.OuterSagaId = message.OuterSagaId;
            var messageBodyUpdated = message.MessageDto.Body;
            if (!string.IsNullOrEmpty(messageBodyUpdated))
            {
                foreach (var attachment in message.AttachmentDtos)
                {
                    var url = string.Format("~/Attachment.aspx?AttachmentID={0}", attachment.AttachmentID);
                    messageBodyUpdated = Regex.Replace(messageBodyUpdated,
                        @"(<img[^>]*src=['""])(cid:" + Regex.Escape(attachment.OriginalFileName) +
                        @")(['""][^>]*?>)", "$1" + url + "$3",
                        RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline);
                }
            }

            if (messageBodyUpdated != message.MessageDto.Body)
            {
                message.MessageDto.Body = messageBodyUpdated;
                Send(new UpdateMessageCommand(message.MessageDto, new Enum[] { MessageField.Body }));
            }
            else
            {
                SendLocal(new MessageBodyUpdatedMessageInternal { MessageDto = message.MessageDto, SagaId = Data.OuterSagaId });
                MarkAsComplete();
            }
        }

        public void Handle(MessageUpdatedMessage message)
        {
            SendLocal(new MessageBodyUpdatedMessageInternal { MessageDto = message.Dto, SagaId = Data.OuterSagaId });
            MarkAsComplete();
        }

        public void Handle(TargetProcessExceptionThrownMessage message)
        {
            SendLocal(ExceptionThrownLocalMessage.Create(message, Data.OuterSagaId));
            MarkAsComplete();
        }
    }

    public class UpdateMessageBodySagaData : ISagaEntity
    {
        public Guid Id { get; set; }
        public string Originator { get; set; }
        public string OriginalMessageId { get; set; }
        public Guid OuterSagaId { get; set; }
    }

    [Serializable]
    public class UpdateMessageBodyCommandInternal : IPluginLocalMessage
    {
        public Guid OuterSagaId { get; set; }
        public AttachmentDTO[] AttachmentDtos { get; set; }
        public MessageDTO MessageDto { get; set; }
    }

    [Serializable]
    public class MessageBodyUpdatedMessageInternal : SagaMessage, IPluginLocalMessage
    {
        public MessageDTO MessageDto { get; set; }
    }
}
