// 
// Copyright (c) 2005-2014 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using NServiceBus;
using NServiceBus.Saga;
using Tp.Integration.Messages.EntityLifecycle.Queries;
using Tp.Integration.Messages.TargetProcessLifecycle;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Search.Bus.Utils;
using Tp.Search.Messages;
using Tp.Search.Model.Document;
using Tp.Search.Model.Entity;

namespace Tp.Search.Bus.Workflow
{
	class IndexExistingEntitiesSaga : TpSaga<IndexExistingEntitiesSagaData>,
	                                  IAmStartedByMessages<IndexExistingEntitiesLocalMessage>,
			                          IHandleMessages<TargetProcessExceptionThrownMessage>,
			                          IHandleMessages<GeneralQueryResult>,
			                          IHandleMessages<AssignableQueryResult>,
			                          IHandleMessages<TestCaseQueryResult>,
			                          IHandleMessages<ImpedimentQueryResult>,
			                          IHandleMessages<CommentQueryResult>
	{
		private readonly IEntityIndexer _entityIndexer;
		private readonly IEntityTypeProvider _entityTypesProvider;
		private readonly IDocumentIndexProvider _documentIndexProvider;
		private readonly IPluginContext _pluginContext;
		private readonly IActivityLogger _logger;
		private readonly SagaServices _sagaServices;
		private readonly GeneralsIndexing _generalsIndexing;
		private readonly AssignablesIndexing _assignablesIndexing;
		private readonly TestCasesIndexing _testCasesIndexing;
		private readonly ImpedimentsIndexing _impedimentsIndexing;
		private readonly CommentsIndexing _commentsIndexing;

		public IndexExistingEntitiesSaga()
		{
		}

		public IndexExistingEntitiesSaga(IEntityIndexer entityIndexer, IEntityTypeProvider entityTypesProvider, IDocumentIndexProvider documentIndexProvider, IPluginContext pluginContext, IActivityLogger logger, SagaServices sagaServices)
		{
			_entityIndexer = entityIndexer;
			_entityTypesProvider = entityTypesProvider;
			_documentIndexProvider = documentIndexProvider;
			_pluginContext = pluginContext;
			_logger = logger;
			_sagaServices = sagaServices;
			_generalsIndexing = new GeneralsIndexing(_entityIndexer, () => Data, _entityTypesProvider, d => _assignablesIndexing.Start(), q => Send(q), _logger);
			_assignablesIndexing = new AssignablesIndexing(_entityIndexer, () => Data, _entityTypesProvider, d => _testCasesIndexing.Start(), q => Send(q), _logger);
			_testCasesIndexing = new TestCasesIndexing(_entityIndexer, () => Data, _entityTypesProvider, d => _impedimentsIndexing.Start(), q => Send(q), _logger);
			_impedimentsIndexing = new ImpedimentsIndexing(_entityIndexer, () => Data, _entityTypesProvider, d => _commentsIndexing.Start(), q => Send(q), _logger);
			_commentsIndexing = new CommentsIndexing(_entityIndexer, () => Data, _entityTypesProvider, d =>
				{
					SendLocal(new IndexExistingEntitiesDoneLocalMessage { SagaId = Data.OuterSagaId });
					MarkAsComplete();
				}, q => Send(q), _logger, (dto, indexer) => indexer.AddCommentIndex(dto, DocumentIndexOptimizeSetup.NoOptimize));
		}

		public override void ConfigureHowToFindSaga()
		{
			ConfigureMapping<TargetProcessExceptionThrownMessage>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<GeneralQueryResult>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<AssignableQueryResult>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<TestCaseQueryResult>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<ImpedimentQueryResult>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<CommentQueryResult>(saga => saga.Id, message => message.SagaId);
		}

		public void Handle(IndexExistingEntitiesLocalMessage message)
		{
			_sagaServices.TryCompleteInprogressSaga<IndexExistingEntitiesSagaData>(Data.Id);
			_documentIndexProvider.ShutdownDocumentIndexes(_pluginContext, new DocumentIndexShutdownSetup(forceShutdown: true, cleanStorage: true), _logger);
			Data.OuterSagaId = message.OuterSagaId;
			_generalsIndexing.Start();
		}

		public void Handle(GeneralQueryResult message)
		{
			_generalsIndexing.Handle(message);
		}

		public void Handle(AssignableQueryResult message)
		{
			_assignablesIndexing.Handle(message);
		}

		public void Handle(TestCaseQueryResult message)
		{
			_testCasesIndexing.Handle(message);
		}

		public void Handle(ImpedimentQueryResult message)
		{
			_impedimentsIndexing.Handle(message);
		}

		public void Handle(CommentQueryResult message)
		{
			_commentsIndexing.Handle(message);
		}

		public void Handle(TargetProcessExceptionThrownMessage message)
		{
			_logger.Error("Build indexes failed", new Exception(message.ExceptionString));
			SendLocal(new IndexExistingEntitiesDoneLocalMessage { SagaId = Data.OuterSagaId });
			MarkAsComplete();
		}
	}

	[Serializable]
	public class IndexExistingEntitiesSagaData : ISagaEntity, IAssignableIndexingSagaData, ICommentIndexingSagaData
	{
		public Guid Id { get; set; }
		public string Originator { get; set; }
		public string OriginalMessageId { get; set; }

		public Guid OuterSagaId { get; set; }

		public int GeneralsRetrievedCount { get; set; }
		public int AssignablesRetrievedCount { get; set; }
		public int TestCasesRetrievedCount { get; set; }
		public int ImpedimentsRetrievedCount { get; set; }
		public int CommentsRetrievedCount { get; set; }

		public int GeneralsCurrentDataWindowSize { get; set; }
		public int AssignablesCurrentDataWindowSize { get; set; }
		public int TestCasesCurrentDataWindowSize { get; set; }
		public int ImpedimentsCurrentDataWindowSize { get; set; }
		public int CommentsCurrentDataWindowSize { get; set; }
	}
}