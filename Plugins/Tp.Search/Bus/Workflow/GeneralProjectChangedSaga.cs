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
using Tp.Integration.Plugin.Common.Activity;
using Tp.Search.Messages;
using Tp.Search.Model.Entity;

namespace Tp.Search.Bus.Workflow
{
	public class GeneralProjectChangedSaga : TpSaga<GeneralProjectChangeSagaData>,
	                                         IAmStartedByMessages<GeneralProjectChangedLocalMessage>,
	                                         IHandleMessages<CommentQueryResult>,
	                                         IHandleMessages<TargetProcessExceptionThrownMessage>
	{
		private const string CommentsForGeneralHql = "from Comment c where c.General = ?";

		private readonly IEntityIndexer _entityIndexer;
		private readonly IActivityLogger _logger;

		public GeneralProjectChangedSaga()
		{
		}

		public GeneralProjectChangedSaga(IEntityIndexer entityIndexer, IActivityLogger logger)
		{
			_entityIndexer = entityIndexer;
			_logger = logger;
		}

		public override void ConfigureHowToFindSaga()
		{
			ConfigureMapping<GeneralProjectChangedLocalMessage>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<CommentQueryResult>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<TargetProcessExceptionThrownMessage>(saga => saga.Id, message => message.SagaId);
		}

		public void Handle(GeneralProjectChangedLocalMessage message)
		{
			Data.ProjectId = message.ProjectId;
			if (message.GeneralId > 0)
			{
				Send(new CommentQuery { Hql = CommentsForGeneralHql,  IgnoreMessageSizeOverrunFailure = true, Params = new object[] { message.GeneralId } });
			}
			else
			{
				MarkAsComplete();
			}
		}

		public void Handle(CommentQueryResult message)
		{
			foreach (var comment in message.Dtos)
			{
				_entityIndexer.UpdateCommentIndex(comment, new List<CommentField>(), Maybe.Return(Data.ProjectId), Maybe.Nothing);
			}
			_entityIndexer.OptimizeCommentIndex();
			MarkAsComplete();
		}

		public void Handle(TargetProcessExceptionThrownMessage message)
		{
			_logger.Error("Rebuild indexes for comment failed", new Exception(message.ExceptionString));
			MarkAsComplete();
		}
	}

	[Serializable]
	public class GeneralProjectChangeSagaData : ISagaEntity
	{
		public Guid Id { get; set; }
		public string Originator { get; set; }
		public string OriginalMessageId { get; set; }
		public int? ProjectId { get; set; }
	}
}