using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Tp.Core;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Search.Model.Document;
using Tp.Search.Model.Entity;
using Tp.Search.Model.Exceptions;

namespace Tp.Search.Model.Query
{
	class QueryEngine
	{
		private readonly IDocumentIndexProvider _documentIndexProvider;
		private readonly IPluginContext _pluginContext;
		private readonly IActivityLogger _logger;
		public QueryEngine(IDocumentIndexProvider documentIndexProvider, IPluginContext pluginContext, IActivityLogger logger)
		{
			_documentIndexProvider = documentIndexProvider;
			_pluginContext = pluginContext;
			_logger = logger;
		}

		public QueryRunResult Run(QueryPlanFull planFull, Page page)
		{
			const int maxTryCount = 5;
			int tryCounter = maxTryCount;
			for (;;)
			{
				try
				{
					var s = Stopwatch.StartNew();
					_logger.InfoFormat("Try to execute query '{0}' ", planFull.Query.Full);
					QueryRunResult result = DoRun(planFull, page);
					_logger.InfoFormat("Query was executed successfully '{0}' in {1} ms", planFull.Query.Full, s.Elapsed.TotalMilliseconds);
					return result;
				}
				catch(DocumentIndexConcurrentAccessException)
				{
					_logger.InfoFormat("Try to execute query '{0}' failed", planFull.Query.Full);
					if(--tryCounter == 0)
					{
						_logger.InfoFormat("Cannot execute query '{0}' after {1} tries", planFull.Query.Full, maxTryCount);
						throw;
					}
				}
			}
		}

		private QueryRunResult DoRun(QueryPlanFull planFull, Page page)
		{
			Maybe<QueryPlanResult> entityPlan = Maybe.Nothing;
			Maybe<QueryPlanResult> testStepPlan = Maybe.Nothing;
			Maybe<QueryPlanResult> commentPlan = Maybe.Nothing;
			Parallel.Invoke(
				() => entityPlan = planFull.EntityPlan.Select(p => p.Eval()),
				() => testStepPlan = planFull.TestStepPlan.Select(p => p.Eval()),
				() => commentPlan = planFull.CommentPlan.Select(p => p.Eval()));
		    int entitiesTotalCount;
			int testStepsTotalCount;
			int commentsTotalCount;
			var entityDocuments = FindEntities(entityPlan, page, out entitiesTotalCount);
			int documentCount = entityDocuments.Documents.Count();
			var testStepDocuments = FindTestSteps(testStepPlan, page, documentCount, entitiesTotalCount, out testStepsTotalCount);
			documentCount += testStepDocuments.Documents.Count();
			var commentDocuments = FindComments(commentPlan, page, documentCount, entitiesTotalCount + testStepsTotalCount, out commentsTotalCount);
			
			return new QueryRunResult
				{
					Entities = entityDocuments.Documents,
					Comments = commentDocuments.Documents,
					TestSteps =  testStepDocuments.Documents,
					EntitiesTotalCount = entitiesTotalCount,
					CommentsTotalCount = commentsTotalCount,
					TestStepsTotalCount = testStepsTotalCount,
					LastIndexedEntityId = entityDocuments.LastIndexedId,
					LastIndexedCommentId = commentDocuments.LastIndexedId,
					LastIndexedTestStepId = testStepDocuments.LastIndexedId
				};
		}

