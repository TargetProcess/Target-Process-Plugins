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
using Tp.Search.Model.Document;
using Tp.Search.Model.Entity;

namespace Tp.Search.Bus.Workflow
{
	class GeneralProjectChangedSaga : TpSaga<GeneralProjectChangeSagaData>,
	                                         IAmStartedByMessages<GeneralProjectChangedLocalMessage>,
	                                         IHandleMessages<CommentQueryResult>,
	                                         IHandleMessages<TargetProcessExceptionThrownMessage>
	{
		private readonly IEntityIndexer _entityIndexer;
		private readonly IEntityTypeProvider _entityTypesProvider;
		private readonly IActivityLogger _logger;
		private readonly CommentsIndexing _commentsIndexing;

		public GeneralProjectChangedSaga()
		{
		}

		public GeneralProjectChangedSaga(IEntityIndexer entityIndexer, IEntityTypeProvider entityTypesProvider, IActivityLogger logger)
		{
			_entityIndexer = entityIndexer;
			_entityTypesProvider = entityTypesProvider;
			_logger = logger;
			_commentsIndexing = new CommentsIndexing(_entityIndexer, () => Data, _entityTypesProvider, d => MarkAsComplete(),
										 q =>
										 {
											 q.GeneralId = Data.GeneralId;
											 Send(q);
										 }
										 , _logger, (dto, indexer) => indexer.UpdateCommentIndex(dto, new List<CommentField>(), Maybe.Return(Data.ProjectId), Maybe.Nothing, DocumentIndexOptimizeSetup.NoOptimize));
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
			Data.GeneralId = message.GeneralId;
			if (message.GeneralId > 0)
			{
				_commentsIndexing.Start();
			}
			else
			{
				MarkAsComplete();
			}
		}

		public void Handle(CommentQueryResult message)
		{
			_commentsIndexing.Handle(message);
		}

		public void Handle(TargetProcessExceptionThrownMessage message)
		{
			_logger.Error("Rebuild indexes for comment failed", new Exception(message.ExceptionString));
			MarkAsComplete();
		}
	}

	[Serializable]
	public class GeneralProjectChangeSagaData : ISagaEntity, ICommentIndexingSagaData
	{
		public Guid Id { get; set; }
		public string Originator { get; set; }
		public string OriginalMessageId { get; set; }
		public int? ProjectId { get; set; }
		public int CommentsRetrievedCount { get; set; }
		public int CommentsCurrentDataWindowSize { get; set; }

		public int? GeneralId { get; set; }
	}
}