using System;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Queries;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Search.Model.Document;
using Tp.Search.Model.Entity;

namespace Tp.Search.Bus.Workflow
{
	class AssignableSquadIndexing : IndexAlgorithm<AssignableSquadDTO, IndexExistingEntitiesSagaData, RetrieveAllAssignableSquadsQuery>
	{
		public AssignableSquadIndexing(IEntityIndexer entityIndexer, Func<IndexExistingEntitiesSagaData> data, IEntityTypeProvider entityTypesProvider, Action<IndexExistingEntitiesSagaData> onComplete, Action<QueryBase> sendQuery, IActivityLogger logger)
			: base(entityIndexer, data, entityTypesProvider, onComplete, sendQuery, logger, "assignableSquad")
		{
		}

		protected override void IndexEntity(AssignableSquadDTO dto)
		{
			EntityIndexer.AddAssignableSquadIndex(dto, DocumentIndexOptimizeSetup.NoOptimize);
		}

		protected override void OptimizeIndex()
		{
		}

		protected override void IncrementCounters(int count)
		{
			Data.ReleaseProjectsRetrievedCount += count;
			Data.ReleaseProjectsCurrentDataWindowSize += count;
		}

		protected override int GetCurrentDataWindowSize()
		{
			return Data.ReleaseProjectsCurrentDataWindowSize;
		}

		protected override void ResetCurrentDataWindowSize()
		{
			Data.ReleaseProjectsCurrentDataWindowSize = 0;
		}

		protected override int GetTotalRetrievedEntitiesCount()
		{
			return Data.ReleaseProjectsRetrievedCount;
		}

		protected override void ResetTotalRetrievedEntitiesCount()
		{
			Data.ReleaseProjectsRetrievedCount = 0;
		}

		protected override RetrieveAllAssignableSquadsQuery CreateQuery()
		{
			return new RetrieveAllAssignableSquadsQuery();
		}
	}
}