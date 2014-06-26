using System.Linq;
using Tp.Search.Bus.Data;

namespace Tp.Search.Model.Query
{
	class QueryRunner
	{
		private readonly QueryParser _queryParser;
		private readonly QueryPlanBuilder _queryPlanBuilder;
		private readonly QueryEngine _queryEngine;
		private readonly IQueryResultFactory _queryResultFactory;

		public QueryRunner(QueryParser queryParser, QueryPlanBuilder queryPlanBuilder, QueryEngine queryEngine, IQueryResultFactory queryResultFactory)
		{
			_queryParser = queryParser;
			_queryPlanBuilder = queryPlanBuilder;
			_queryEngine = queryEngine;
			_queryResultFactory = queryResultFactory;
		}

		public QueryResult Run(QueryData data)
		{
			if (string.IsNullOrEmpty(data.Query))
			{
				return new QueryResult();
			}
			var parsedQuery = _queryParser.Parse(data.Query);
			QueryPlanFull queryPlanFull = _queryPlanBuilder.Build(data, parsedQuery);
			QueryRunResult result = _queryEngine.Run(queryPlanFull, Map(data.Page));
			return CreateResult(result, data);
		}

		private Page Map(PageData page)
		{
			return new Page
				{
					Number = page.Number,
					Size = page.Size
				};
		}

		private QueryResult CreateResult(QueryRunResult queryResult, QueryData queryData)
		{
			QueryEntityTypeProvider.SearchResult result = _queryResultFactory.CreateQueryResult(queryResult.Entities);
			var searchResult = new QueryResult
				{
					GeneralIds = result.GeneralIds.ToArray(),
					AssignableIds = result.AssignableIds.ToArray(),
					TestCaseIds = result.TestCaseIds.ToArray(),
					ImpedimentIds =  result.ImpedimentIds.ToArray(),
					CommentIds = queryResult.Comments.Select(i => i.FileName).ToArray(),
					QueryString = queryData.Query,
					Total = queryResult.EntitiesTotalCount + queryResult.CommentsTotalCount,
					LastIndexedEntityId = queryResult.LastIndexedEntityId,
					LastIndexedCommentId = queryResult.LastIndexedCommentId
				};
			return searchResult;
		}
	}
}