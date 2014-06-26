using System;
using System.Collections.Generic;
using System.Linq;
using Tp.Core;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Search.Bus.Data;
using Tp.Search.Model.Document;
using Tp.Search.Model.Entity;

namespace Tp.Search.Model.Query
{
	internal class QueryPlanBuilder
	{
		private readonly IPluginContext _pluginContext;
		private readonly IProfileReadonly _profile;
		private readonly IDocumentIndexProvider _documentIndexProvider;
		private readonly IEntityTypeProvider _entityTypeProvider;
		private readonly IIndexDataFactory _indexDataFactory;
		private readonly ContextQueryPlanBuilder _contextQueryPlanBuilder;
		public QueryPlanBuilder(IPluginContext pluginContext, IProfileReadonly profile, IDocumentIndexProvider documentIndexProvider, IEntityTypeProvider entityTypeProvider, IIndexDataFactory indexDataFactory, ContextQueryPlanBuilder contextQueryPlanBuilder)
		{
			_pluginContext = pluginContext;
			_profile = profile;
			_documentIndexProvider = documentIndexProvider;
			_entityTypeProvider = entityTypeProvider;
			_indexDataFactory = indexDataFactory;
			_contextQueryPlanBuilder = contextQueryPlanBuilder;
		}

		public QueryPlanFull Build(QueryData data, ParsedQuery parsedQuery)
		{
			Maybe<QueryPlan> entityPlan = CreateEntityPlan(data, parsedQuery);
			Maybe<QueryPlan> commentPlan = CreateCommentPlan(data, parsedQuery);
			return new QueryPlanFull
				{
					Query = parsedQuery,
					EntityPlan = entityPlan,
					CommentPlan = commentPlan
				};
		}
		
		private Maybe<QueryPlan> CreateCommentPlan(QueryData data, ParsedQuery parsedQuery)
		{
			if (!data.ShouldSearchComment)
			{
				return Maybe.Nothing;
			}
			var contextPlan = _contextQueryPlanBuilder.Build(data, DocumentIndexTypeToken.CommentProject, DocumentIndexTypeToken.CommentSquad, DocumentIndexTypeToken.CommentEntityType);
			if (!contextPlan.HasValue)
			{
				return Maybe.Nothing;
			}
			var commentIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.Comment);
			var plan = commentIndex.BuildExecutionPlan(parsedQuery, _profile.Initialized);
			return And(plan, contextPlan);
		}

		private Maybe<QueryPlan> CreateEntityPlan(QueryData queryData, ParsedQuery parsedQuery)
		{
			if (queryData.IsCommentEntityType)
			{
				return Maybe.Nothing;
			}
			var contextPlan = _contextQueryPlanBuilder.Build(queryData, DocumentIndexTypeToken.EntityProject, DocumentIndexTypeToken.EntitySquad, DocumentIndexTypeToken.EntityType);
			if (!contextPlan.HasValue)
			{
				return Maybe.Nothing;
			}
			QueryPlan plan = CreateEntityQueryPlan(parsedQuery);
			plan = And(plan, CreateEntityTypePlan(queryData.EntityTypeId));
			plan = And(plan, CreateEntityStatePlan(queryData.EntityStateIds));
			plan = And(plan, contextPlan);
			return plan;
		}

		private Maybe<QueryPlan> CreateEntityTypePlan(int? entityTypeId)
		{
			if (entityTypeId == null)
			{
				return Maybe.Nothing;
			}
			Maybe<string> maybeEntityTypeName = _entityTypeProvider.GetEntityTypeName(entityTypeId);
			string entityTypeName = maybeEntityTypeName.GetOrThrow(() => new ApplicationException("Entity type name was not found {0}".Fmt(entityTypeId)));
			var entityTypeFinders = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.EntityType);
			return entityTypeFinders.BuildExecutionPlan(new ParsedQuery(words:string.Format("+{0}", entityTypeName.ToLower())), _profile.Initialized);
		}

		private QueryPlan And(QueryPlan left, Maybe<QueryPlan> right)
		{
			return right.HasValue ? QueryPlan.And(left, right.Value) : left;
		}

		private QueryPlan CreateEntityQueryPlan(ParsedQuery parsedQuery)
		{
			IDocumentIndex entityIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.Entity);
			return entityIndex.BuildExecutionPlan(parsedQuery, _profile.Initialized);
		}

		private Maybe<QueryPlan> CreateEntityStatePlan(IEnumerable<int> entityStateIds)
		{
			if (entityStateIds == null)
			{
				return Maybe.Nothing;
			}
			if (!entityStateIds.Any())
			{
				return Maybe.Nothing;
			}
			var queryBuf = new List<string>();
			IDocumentIndex entityStateIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.EntityState);
			queryBuf.AddRange(entityStateIds.Select(entityStateId => _indexDataFactory.CreateEntityStateData(entityStateId)));
			string query = string.Join(" ", queryBuf.ToArray());
			return entityStateIndex.BuildExecutionPlan(new ParsedQuery(words:query), _profile.Initialized);
		}
	}
}