// 
// Copyright (c) 2005-2013 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using NServiceBus;
using NServiceBus.Saga;
using StructureMap;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Queries;
using Tp.Integration.Messages.TargetProcessLifecycle;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
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
			                                     IHandleMessages<CommentQueryResult>
	{
		private static readonly string GeneralsHql =
			string.Format(
				@"select g from General g left join g.ParentProject p where g.EntityType IN ({0}) and (p is null or p.DeleteDate is null) order by g skip {1} take {2}",
				ObjectFactory.GetInstance<IEntityTypeProvider>().QueryableEntityTypesIdSqlString, "{0}", "{1}");
		private const string AssignablesHql = "select a from Assignable a where a in ({0})";
		private const string TestCaseHql = "select t from TestCase t where t in ({0})";
		private static readonly string CommentsHql =
			string.Format(
				"select c from Comment c join c.General g left join g.ParentProject p where g.EntityType IN ({0}) and (p is null or p.DeleteDate is null) order by c skip {1} take {2}",
				ObjectFactory.GetInstance<IEntityTypeProvider>().QueryableEntityTypesIdSqlString, "{0}", "{1}");
		private readonly IEntityIndexer _entityIndexer;
		private readonly ISagaPersister _sagaPersister;
		private readonly IEntityTypeProvider _entityTypesProvider;
		private readonly IDocumentIndexProvider _documentIndexProvider;
		private readonly IPluginContext _pluginContext;
		private readonly IActivityLogger _logger;
		private readonly object[] _param = new object[] { };
		private const int PageSize = 10;

		public IndexExistingEntitiesSaga()
		{
		}

		public IndexExistingEntitiesSaga(IEntityIndexer entityIndexer, ISagaPersister sagaPersister, IEntityTypeProvider entityTypesProvider, IDocumentIndexProvider documentIndexProvider, IPluginContext pluginContext, IActivityLogger logger)
		{
			_entityIndexer = entityIndexer;
			_sagaPersister = sagaPersister;
			_entityTypesProvider = entityTypesProvider;
			_documentIndexProvider = documentIndexProvider;
			_pluginContext = pluginContext;
			_logger = logger;
		}

		public override void ConfigureHowToFindSaga()
		{
			ConfigureMapping<TargetProcessExceptionThrownMessage>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<GeneralQueryResult>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<AssignableQueryResult>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<TestCaseQueryResult>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<CommentQueryResult>(saga => saga.Id, message => message.SagaId);
		}

		public void Handle(IndexExistingEntitiesLocalMessage message)
		{
			try
			{
				foreach (var tpSagaEntity in ObjectFactory.GetInstance<IStorageRepository>().Get<ISagaEntity>())
				{
					if (!(tpSagaEntity is IndexExistingEntitiesSagaData)) continue;

					if (tpSagaEntity.Id != Data.Id)
						_sagaPersister.Complete(tpSagaEntity);
				}
			}
			catch (Exception e)
			{
				_logger.ErrorFormat("Failed to complete Running Saga When start to rebuild indexed, error: ", e.Message);
			}
			_documentIndexProvider.ShutdownDocumentIndexes(_pluginContext.AccountName,
			                                               new DocumentIndexShutdownSetup(forceShutdown: true, cleanStorage: true));

			Data.OuterSagaId = message.OuterSagaId;
			Data.GeneralsRetrievedCount = 0;
			Data.CommentsRetrievedCount = 0;

			Send(new GeneralQuery {Hql = string.Format(GeneralsHql, Data.SkipGenerals, PageSize), IgnoreMessageSizeOverrunFailure = true, Params = _param});
		}

		public void Handle(GeneralQueryResult message)
		{
			if (message.Dtos.Any())
			{
				if (Data.SkipGenerals % 1000 == 0)
				{
					_entityIndexer.OptimizeGeneralIndex();
					_entityIndexer.OptimizeAssignableIndex();
					_entityIndexer.OptimizeTestCaseIndex();
				}

				Data.GeneralsRetrievedCount += message.Dtos.Length;

				foreach (var general in message.Dtos)
				{
					_entityIndexer.AddGeneralIndex(general);
				}

				var assignables = message.Dtos.Where(g => _entityTypesProvider.IsAssignable(g.EntityTypeID)).Select(generalDto => generalDto.GeneralID.GetValueOrDefault()).ToArray();
				if (assignables.Any())
				{
					Send(new AssignableQuery { Hql = string.Format(AssignablesHql, string.Join(",", assignables)), IgnoreMessageSizeOverrunFailure = true, Params = _param });
				}

				var testCases = message.Dtos.Where(g => _entityTypesProvider.IsTestCase(g.EntityTypeID)).Select(generalDto => generalDto.GeneralID.GetValueOrDefault()).ToArray();
				if (testCases.Any())
				{
					Send(new TestCaseQuery { Hql = string.Format(TestCaseHql, string.Join(",", testCases)), IgnoreMessageSizeOverrunFailure = true, Params = _param });
				}

				if (Data.GeneralsRetrievedCount == message.QueryResultCount)
				{
					Data.GeneralsRetrievedCount = 0;
					Data.SkipGenerals += PageSize;

					Send(new GeneralQuery { Hql = string.Format(GeneralsHql, Data.SkipGenerals, PageSize), IgnoreMessageSizeOverrunFailure = true, Params = _param });
				}
			}
			else
			{
				_entityIndexer.OptimizeGeneralIndex();
				_entityIndexer.OptimizeAssignableIndex();
				_entityIndexer.OptimizeTestCaseIndex();
				Data.CommentsRetrievedCount = 0;
				Data.GeneralsRetrievedCount = 0;
				Data.SkipGenerals = 0;
				Send(new CommentQuery { Hql = string.Format(CommentsHql, Data.SkipComments, PageSize), IgnoreMessageSizeOverrunFailure = true, Params = _param });
			}
		}

		public void Handle(AssignableQueryResult message)
		{
			foreach (var assignable in message.Dtos)
			{
				_entityIndexer.UpdateAssignableIndex(assignable, new[] { AssignableField.EntityStateID, AssignableField.SquadID }, isIndexing: true);
			}
		}

		public void Handle(TestCaseQueryResult message)
		{
			foreach (var testCase in message.Dtos)
			{
				_entityIndexer.UpdateTestCaseIndex(testCase, new[] { TestCaseField.Steps, TestCaseField.Success }, isIndexing: true);
			}
		}

		public void Handle(CommentQueryResult message)
		{
			if (message.Dtos.Any())
			{
				if (Data.SkipComments %1000 == 0)
				{
					_entityIndexer.OptimizeCommentIndex();
				}

				Data.CommentsRetrievedCount += message.Dtos.Length;

				foreach (var comment in message.Dtos)
				{
					_entityIndexer.AddCommentIndex(comment);
				}

				if (Data.CommentsRetrievedCount == message.QueryResultCount)
				{
					Data.CommentsRetrievedCount = 0;
					Data.SkipComments += PageSize;

					Send(new CommentQuery { Hql = string.Format(CommentsHql, Data.SkipComments, PageSize), IgnoreMessageSizeOverrunFailure = true, Params = _param });
				}
			}
			else
			{
				_entityIndexer.OptimizeCommentIndex();
				SendLocal(new IndexExistingEntitiesDoneLocalMessage { SagaId = Data.OuterSagaId });
				MarkAsComplete();
			}
		}

		public void Handle(TargetProcessExceptionThrownMessage message)
		{
			_logger.Error("Build indexes failed", new Exception(message.ExceptionString));
			MarkAsComplete();
		}
	}

	[Serializable]
	public class IndexExistingEntitiesSagaData : ISagaEntity
	{
		public Guid Id { get; set; }
		public string Originator { get; set; }
		public string OriginalMessageId { get; set; }

		public Guid OuterSagaId { get; set; }
		public int GeneralsRetrievedCount { get; set; }
		public int CommentsRetrievedCount { get; set; }
		public int SkipGenerals { get; set; }
		public int SkipComments { get; set; }
	}
}