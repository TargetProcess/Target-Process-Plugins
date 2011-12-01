// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using NServiceBus;
using NServiceBus.Saga;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Messages.TargetProcessLifecycle;
using Tp.Integration.Plugin.Common;
using Tp.SourceControl.Comments;
using Tp.SourceControl.Messages;

namespace Tp.SourceControl.Workflow.Workflow
{
	public class AttachToEntitySaga : TpSaga<AttachToEntitySagaData>,
	                                  IAmStartedByMessages<AssignRevisionToEntityAction>,
	                                  IHandleMessages<RevisionAssignableCreatedMessage>,
	                                  IHandleMessages<TargetProcessExceptionThrownMessage>
	{
		public override void ConfigureHowToFindSaga()
		{
			ConfigureMapping<AssignRevisionToEntityAction>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<RevisionAssignableCreatedMessage>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<TargetProcessExceptionThrownMessage>(saga => saga.Id, message => message.SagaId);
		}

		public void Handle(AssignRevisionToEntityAction message)
		{
			Data.RevisionDto = message.Dto;

			var dto = new RevisionAssignableDTO
			{
				AssignableID = message.EntityId,
				RevisionID = message.Dto.ID
			};

			Data.EntityId = message.EntityId;


			Send(new CreateEntityCommand<DataTransferObject>(dto));
		}

		public void Handle(RevisionAssignableCreatedMessage message)
		{
			var commentParser = new CommentParser();

			var actions = commentParser.Parse(Data.RevisionDto, Data.EntityId);

			var actionParamFiller = new ActionParameterFillerVisitor(Data.RevisionDto, Data.EntityId);

			var actionsToExecute = actions.ToArray();
			if (!actionsToExecute.Any())
			{
				MarkAsComplete();
				return;
			}
			
			foreach (var action in actionsToExecute)
			{
				action.Execute(actionParamFiller, command => Send(command));
			}
		}

		public void Handle(TargetProcessExceptionThrownMessage message)
		{
			Log().Error(string.Format("Failed to attach revision {0} to entity", Data.RevisionDto.SourceControlID), message.GetException());
			MarkAsComplete();
		}
	}

	public class AttachToEntitySagaData : ISagaEntity
	{
		public Guid Id { get; set; }
		public string Originator { get; set; }
		public string OriginalMessageId { get; set; }

		public RevisionDTO RevisionDto { get; set; }

		public int EntityId { get; set; }
	}
}