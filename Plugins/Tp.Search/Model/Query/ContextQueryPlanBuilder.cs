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
	internal class ContextQueryPlanBuilder
	{
		private readonly IDocumentIndexProvider _documentIndexProvider;
		private readonly IDocumentIdFactory _documentIdFactory;
		private readonly AccountName _accountName;
		private readonly IProfileReadonly _profile;
		private readonly IEntityTypeProvider _entityTypeProvider;

        public ContextQueryPlanBuilder(IDocumentIndexProvider documentIndexProvider, IDocumentIdFactory documentIdFactory, AccountName accountName, IProfileReadonly profile, IEntityTypeProvider entityTypeProvider)
		{
			_documentIndexProvider = documentIndexProvider;
			_documentIdFactory = documentIdFactory;
            _accountName = accountName;
			_profile = profile;
			_entityTypeProvider = entityTypeProvider;
		}

		public Maybe<Lazy<WAHBitArray>> Build(QueryData data, DocumentIndexTypeToken projectContextType, DocumentIndexTypeToken squadContextType, DocumentIndexTypeToken entityType)
		{
			var projectContextPlan = BuildProjectContextPlan(data.ProjectIds, data.IncludeNoProject, projectContextType);
			var squadPlan = BuildSquadPlan(data.TeamIds, data.IncludeNoTeam, squadContextType);
			var projectAndSquad = Op(projectContextPlan, squadPlan, Operation.And);
			var noSquadEntityPlan = BuildNoSquadEntityProjectContextPlan(data, projectContextType, entityType);
			var resultPlan = Op(projectAndSquad, noSquadEntityPlan, Operation.Or);
			var projectsReachableThroughTeamsPlan = BuildProjectReachableThroughTeamContextPlan(data, projectContextType, squadContextType);
			return Op(resultPlan, projectsReachableThroughTeamsPlan, Operation.Or);
		}

		private Maybe<Lazy<WAHBitArray>> BuildProjectReachableThroughTeamContextPlan(QueryData data, DocumentIndexTypeToken projectContextType, DocumentIndexTypeToken squadContextType)
		{
			if (data.TeamProjectRelations == null)
			{
				return Maybe.Nothing;
			}
			Maybe<Lazy<WAHBitArray>> result = Maybe.Nothing;
			foreach (var projectsReachableThroughTeamData in data.TeamProjectRelations)
			{
				var projectContextPlan = BuildProjectContextPlan(projectsReachableThroughTeamData.ProjectIds, false, projectContextType);
				var squadPlan = BuildSquadPlan(new[] { projectsReachableThroughTeamData.TeamId }, false, squadContextType);
				var temp = Op(projectContextPlan, squadPlan, Operation.And);
				result = Op(result, temp, Operation.Or);
			}
			return result;
		}

		private Maybe<Lazy<WAHBitArray>> BuildProjectContextPlan(IEnumerable<int> projectIds, bool includeNoProject, DocumentIndexTypeToken documentIndexTypeToken)
		{
			var queryBuf = new List<string>();
			if (projectIds != null)
			{
				queryBuf.AddRange(projectIds.Select(projectId => _documentIdFactory.EncodeProjectId(projectId)));
			}
			if (includeNoProject)
			{
				queryBuf.Add(_documentIdFactory.EncodeProjectId(null));
			}
			if (!queryBuf.Any())
			{
				return Maybe.Nothing;
			}
			string query = String.Join(" ", queryBuf.ToArray());
			IDocumentIndex projectContextIndices = _documentIndexProvider.GetOrCreateDocumentIndex(_accountName, documentIndexTypeToken);
			return projectContextIndices.BuildExecutionPlan(query, _profile.Initialized);
		}

		private Maybe<Lazy<WAHBitArray>> BuildSquadPlan(IEnumerable<int> squadIds, bool includeNoTeam, DocumentIndexTypeToken squadContextType)
		{
			var queryBuf = new List<string>();
			if (squadIds != null)
			{
				queryBuf.AddRange(squadIds.Select(squadId => _documentIdFactory.EncodeSquadId(squadId)));
			}
			if (includeNoTeam)
			{
				queryBuf.Add(_documentIdFactory.EncodeSquadId(null));
			}
			if (!queryBuf.Any())
			{
				return Maybe.Nothing;
			}
			string query = String.Join(" ", queryBuf.ToArray());
			IDocumentIndex squadIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_accountName, squadContextType);
			return squadIndex.BuildExecutionPlan(query, _profile.Initialized);
		}

		private Maybe<Lazy<WAHBitArray>> BuildNoSquadEntityProjectContextPlan(QueryData data, DocumentIndexTypeToken project, DocumentIndexTypeToken entityType)
		{
			var projectContextPlan = BuildProjectContextPlan(data.ProjectIds, data.IncludeNoProject, project);
			if (!projectContextPlan.HasValue)
			{
				return Maybe.Nothing;
			}
			string noSquadEntityTypeIdsQuery = String.Join(" ", _entityTypeProvider.NoSquadEntityTypeNames);
			IDocumentIndex entityTypeIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_accountName, entityType);
			Lazy<WAHBitArray> noSquadEntityPlan = entityTypeIndex.BuildExecutionPlan(noSquadEntityTypeIdsQuery, _profile.Initialized);
			return Maybe.Return(Lazy.Create(() => noSquadEntityPlan.Value.And(projectContextPlan.Value.Value)));
		}

		private enum Operation
		{
			And,
			Or
		}

		private Maybe<Lazy<WAHBitArray>> Op(Maybe<Lazy<WAHBitArray>> left, Maybe<Lazy<WAHBitArray>> right, Operation operation)
		{
			if (left.HasValue && right.HasValue)
			{
				switch (operation)
				{
					case Operation.And:
						return Maybe.Return(Lazy.Create(() => left.Value.Value.And(right.Value.Value)));
					case Operation.Or:
						return Maybe.Return(Lazy.Create(() => left.Value.Value.Or(right.Value.Value)));
					default:
						throw new ArgumentException("{0} is not supported.".Fmt(operation));
				}
			}
			return left.HasValue ? left : right;
		}
	}
}