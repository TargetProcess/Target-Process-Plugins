// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NServiceBus;
using NServiceBus.Saga;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Commands;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Messages.TargetProcessLifecycle;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Tfs.WorkItemsIntegration.EntitiesSynchronization.Entities;
using Tp.Tfs.WorkItemsIntegration.EntitiesSynchronization.Messages;
using Tp.Tfs.WorkItemsIntegration.EntitiesSynchronization.Sagas;

namespace Tp.Tfs.WorkItemsIntegration.EntitiesSynchronization
{
    public class RequestSynchronizeSaga
        :
            EntitySynchronizeSaga<RequestEntity, RequestDTO>,
            IAmStartedByMessages<RequestSynchronizationMessage>,
            IHandleMessages<RequestCreatedMessage>,
            IHandleMessages<RequestUpdatedMessage>,
            IHandleMessages<TargetProcessExceptionThrownMessage>
    {
        public RequestSynchronizeSaga()
        {
        }

        public RequestSynchronizeSaga(IActivityLogger logger)
        {
            Logger = logger;
        }

        public override void ConfigureHowToFindSaga()
        {
            ConfigureHowToFindSagaInternal<RequestDTO, RequestField, RequestCreatedMessage, RequestUpdatedMessage>();
        }

        public void Handle(RequestSynchronizationMessage message)
        {
            HandleInternal(message.WorkItem);
        }

        public void Handle(RequestCreatedMessage message)
        {
            HandleCreatedInternal(message);
        }

        public void Handle(RequestUpdatedMessage message)
        {
            HandleUpdatedInternal<RequestUpdatedMessage, RequestField>(message);
        }

        public void Handle(TargetProcessExceptionThrownMessage message)
        {
            HandleErrorInternal(message);
        }

        protected override void SendUpdateCommand(RequestDTO requestDTO, WorkItemInfo itemInfo)
        {
            Send(new UpdateRequestCommand(requestDTO) { ChangedFields = itemInfo.ChangedFields });
        }

        protected override void SendCreateCommand(RequestDTO requestDTO)
        {
            Send(new CreateRequestCommand(requestDTO));
            Data.CreatingEntity = true;
        }

        protected override RequestDTO CreateEntityDTO(WorkItemInfo workItem)
        {
            RequestDTO requestDto = base.CreateEntityDTO(workItem);

            requestDto.ProjectName = workItem.TpProjectName;
            requestDto.ProjectID = workItem.TpProjectId;

            return requestDto;
        }
    }
}
