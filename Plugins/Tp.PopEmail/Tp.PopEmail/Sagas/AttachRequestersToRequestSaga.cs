// 
// Copyright (c) 2005-2014 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using NServiceBus;
using NServiceBus.Saga;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Messages.EntityLifecycle.Commands;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Messages.TargetProcessLifecycle;
using Tp.Integration.Plugin.Common;

namespace Tp.PopEmailIntegration.Sagas
{
    public class AttachRequestersToRequestSaga : TpSaga<AttachRequestersToRequestSagaData>,
                                              IAmStartedByMessages<AttachRequestersToRequestCommandInternal>,
                                              IHandleMessages<GeneralUserAttachedToRequestMessage>,
                                              IHandleMessages<TargetProcessExceptionThrownMessage>
    {
        public override void ConfigureHowToFindSaga()
        {
            ConfigureMapping<GeneralUserAttachedToRequestMessage>(
                saga => saga.Id,
                message => message.SagaId
                );
            ConfigureMapping<TargetProcessExceptionThrownMessage>(
                saga => saga.Id,
                message => message.SagaId
                );
        }

        public void Handle(AttachRequestersToRequestCommandInternal message)
        {
            Data.OuterSagaId = message.OuterSagaId;
            Data.Requesters = message.Requesters;
            foreach (var requesterId in message.Requesters)
            {
                Log().Info(string.Format("Attaching requester with Id {0} to request with id {1}", requesterId, message.RequestId));
                var attachGeneralUserToRequestCommand = new AttachGeneralUserToRequestCommand
                                                        {
                                                            RequestId = message.RequestId,
                                                            RequesterId = requesterId
                                                        };
                Send(attachGeneralUserToRequestCommand);
            }

        }

        public void Handle(GeneralUserAttachedToRequestMessage message)
        {
            Data.ProcessedRequestersCount++;
            if (Data.ProcessedRequestersCount == Data.Requesters.Count())
            {
                SendLocal(new RequestersAttachedToRequestMessageInternal { SagaId = Data.OuterSagaId });
                MarkAsComplete();
            }
        }

        public void Handle(TargetProcessExceptionThrownMessage message)
        {
            Log().Error("Failed to attach requesters to project", message.GetException());
            MarkAsComplete();
        }
    }

    [Serializable]
    public class AttachRequestersToRequestCommandInternal : IPluginLocalMessage
    {
        public Guid OuterSagaId { get; set; }
        public int[] Requesters { get; set; }
        public int? RequestId { get; set; }
    }

    [Serializable]
    public class RequestersAttachedToRequestMessageInternal : SagaMessage, IPluginLocalMessage
    {
    }

    public class AttachRequestersToRequestSagaData : ISagaEntity
    {
        public Guid Id { get; set; }
        public string Originator { get; set; }
        public string OriginalMessageId { get; set; }
        public Guid OuterSagaId { get; set; }
        public int ProcessedRequestersCount { get; set; }
        public int[] Requesters { get; set; }
    }
}