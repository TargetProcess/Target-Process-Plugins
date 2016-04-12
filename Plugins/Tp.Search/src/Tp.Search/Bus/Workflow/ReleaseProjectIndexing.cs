using System;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Queries;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Search.Model.Document;
using Tp.Search.Model.Entity;

namespace Tp.Search.Bus.Workflow
{
	class ReleaseProjectIndexing : IndexAlgorithm<ReleaseProjectDTO, IndexExistingEntitiesSagaData, ReleaseProjectQuery>
	{
		public ReleaseProjectIndexing(IEntityIndexer entityIndexer, Func<IndexExistingEntitiesSagaData> data, IEntityTypeProvider entityTypesProvider, Action<IndexExistingEntitiesSagaData> onComplete, Action<QueryBase> sendQuery, IActivityLogger logger)
			: base(entityIndexer, data, entityTypesProvider, onComplete, sendQuery, logger, "releaseProject")
		{
		}

		protected override void IndexEntity(ReleaseProjectDTO dto)
		{
			EntityIndexer.AddReleaseProjectIndex(dto, DocumentIndexOptimizeSetup.NoOptimize);
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

		protected override ReleaseProjectQuery CreateQuery()
		{
			return new ReleaseProjectQuery();
		}
	}
}