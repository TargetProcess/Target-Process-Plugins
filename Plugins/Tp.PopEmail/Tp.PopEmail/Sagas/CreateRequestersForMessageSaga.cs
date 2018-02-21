// 
// Copyright (c) 2005-2016 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using NServiceBus;
using NServiceBus.Saga;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Messages.EntityLifecycle.Commands;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Messages.TargetProcessLifecycle;
using Tp.Integration.Plugin.Common;

namespace Tp.PopEmailIntegration.Sagas
{
    public class CreateRequestersForMessageSaga
        : TpSaga<CreateRequestersSagaData>,
          IAmStartedByMessages<CreateRequestersForMessageCommandInternal>,
          IHandleMessages<RequesterCreatedMessage>,
          IHandleMessages<TargetProcessExceptionThrownMessage>
    {
        public override void ConfigureHowToFindSaga()
        {
            ConfigureMapping<RequesterCreatedMessage>(
                saga => saga.Id,
                message => message.SagaId
            );
            ConfigureMapping<TargetProcessExceptionThrownMessage>(
                saga => saga.Id,
                message => message.SagaId
            );
        }

        public void Handle(CreateRequestersForMessageCommandInternal message)
        {
            Data.OuterSagaId = message.OuterSagaId;
            Data.RequestersDto = message.RequestersDto;
            Log().Info("Create requesters for message");
            foreach (var requesterDto in message.RequestersDto)
            {
                Log().Info($"Create requester with email '{requesterDto.Email}'");
                Send(new CreateRequesterCommand(requesterDto));
            }
        }

        public void Handle(RequesterCreatedMessage message)
        {
            Log().Info($"Requester with email '{message.Dto.Email}' created, id is {message.Dto.ID}");
            ProcessRequesterCreated();
        }

        private void ProcessRequesterCreated()
        {
            Data.ProcessedRequestersCount++;
            if (Data.ProcessedRequestersCount == Data.RequestersDto.Length)
            {
                SendLocal(new RequestersCreatedMessageInternal { SagaId = Data.OuterSagaId });
                MarkAsComplete();
            }
        }

        public void Handle(TargetProcessExceptionThrownMessage message)
        {
            if (!message.ExceptionString.Contains("The requester with the same email already exists"))
            {
                Log().Error("Failed to create requesters.", message.GetException());
                SendLocal(new RequestersCreationFailedMessageInternal { SagaId = Data.OuterSagaId });
            }
            else
            {
                var count = Data.ProcessedRequestersCount;
                var requesters = Data.RequestersDto;
                if (requesters.Length > count)
                {
                    Log().Error($"Requester with email {requesters[count].Email} already exists in targetprocess");
                }
                ProcessRequesterCreated();
            }
        }
    }

    [Serializable]
    public class CreateRequestersForMessageCommandInternal : IPluginLocalMessage
    {
        public RequesterDTO[] RequestersDto { get; set; }
        public Guid OuterSagaId { get; set; }
    }

    [Serializable]
    public class RequestersCreatedMessageInternal : SagaMessage, IPluginLocalMessage
    {
    }

    [Serializable]
    public class RequestersCreationFailedMessageInternal : SagaMessage, IPluginLocalMessage
    {
    }

    public class CreateRequestersSagaData : ISagaEntity
    {
        public Guid Id { get; set; }
        public string Originator { get; set; }
        public string OriginalMessageId { get; set; }
        public Guid OuterSagaId { get; set; }
        public RequesterDTO[] RequestersDto { get; set; }
        public int ProcessedRequestersCount { get; set; }
    }
}
