using Tp.Core;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Search.Bus.Data;
using Tp.Search.Model.Document;
using Tp.Search.Model.Entity;

namespace Tp.Search.Model.Query
{
	class ImpedimentContextQueryPlanBuilder : IContextQueryPlanBuilder
	{
		private readonly IProjectContextQueryPlanBuilder _plansBuilder;
		private readonly IProfileReadonly _profile;
		private readonly IIndexDataFactory _indexDataFactory;
		private readonly IDocumentIndexProvider _documentIndexProvider;
		private readonly IPluginContext _pluginContext;
		private readonly IEntityTypeProvider _entityTypeProvider;

		public ImpedimentContextQueryPlanBuilder(IProjectContextQueryPlanBuilder plansBuilder, IProfileReadonly profile, IIndexDataFactory indexDataFactory, IDocumentIndexProvider documentIndexProvider, IPluginContext pluginContext, IEntityTypeProvider entityTypeProvider)
		{
			_plansBuilder = plansBuilder;
			_profile = profile;
			_indexDataFactory = indexDataFactory;
			_documentIndexProvider = documentIndexProvider;
			_pluginContext = pluginContext;
			_entityTypeProvider = entityTypeProvider;
		}

		public Maybe<QueryPlan> Build(QueryData data, DocumentIndexTypeToken projectContextType, DocumentIndexTypeToken _, DocumentIndexTypeToken __)
		{
			var query = _indexDataFactory.CreateImpedimentData(false, data.LoggedUserId, data.LoggedUserId);
			IDocumentIndex impedimentContextIndex = _documentIndexProvider.GetOrCreateDocumentIndex(_pluginContext, DocumentIndexTypeToken.Impediment);
			var impedimentPlan = impedimentContextIndex.BuildExecutionPlan(new ParsedQuery(words: query), _profile.Initialized);
			var projectContextPlan = _plansBuilder.BuildProjectContextPlan(data.ProjectIds, data.IncludeNoProject, projectContextType);
			return projectContextPlan.And(Maybe.Return(impedimentPlan));
		}

		public bool ShouldBuild(QueryData data)
		{
			if (data.LoggedUserId == null)
			{
				return false;
			}
			if(data.EntityTypeId == null)
			{
				return true;
			}
			return _entityTypeProvider.IsImpediment(data.EntityTypeId);
		}
	}
}