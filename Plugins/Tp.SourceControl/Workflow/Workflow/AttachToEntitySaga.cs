// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using NServiceBus;
using NServiceBus.Saga;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Messages.TargetProcessLifecycle;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.SourceControl.Comments;
using Tp.SourceControl.Messages;

namespace Tp.SourceControl.Workflow.Workflow
{
	public class AttachToEntitySaga : TpSaga<AttachToEntitySagaData>,
	                                  IAmStartedByMessages<AssignRevisionToEntityAction>,
	                                  IHandleMessages<RevisionAssignableCreatedMessage>,
	                                  IHandleMessages<TargetProcessExceptionThrownMessage>
	{
		private readonly IActivityLogger _logger;

		public AttachToEntitySaga()
		{
		}

		public AttachToEntitySaga(IActivityLogger logger)
		{
			_logger = logger;
		}

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

			_logger.InfoFormat("Assigning revision to entity. Revision ID: {0}; Assignable ID: {1}", message.Dto.SourceControlID, message.EntityId);

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

			_logger.InfoFormat("Processing comment. Revision ID: {0}", Data.RevisionDto.SourceControlID);
			
			foreach (var action in actionsToExecute)
			{
				action.Execute(actionParamFiller, command => Send(command), _logger);
			}
		}

		public void Handle(TargetProcessExceptionThrownMessage message)
		{
			_logger.Error(string.Format("Failed to attach revision to entity. Revision ID: {0}; Entity ID: {1}", Data.RevisionDto.SourceControlID, Data.EntityId), message.GetException());

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