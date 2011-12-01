// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
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
	public class CreateRequesterForMessageSaga : TpSaga<CreateRequesterSagaData>,
	                                   IAmStartedByMessages<CreateRequesterForMessageCommandInternal>,
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

		public void Handle(CreateRequesterForMessageCommandInternal message)
		{
			Data.OuterSagaId = message.OuterSagaId;
			Send(new CreateRequesterCommand(message.RequesterDto));
		}

		public void Handle(RequesterCreatedMessage message)
		{
			SendLocal(new RequesterCreatedMessageInternal {SagaId = Data.OuterSagaId});
			MarkAsComplete();
		}

		public void Handle(TargetProcessExceptionThrownMessage message)
		{
			if (!message.ExceptionString.Contains("The requester with the same email already exists"))
			{
				Log().Error("Failed to create requester.", message.GetException());
				SendLocal(new RequesterCreationFailedMessageInternal {SagaId = Data.OuterSagaId});
			}
			else
			{
				SendLocal(new RequesterCreatedMessageInternal { SagaId = Data.OuterSagaId });
			}

			MarkAsComplete();
		}
	}

	[Serializable]
	public class CreateRequesterForMessageCommandInternal : IPluginLocalMessage
	{
		public RequesterDTO RequesterDto { get; set; }
		public Guid OuterSagaId { get; set; }
	}

	[Serializable]
	public class RequesterCreatedMessageInternal : SagaMessage, IPluginLocalMessage
	{
	}

	[Serializable]
	public class RequesterCreationFailedMessageInternal : SagaMessage, IPluginLocalMessage
	{
	}

	public class CreateRequesterSagaData : ISagaEntity
	{
		public Guid Id { get; set; }
		public string Originator { get; set; }
		public string OriginalMessageId { get; set; }
		public Guid OuterSagaId { get; set; }
	}
}
