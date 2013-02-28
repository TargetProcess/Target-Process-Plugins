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
	public class BugSynchronizeSaga :
			EntitySynchronizeSaga<BugEntity, BugDTO>,
			IAmStartedByMessages<BugSynchronizationMessage>,
			IHandleMessages<BugCreatedMessage>,
			IHandleMessages<BugUpdatedMessage>,
			IHandleMessages<TargetProcessExceptionThrownMessage>
	{
		public BugSynchronizeSaga()
		{
		}

		public BugSynchronizeSaga(IActivityLogger logger)
		{
			Logger = logger;
		}

		public override void ConfigureHowToFindSaga()
		{
			ConfigureHowToFindSagaInternal<BugDTO, BugField, BugCreatedMessage, BugUpdatedMessage>();
		}

		public void Handle(BugSynchronizationMessage message)
		{
			HandleInternal(message.WorkItem);
		}

		public void Handle(BugCreatedMessage message)
		{
			HandleCreatedInternal(message);
		}

		public void Handle(BugUpdatedMessage message)
		{
			HandleUpdatedInternal<BugUpdatedMessage, BugField>(message);
		}

		public void Handle(TargetProcessExceptionThrownMessage message)
		{
			HandleErrorInternal(message);
		}

		protected override void SendUpdateCommand(BugDTO bugDTO, WorkItemInfo itemInfo)
		{
			Send(new UpdateBugCommand(bugDTO) { ChangedFields = itemInfo.ChangedFields });
		}

		protected override void SendCreateCommand(BugDTO bugDTO)
		{
			Send(new CreateBugCommand(bugDTO));
			Data.CreatingEntity = true;
		}

		protected override BugDTO CreateEntityDTO(WorkItemInfo workItem)
		{
			BugDTO bugDto = base.CreateEntityDTO(workItem);

			bugDto.ProjectName = workItem.TpProjectName;
			bugDto.ProjectID = workItem.TpProjectId;

			return bugDto;
		}
	}
}
