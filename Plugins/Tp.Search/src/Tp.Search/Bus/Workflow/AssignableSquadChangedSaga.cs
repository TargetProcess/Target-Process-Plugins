// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using NServiceBus;
using NServiceBus.Saga;
using Tp.Core;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Queries;
using Tp.Integration.Messages.TargetProcessLifecycle;
using Tp.Integration.Plugin.Common;
using Tp.Search.Messages;
using Tp.Search.Model.Document;
using Tp.Search.Model.Entity;

namespace Tp.Search.Bus.Workflow
{
	public class AssignableSquadChangedSaga : TpSaga<AssignableSquadChangeSagaData>,
	                                          IAmStartedByMessages<AssignableSquadChangedLocalMessage>,
			                                      IHandleMessages<CommentQueryResult>
	{
		private readonly IEntityIndexer _entityIndexer;

		public AssignableSquadChangedSaga()
		{
		}

		public AssignableSquadChangedSaga(IEntityIndexer entityIndexer)
		{
			_entityIndexer = entityIndexer;
		}

		public void Handle(AssignableSquadChangedLocalMessage message)
		{
			Data.SquadId = message.SquadId;
			if (message.AssignableId > 0)
			{
				Send(new CommentQuery { GeneralId = message.AssignableId, IgnoreMessageSizeOverrunFailure = true });
			}
			else
			{
				MarkAsComplete();
			}
		}

		public override void ConfigureHowToFindSaga()
		{
			ConfigureMapping<AssignableSquadChangedLocalMessage>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<CommentQueryResult>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<TargetProcessExceptionThrownMessage>(saga => saga.Id, message => message.SagaId);
		}

		public void Handle(CommentQueryResult message)
		{
			foreach (var comment in message.Dtos)
			{
				_entityIndexer.UpdateCommentIndex(comment, new List<CommentField>(), Maybe.Nothing, Maybe.Return(Data.SquadId), DocumentIndexOptimizeSetup.NoOptimize);
			}
			_entityIndexer.OptimizeCommentIndex(DocumentIndexOptimizeSetup.ImmediateOptimize);
			MarkAsComplete();
		}
	}

	public class AssignableSquadChangeSagaData : ISagaEntity
	{
		public Guid Id { get; set; }
		public string Originator { get; set; }
		public string OriginalMessageId { get; set; }
		public int? SquadId { get; set; }
	}
}