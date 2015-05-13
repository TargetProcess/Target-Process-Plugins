// 
// Copyright (c) 2005-2015 TargetProcess. All rights reserved.
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
									  IHandleMessages<TestStepQueryResult>,
									  IHandleMessages<ImpedimentQueryResult>,
									  IHandleMessages<CommentQueryResult>,
									  IHandleMessages<ReleaseProjectQueryResult>
	{
		private readonly IDocumentIndexProvider _documentIndexProvider;
		private readonly IPluginContext _pluginContext;
		private readonly IActivityLogger _logger;
		private readonly SagaServices _sagaServices;
		private readonly GeneralsIndexing _generalsIndexing;
		private readonly AssignablesIndexing _assignablesIndexing;
		private readonly TestStepsIndexing _testStepsIndexing;
		private readonly ImpedimentsIndexing _impedimentsIndexing;
		private readonly CommentsIndexing _commentsIndexing;
		private readonly ReleaseProjectIndexing _releaseProjectIndexing;

		public IndexExistingEntitiesSaga()
		{
		}

		public IndexExistingEntitiesSaga(IEntityIndexer entityIndexer, IEntityTypeProvider entityTypesProvider, IDocumentIndexProvider documentIndexProvider, IPluginContext pluginContext, IActivityLogger logger, SagaServices sagaServices)
		{
			_documentIndexProvider = documentIndexProvider;
			_pluginContext = pluginContext;
			_logger = logger;
			_sagaServices = sagaServices;
			_generalsIndexing = new GeneralsIndexing(entityIndexer, () => Data, entityTypesProvider, d => _assignablesIndexing.Start(), q => Send(q), _logger);
			_assignablesIndexing = new AssignablesIndexing(entityIndexer, () => Data, entityTypesProvider, d => _testStepsIndexing.Start(), q => Send(q), _logger);
			_testStepsIndexing = new TestStepsIndexing(entityIndexer, () => Data, entityTypesProvider, d => _impedimentsIndexing.Start(), q => Send(q), _logger, (dto, indexer) => indexer.AddTestStepIndex(dto, DocumentIndexOptimizeSetup.NoOptimize));
			_impedimentsIndexing = new ImpedimentsIndexing(entityIndexer, () => Data, entityTypesProvider, d => _releaseProjectIndexing.Start(), q => Send(q), _logger);
			_releaseProjectIndexing = new ReleaseProjectIndexing(entityIndexer, () => Data, entityTypesProvider, d => _commentsIndexing.Start(), q => Send(q), _logger);
			_commentsIndexing = new CommentsIndexing(entityIndexer, () => Data, entityTypesProvider, d =>
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
			ConfigureMapping<TestStepQueryResult>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<ImpedimentQueryResult>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<ReleaseProjectQueryResult>(saga => saga.Id, message => message.SagaId);
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
		
		public void Handle(TestStepQueryResult message)
		{
			_testStepsIndexing.Handle(message);
		}

		public void Handle(ImpedimentQueryResult message)
		{
			_impedimentsIndexing.Handle(message);
		}

		public void Handle(CommentQueryResult message)
		{
			_commentsIndexing.Handle(message);
		}

		public void Handle(ReleaseProjectQueryResult message)
		{
			_releaseProjectIndexing.Handle(message);
		}

		public void Handle(TargetProcessExceptionThrownMessage message)
		{
			_logger.Error("Build indexes failed", new Exception(message.ExceptionString));
			SendLocal(new IndexExistingEntitiesDoneLocalMessage { SagaId = Data.OuterSagaId });
			MarkAsComplete();
		}
	}

	[Serializable]
	public class IndexExistingEntitiesSagaData : ISagaEntity, IAssignableIndexingSagaData, ICommentIndexingSagaData, ITestStepIndexingSagaData
	{
		public Guid Id { get; set; }
		public string Originator { get; set; }
		public string OriginalMessageId { get; set; }

		public Guid OuterSagaId { get; set; }

		public int GeneralsRetrievedCount { get; set; }
		public int AssignablesRetrievedCount { get; set; }
		public int TestStepsRetrievedCount { get; set; }
		public int ReleaseProjectsRetrievedCount { get; set; }
		public int ImpedimentsRetrievedCount { get; set; }
		public int CommentsRetrievedCount { get; set; }

		public int GeneralsCurrentDataWindowSize { get; set; }
		public int AssignablesCurrentDataWindowSize { get; set; }
		public int TestStepsCurrentDataWindowSize { get; set; }
		public int ReleaseProjectsCurrentDataWindowSize { get; set; }
		public int ImpedimentsCurrentDataWindowSize { get; set; }
		public int CommentsCurrentDataWindowSize { get; set; }
	}
}