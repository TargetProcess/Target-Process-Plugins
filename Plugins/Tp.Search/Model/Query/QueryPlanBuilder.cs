using System;
using System.Collections.Generic;
using System.Linq;
using Tp.Core;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Search.Bus.Data;
using Tp.Search.Model.Document;
using Tp.Search.Model.Entity;
using hOOt;
using Tp.Integration.Messages;

namespace Tp.Search.Model.Query
{
	internal class QueryPlanBuilder
	{
        private readonly AccountName _accountName;
		private readonly IProfileReadonly _profile;
		private readonly IDocumentIndexProvider _documentIndexProvider;
		private readonly IEntityTypeProvider _entityTypeProvider;
		private readonly IDocumentIdFactory _documentIdFactory;
		private readonly ContextQueryPlanBuilder _contextQueryPlanBuilder;
		public QueryPlanBuilder(IPluginContext pluginContext, IProfileReadonly profile, IDocumentIndexProvider documentIndexProvider, IEntityTypeProvider entityTypeProvider, IDocumentIdFactory documentIdFactory)
		{
			_accountName = pluginContext.AccountName;
			_profile = profile;
			_documentIndexProvider = documentIndexProvider;
			_entityTypeProvider = entityTypeProvider;
			_documentIdFactory = documentIdFactory;
			_contextQueryPlanBuilder = new ContextQueryPlanBuilder(_documentIndexProvider, _documentIdFactory, pluginContext.AccountName, _profile, _entityTypeProvider);
		}

		public QueryPlan Build(QueryData data, string query)
		{
			Maybe<Lazy<WAHBitArray>> entityPlan = CreateEntityPlan(data, query);
			Maybe<Lazy<WAHBitArray>> commentPlan = CreateCommentPlan(data, query);
			return new QueryPlan
				{
					EntityPlan = entityPlan,
					CommentPlan = commentPlan
				};
		}
		
		private Maybe<Lazy<WAHBitArray>> CreateCommentPlan(QueryData data, string query)
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
			var commentIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_accountName, DocumentIndexTypeToken.Comment);
			var plan = commentIndex.BuildExecutionPlan(query, _profile.Initialized);
			return And(plan, contextPlan);
		}

		private Maybe<Lazy<WAHBitArray>> CreateEntityPlan(QueryData queryData, string query)
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
			Lazy<WAHBitArray> plan = CreateEntityQueryPlan(query);
			plan = And(plan, CreateEntityTypePlan(queryData.EntityTypeId));
			plan = And(plan, CreateEntityStatePlan(queryData.EntityStateIds));
			plan = And(plan, contextPlan);
			return plan;
		}

		private Maybe<Lazy<WAHBitArray>> CreateEntityTypePlan(int? entityTypeId)
		{
			if (entityTypeId == null)
			{
				return Maybe.Nothing;
			}
			Maybe<string> maybeEntityTypeName = _entityTypeProvider.GetEntityTypeName(entityTypeId);
			string entityTypeName = maybeEntityTypeName.FailIfNothing(() => new ApplicationException("Entity type name was not found {0}".Fmt(entityTypeId)));
			var entityTypeFinders = _documentIndexProvider.GetOrCreateDocumentIndex(_accountName, DocumentIndexTypeToken.EntityType);
			return entityTypeFinders.BuildExecutionPlan(String.Format("+{0}", entityTypeName.ToLower()), _profile.Initialized);
		}

		private Lazy<WAHBitArray> And(Lazy<WAHBitArray> left, Maybe<Lazy<WAHBitArray>> right)
		{
			return right.HasValue ? Lazy.Create(() => left.Value.And(right.Value.Value)) : left;
		}
		
		private Lazy<WAHBitArray> CreateEntityQueryPlan(string query)
		{
			IDocumentIndex entityIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_accountName, DocumentIndexTypeToken.Entity);
			return entityIndex.BuildExecutionPlan(query, _profile.Initialized);
		}

		private Maybe<Lazy<WAHBitArray>> CreateEntityStatePlan(IEnumerable<int> entityStateIds)
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
			IDocumentIndex entityStateIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_accountName, DocumentIndexTypeToken.EntityState);
			queryBuf.AddRange(entityStateIds.Select(entityStateId => _documentIdFactory.EncodeEntityStateId(entityStateId)));
			string query = string.Join(" ", queryBuf.ToArray());
			return entityStateIndex.BuildExecutionPlan(query, _profile.Initialized);
		}
	}
}