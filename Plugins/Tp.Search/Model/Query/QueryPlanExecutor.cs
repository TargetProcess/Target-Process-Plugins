using System;
using System.Collections.Generic;
using System.Linq;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Search.Model.Document;
using Tp.Search.Model.Entity;
using hOOt;

namespace Tp.Search.Model.Query
{
	class QueryPlanExecutor
	{
		private readonly IDocumentIndexProvider _documentIndexProvider;
		private readonly IPluginContext _pluginContext;

		public QueryPlanExecutor(IDocumentIndexProvider documentIndexProvider, IPluginContext pluginContext)
		{
			_documentIndexProvider = documentIndexProvider;
			_pluginContext = pluginContext;
		}

		public QueryRunResult Run(QueryPlan plan, Page page)
		{
			int entitiesTotalCount = 0;
			int commentsTotalCount = 0;
			var entityDocuments = plan.EntityPlan.HasValue ? FindEntities(plan.EntityPlan.Value, page, out entitiesTotalCount) : Tuple.Create(Enumerable.Empty<EntityDocument>(),0);
			var commentDocuments = plan.CommentPlan.HasValue ? FindComments(plan.CommentPlan.Value, page, entityDocuments.Item1, entitiesTotalCount, out commentsTotalCount) : Tuple.Create(Enumerable.Empty<hOOt.Document>(), 0);
			return new QueryRunResult
				{
					Entities = entityDocuments.Item1,
					Comments = commentDocuments.Item1,
					EntitiesTotalCount = entitiesTotalCount,
					CommentsTotalCount = commentsTotalCount,
					LastIndexedEntityId = entityDocuments.Item2,
					LastIndexedCommentId = commentDocuments.Item2
				};
		}

		private Tuple<IEnumerable<hOOt.Document>,int> FindComments(Lazy<WAHBitArray> plan, Page page, IEnumerable<EntityDocument> entityDocuments, int entitiesTotalCount, out int commentsTotalCount)
		{
			var commentIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, DocumentIndexTypeToken.Comment);
			if (entityDocuments.Count() == page.Size)
			{
				commentIndex.Find<hOOt.Document>(plan, -1, -1, 0, out commentsTotalCount);
				return Tuple.Create(Enumerable.Empty<hOOt.Document>(), 0);
			}
			Page newPage = CalculatePage(entitiesTotalCount, page.Number, page.Size);
			IEnumerable<hOOt.Document> found =  commentIndex.Find<hOOt.Document>(plan, newPage.Number, newPage.Size, newPage.PositionWithin, out commentsTotalCount);
			var lastDocument = commentIndex.GetLastDocument<hOOt.Document>();
			return Tuple.Create(found, lastDocument != null ? int.Parse(lastDocument.FileName) : 0);
		}

		private Tuple<IEnumerable<EntityDocument>,int> FindEntities(Lazy<WAHBitArray> plan, Page page, out int entitiesTotalCount)
		{
			IDocumentIndex entityIndexes = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext.AccountName, DocumentIndexTypeToken.Entity);
			IEnumerable<EntityDocument> found =  entityIndexes.Find<EntityDocument>(plan, page.Number, page.Size, 0, out entitiesTotalCount);
			var lastDocument = entityIndexes.GetLastDocument<EntityDocument>();
			return Tuple.Create(found, lastDocument != null ? int.Parse(lastDocument.FileName) : 0);
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
	}
}