		private QueryResult<hOOt.Document> FindComments(Maybe<QueryPlanResult> plan, Page page, int entityDocumentsCountOnPage, int entitiesTotalCount, out int commentsTotalCount)
		{
			if (!plan.HasValue)
			{
				commentsTotalCount = 0;
				return new QueryResult<hOOt.Document>
					{
						Documents = Enumerable.Empty<hOOt.Document>(),
						LastIndexedId = 0
					};
			}
			var commentIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.Comment);
			if (entityDocumentsCountOnPage == page.Size)
			{
				commentIndex.Find<hOOt.Document>(plan.Value, -1, -1, 0, out commentsTotalCount);
				return new QueryResult<hOOt.Document>
					{
						Documents = Enumerable.Empty<hOOt.Document>(),
						LastIndexedId = 0
					};
			}
			Page newPage = CalculatePage(entitiesTotalCount, page.Number, page.Size);
			IEnumerable<hOOt.Document> found = commentIndex.Find<hOOt.Document>(plan.Value, newPage.Number, newPage.Size, newPage.PositionWithin, out commentsTotalCount);
			var lastDocument = commentIndex.GetLastDocument<hOOt.Document>();
			return new QueryResult<hOOt.Document>
				{
					Documents = found,
					LastIndexedId = lastDocument != null ? int.Parse(lastDocument.FileName) : 0
				};
		}

		private QueryResult<hOOt.Document> FindTestSteps(Maybe<QueryPlanResult> plan, Page page, int entityDocumentsCountOnPage, int entitiesTotalCount, out int testStepsTotalCount)
		{
			if (!plan.HasValue)
			{
				testStepsTotalCount = 0;
				return new QueryResult<hOOt.Document>
				{
					Documents = Enumerable.Empty<hOOt.Document>(),
					LastIndexedId = 0
				};
			}
			var testStepIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.TestStep);
			if (entityDocumentsCountOnPage == page.Size)
			{
				testStepIndex.Find<hOOt.Document>(plan.Value, -1, -1, 0, out testStepsTotalCount);
				return new QueryResult<hOOt.Document>
				{
					Documents = Enumerable.Empty<hOOt.Document>(),
					LastIndexedId = 0
				};
			}
			Page newPage = CalculatePage(entitiesTotalCount, page.Number, page.Size);
			IEnumerable<hOOt.Document> found = testStepIndex.Find<hOOt.Document>(plan.Value, newPage.Number, newPage.Size, newPage.PositionWithin, out testStepsTotalCount);
			var lastDocument = testStepIndex.GetLastDocument<hOOt.Document>();
			return new QueryResult<hOOt.Document>
			{
				Documents = found,
				LastIndexedId = lastDocument != null ? int.Parse(lastDocument.FileName) : 0
			};
		}

		private QueryResult<EntityDocument> FindEntities(Maybe<QueryPlanResult> plan, Page page, out int entitiesTotalCount)
		{
			if (!plan.HasValue)
			{
				entitiesTotalCount = 0;
				return new QueryResult<EntityDocument>
					{
						Documents = Enumerable.Empty<EntityDocument>(),
						LastIndexedId = 0
					};
			}
			IDocumentIndex entityIndexes = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.Entity);
			IEnumerable<EntityDocument> found = entityIndexes.Find<EntityDocument>(plan.Value, page.Number, page.Size, 0, out entitiesTotalCount);
			var lastDocument = entityIndexes.GetLastDocument<EntityDocument>();
			return new QueryResult<EntityDocument>
				{
					Documents = found,
					LastIndexedId = lastDocument != null ? int.Parse(lastDocument.FileName) : 0
				};
		}

		private Page CalculatePage(int entitiesTotalCount, int pageNumber, int pageSize)
		{
			int fullEntityPagesCount = (int)Math.Floor((decimal)entitiesTotalCount / pageSize);
			bool lastPageFullOfEntities = fullEntityPagesCount == pageNumber;
			int entitiesCountOnFirstNotFullEntitiesPage = entitiesTotalCount - fullEntityPagesCount * pageSize;
			return new Page
			{
				Number = lastPageFullOfEntities ? 0 : pageNumber - fullEntityPagesCount - 1,
				Size = lastPageFullOfEntities ? pageSize - entitiesCountOnFirstNotFullEntitiesPage : pageSize,
				PositionWithin = lastPageFullOfEntities ? 0 : pageSize - entitiesCountOnFirstNotFullEntitiesPage
			};
		}

		private struct QueryResult<T>
		{
			public IEnumerable<T> Documents { get; set; }
			public int LastIndexedId { get; set; }
		}
	}
}