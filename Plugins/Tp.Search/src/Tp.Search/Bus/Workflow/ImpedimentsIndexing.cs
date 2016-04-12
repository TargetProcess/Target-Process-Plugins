// 
// Copyright (c) 2005-2014 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Queries;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Search.Model.Document;
using Tp.Search.Model.Entity;

namespace Tp.Search.Bus.Workflow
{
	internal class ImpedimentsIndexing : IndexAlgorithm<ImpedimentDTO, IndexExistingEntitiesSagaData, ImpedimentQuery>
	{
		public ImpedimentsIndexing(IEntityIndexer entityIndexer, Func<IndexExistingEntitiesSagaData> data, IEntityTypeProvider entityTypesProvider, Action<IndexExistingEntitiesSagaData> onComplete, Action<QueryBase> sendQuery, IActivityLogger logger)
			: base(entityIndexer, data, entityTypesProvider, onComplete, sendQuery, logger, "impediment")
		{
		}

		protected override void IndexEntity(ImpedimentDTO dto)
		{
			EntityIndexer.UpdateImpedimentIndex(dto, new[] { ImpedimentField.EntityStateID, ImpedimentField.OwnerID, ImpedimentField.ResponsibleID, ImpedimentField.IsPrivate }, true, DocumentIndexOptimizeSetup.NoOptimize);
		}

		protected override void OptimizeIndex()
		{
			EntityIndexer.OptimizeImpedimentIndex(DocumentIndexOptimizeSetup.ImmediateOptimize);
		}

		protected override void IncrementCounters(int count)
		{
			Data.ImpedimentsRetrievedCount += count;
			Data.ImpedimentsCurrentDataWindowSize += count;
		}

		protected override int GetCurrentDataWindowSize()
		{
			return Data.ImpedimentsCurrentDataWindowSize;
		}

		protected override void ResetCurrentDataWindowSize()
		{
			Data.ImpedimentsCurrentDataWindowSize = 0;
		}

		protected override int GetTotalRetrievedEntitiesCount()
		{
			return Data.ImpedimentsRetrievedCount;
		}

		protected override void ResetTotalRetrievedEntitiesCount()
		{
			Data.ImpedimentsRetrievedCount = 0;
		}

		protected override ImpedimentQuery CreateQuery()
		{
			return new ImpedimentQuery();
		}
	}
}