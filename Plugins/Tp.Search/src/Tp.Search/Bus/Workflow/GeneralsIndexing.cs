using System;
using System.Linq;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Queries;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Search.Model.Document;
using Tp.Search.Model.Entity;

namespace Tp.Search.Bus.Workflow
{
	class GeneralsIndexing : IndexAlgorithm<GeneralDTO, IndexExistingEntitiesSagaData, GeneralQuery>
	{
		public GeneralsIndexing(IEntityIndexer entityIndexer, Func<IndexExistingEntitiesSagaData> data, IEntityTypeProvider entityTypesProvider, Action<IndexExistingEntitiesSagaData> onComplete, Action<QueryBase> sendQuery, IActivityLogger logger)
			: base(entityIndexer, data, entityTypesProvider, onComplete, sendQuery, logger, "general")
		{
		}

		protected override void IndexEntity(GeneralDTO dto)
		{
			EntityIndexer.AddGeneralIndex(dto, DocumentIndexOptimizeSetup.NoOptimize);
		}

		protected override void OptimizeIndex()
		{
			EntityIndexer.OptimizeGeneralIndex(DocumentIndexOptimizeSetup.ImmediateOptimize);
		}

		protected override void IncrementCounters(int count)
		{
			Data.GeneralsRetrievedCount += count;
			Data.GeneralsCurrentDataWindowSize += count;
		}

		protected override int GetCurrentDataWindowSize()
		{
			return Data.GeneralsCurrentDataWindowSize;
		}

		protected override void ResetCurrentDataWindowSize()
		{
			Data.GeneralsCurrentDataWindowSize = 0;
		}

		protected override int GetTotalRetrievedEntitiesCount()
		{
			return Data.GeneralsRetrievedCount;
		}

		protected override void ResetTotalRetrievedEntitiesCount()
		{
			Data.GeneralsRetrievedCount = 0;
		}

		protected override GeneralQuery CreateQuery()
		{
			return new GeneralQuery
				{
					EntityTypes = EntityTypesProvider.EntityTypeIds.ToArray()
				};
		}
	}
}