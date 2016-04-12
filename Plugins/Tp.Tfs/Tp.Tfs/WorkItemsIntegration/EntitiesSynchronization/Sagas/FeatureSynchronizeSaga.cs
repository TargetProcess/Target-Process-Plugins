// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Commands;
using NServiceBus;
using NServiceBus.Saga;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Messages.TargetProcessLifecycle;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Tfs.WorkItemsIntegration.EntitiesSynchronization.Entities;
using Tp.Tfs.WorkItemsIntegration.EntitiesSynchronization.Messages;
using Tp.Tfs.WorkItemsIntegration.EntitiesSynchronization.Sagas;

namespace Tp.Tfs.WorkItemsIntegration.EntitiesSynchronization
{
	public class FeatureSynchronizeSaga :
			EntitySynchronizeSaga<FeatureEntity, FeatureDTO>,
			IAmStartedByMessages<FeatureSynchronizationMessage>,
			IHandleMessages<FeatureCreatedMessage>,
			IHandleMessages<FeatureUpdatedMessage>,
			IHandleMessages<TargetProcessExceptionThrownMessage>
	{
		public FeatureSynchronizeSaga()
		{
		}

		public FeatureSynchronizeSaga(IActivityLogger logger)
		{
			Logger = logger;
		}

		public override void ConfigureHowToFindSaga()
		{
			ConfigureHowToFindSagaInternal<FeatureDTO, FeatureField, FeatureCreatedMessage, FeatureUpdatedMessage>();
		}

		public void Handle(FeatureSynchronizationMessage message)
		{
			HandleInternal(message.WorkItem);
		}

		public void Handle(FeatureCreatedMessage message)
		{
			HandleCreatedInternal(message);
		}

		public void Handle(FeatureUpdatedMessage message)
		{
			HandleUpdatedInternal<FeatureUpdatedMessage, FeatureField>(message);
		}

		public void Handle(TargetProcessExceptionThrownMessage message)
		{
			HandleErrorInternal(message);
		}

		protected override void SendUpdateCommand(FeatureDTO featureDTO, WorkItemInfo itemInfo)
		{
			Send(new UpdateFeatureCommand(featureDTO) { ChangedFields = itemInfo.ChangedFields });
		}

		protected override void SendCreateCommand(FeatureDTO featureDTO)
		{
			Send(new CreateFeatureCommand(featureDTO));
			Data.CreatingEntity = true;
		}

		protected override FeatureDTO CreateEntityDTO(WorkItemInfo workItem)
		{
			FeatureDTO featureDto = base.CreateEntityDTO(workItem);

			featureDto.ProjectName = workItem.TpProjectName;
			featureDto.ProjectID = workItem.TpProjectId;

			return featureDto;
		}
	}
}
