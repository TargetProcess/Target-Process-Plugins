// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using NServiceBus;
using NServiceBus.Saga;
using Tp.Integration.Messages.EntityLifecycle.Commands;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Messages.TargetProcessLifecycle;
using Tp.Integration.Plugin.Common;
using Tp.PopEmailIntegration.Rules.ThenClauses;

namespace Tp.PopEmailIntegration.Sagas
{
	public class AttachMessageToGeneralSaga : TpSaga<AttachMessageToGeneralSagaData>,
	                                          IAmStartedByMessages<AttachMessageToProjectCommand>,
	                                          IHandleMessages<MessageAttachedToGeneralMessage>,
	                                          IHandleMessages<TargetProcessExceptionThrownMessage>
	{
		public override void ConfigureHowToFindSaga()
		{
			ConfigureMapping<MessageAttachedToGeneralMessage>(
				saga => saga.Id,
				message => message.SagaId
				);
			ConfigureMapping<TargetProcessExceptionThrownMessage>(
				saga => saga.Id,
				message => message.SagaId
				);
		}

		public void Handle(AttachMessageToProjectCommand message)
		{
			var attachMessageToGeneralCommand = new AttachMessageToGeneralCommand
			                                    	{
			                                    		GeneralId = message.ProjectId,
			                                    		MessageId = message.MessageDto.ID
			                                    	};
			Send(attachMessageToGeneralCommand);
		}

		public void Handle(MessageAttachedToGeneralMessage message)
		{
			MarkAsComplete();
		}

		public void Handle(TargetProcessExceptionThrownMessage message)
		{
			Log().Error("Failed to attach message to project", message.GetException());
			MarkAsComplete();
		}
	}

	public class AttachMessageToGeneralSagaData : ISagaEntity
	{
		public Guid Id { get; set; }

		public string Originator { get; set; }

		public string OriginalMessageId { get; set; }
	}
}