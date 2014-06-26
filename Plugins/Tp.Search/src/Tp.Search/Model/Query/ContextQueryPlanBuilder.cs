// 
// Copyright (c) 2005-2014 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

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
	internal class ContextQueryPlanBuilder : IProjectContextQueryPlanBuilder
	{
		private readonly IDocumentIndexProvider _documentIndexProvider;
		private readonly IIndexDataFactory _indexDataFactory;
		private readonly IPluginContext _pluginContext;
		private readonly IProfileReadonly _profile;
		private readonly IEntityTypeProvider _entityTypeProvider;
		private readonly IEnumerable<IContextQueryPlanBuilder> _planBuilders;

		public ContextQueryPlanBuilder(IDocumentIndexProvider documentIndexProvider, IIndexDataFactory indexDataFactory, IPluginContext pluginContext, IProfileReadonly profile, IEntityTypeProvider entityTypeProvider, IEnumerable<IContextQueryPlanBuilder> planBuilders)
		{
			_documentIndexProvider = documentIndexProvider;
			_indexDataFactory = indexDataFactory;
			_pluginContext = pluginContext;
			_profile = profile;
			_entityTypeProvider = entityTypeProvider;
			_planBuilders = planBuilders;
		}

		public Maybe<QueryPlan> Build(QueryData data, DocumentIndexTypeToken projectContextType, DocumentIndexTypeToken squadContextType, DocumentIndexTypeToken entityType)
		{
			var projectContextPlan = ContextPlansBuilder.BuildProjectContextPlan(data.ProjectIds, data.IncludeNoProject, projectContextType);
			var squadPlan = BuildSquadPlan(data.TeamIds, data.IncludeNoTeam, squadContextType);
			var projectAndSquad = projectContextPlan.And(squadPlan);
			var noSquadEntityPlan = BuildNoSquadEntityProjectContextPlan(data, projectContextType, entityType);
			var resultPlan = projectAndSquad.Or(noSquadEntityPlan);
			var projectsReachableThroughTeamsPlan = BuildProjectReachableThroughTeamContextPlan(data, projectContextType, squadContextType);
			resultPlan = resultPlan.Or(projectsReachableThroughTeamsPlan);
			resultPlan = _planBuilders.Where(b => b.ShouldBuild(data)).Aggregate(resultPlan, (acc, builder) =>
				{
					var plan = builder.Build(data, projectContextType, squadContextType, entityType);
					return acc.Or(plan);
				});
			return resultPlan;
		}

		private Maybe<QueryPlan> BuildProjectReachableThroughTeamContextPlan(QueryData data, DocumentIndexTypeToken projectContextType, DocumentIndexTypeToken squadContextType)
		{
			if (data.TeamProjectRelations == null)
			{
				return Maybe.Nothing;
			}
			Maybe<QueryPlan> result = Maybe.Nothing;
			foreach (var projectsReachableThroughTeamData in data.TeamProjectRelations)
			{
				var projectContextPlan = ContextPlansBuilder.BuildProjectContextPlan(projectsReachableThroughTeamData.ProjectIds, false, projectContextType);
				var squadPlan = BuildSquadPlan(new[] { projectsReachableThroughTeamData.TeamId }, false, squadContextType);
				var temp = projectContextPlan.And(squadPlan);
				result = result.Or(temp);
			}
			return result;
		}

		Maybe<QueryPlan> IProjectContextQueryPlanBuilder.BuildProjectContextPlan(IEnumerable<int> projectIds, bool includeNoProject, DocumentIndexTypeToken projectIndexTypeToken)
		{
			var queryBuf = new List<string>();
			if (projectIds != null)
			{
				queryBuf.AddRange(projectIds.Select(projectId => _indexDataFactory.CreateProjectData(projectId)));
			}
			if (includeNoProject)
			{
				queryBuf.Add(_indexDataFactory.CreateProjectData(null));
			}
			if (!queryBuf.Any())
			{
				return Maybe.Nothing;
			}
			string query = String.Join(" ", queryBuf.ToArray());
			IDocumentIndex projectContextIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, projectIndexTypeToken);
			return projectContextIndex.BuildExecutionPlan(new ParsedQuery(words:query), _profile.Initialized);
		}

		private IProjectContextQueryPlanBuilder ContextPlansBuilder
		{
			get { return this; }
		}

		private Maybe<QueryPlan> BuildSquadPlan(IEnumerable<int> squadIds, bool includeNoTeam, DocumentIndexTypeToken squadContextType)
		{
			var queryBuf = new List<string>();
			if (squadIds != null)
			{
				queryBuf.AddRange(squadIds.Select(squadId => _indexDataFactory.CreateSquadData(squadId)));
			}
			if (includeNoTeam)
			{
				queryBuf.Add(_indexDataFactory.CreateSquadData(null));
			}
			if (!queryBuf.Any())
			{
				return Maybe.Nothing;
			}
			string query = String.Join(" ", queryBuf.ToArray());
			IDocumentIndex squadIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, squadContextType);
			return squadIndex.BuildExecutionPlan(new ParsedQuery(words:query), _profile.Initialized);
		}

		private Maybe<QueryPlan> BuildNoSquadEntityProjectContextPlan(QueryData data, DocumentIndexTypeToken project, DocumentIndexTypeToken entityType)
		{
			var projectContextPlan = ContextPlansBuilder.BuildProjectContextPlan(data.ProjectIds, data.IncludeNoProject, project);
			if (!projectContextPlan.HasValue)
			{
				return Maybe.Nothing;
			}
			string noSquadEntityTypeIdsQuery = String.Join(" ", _entityTypeProvider.NoSquadEntityTypeNames);
			IDocumentIndex entityTypeIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, entityType);
			QueryPlan noSquadEntityPlan = entityTypeIndex.BuildExecutionPlan(new ParsedQuery(words:noSquadEntityTypeIdsQuery), _profile.Initialized);
			return Maybe.Return(QueryPlan.And(noSquadEntityPlan, projectContextPlan.Value));
		}
	}
}