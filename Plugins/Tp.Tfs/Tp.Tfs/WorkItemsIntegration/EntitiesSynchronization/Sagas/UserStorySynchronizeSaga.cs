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
	public class UserStorySynchronizeSaga :
			EntitySynchronizeSaga<UserStoryEntity, UserStoryDTO>,
			IAmStartedByMessages<UserStorySynchronizationMessage>,
			IHandleMessages<UserStoryCreatedMessage>,
			IHandleMessages<UserStoryUpdatedMessage>,
			IHandleMessages<TargetProcessExceptionThrownMessage>
	{
		public UserStorySynchronizeSaga()
		{
		}

		public UserStorySynchronizeSaga(IActivityLogger logger)
		{
			Logger = logger;
		}

		public override void ConfigureHowToFindSaga()
		{
			ConfigureHowToFindSagaInternal<UserStoryDTO, UserStoryField, UserStoryCreatedMessage, UserStoryUpdatedMessage>();
		}

		public void Handle(UserStorySynchronizationMessage message)
		{
			HandleInternal(message.WorkItem);
		}

		public void Handle(UserStoryCreatedMessage message)
		{
			HandleCreatedInternal(message);
		}

		public void Handle(UserStoryUpdatedMessage message)
		{
			HandleUpdatedInternal<UserStoryUpdatedMessage, UserStoryField>(message);
		}

		public void Handle(TargetProcessExceptionThrownMessage message)
		{
			HandleErrorInternal(message);
		}

		protected override void SendUpdateCommand(UserStoryDTO userStoryDTO, WorkItemInfo itemInfo)
		{
			Send(new UpdateUserStoryCommand(userStoryDTO) { ChangedFields = itemInfo.ChangedFields });
		}

		protected override void SendCreateCommand(UserStoryDTO userStoryDTO)
		{
			Send(new CreateUserStoryCommand(userStoryDTO));
			Data.CreatingEntity = true;
		}

		protected override UserStoryDTO CreateEntityDTO(WorkItemInfo workItem)
		{
			UserStoryDTO userStoryDto = base.CreateEntityDTO(workItem);

			userStoryDto.ProjectName = workItem.TpProjectName;
			userStoryDto.ProjectID = workItem.TpProjectId;

			return userStoryDto;
		}
	}
}
