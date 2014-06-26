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
	class TestCasesIndexing : IndexAlgorithm<TestCaseDTO, IndexExistingEntitiesSagaData, TestCaseQuery>
	{
		public TestCasesIndexing(IEntityIndexer entityIndexer, Func<IndexExistingEntitiesSagaData> data, IEntityTypeProvider entityTypesProvider, Action<IndexExistingEntitiesSagaData> onComplete, Action<QueryBase> sendQuery, IActivityLogger logger)
			: base(entityIndexer, data, entityTypesProvider, onComplete, sendQuery, logger, "testCase")
		{
		}

		protected override void IndexEntity(TestCaseDTO dto)
		{
			EntityIndexer.UpdateTestCaseIndex(dto, new[] { TestCaseField.Steps, TestCaseField.Success }, true, DocumentIndexOptimizeSetup.NoOptimize);
		}

		protected override void OptimizeIndex()
		{
			EntityIndexer.OptimizeTestCaseIndex(DocumentIndexOptimizeSetup.ImmediateOptimize);
		}

		protected override void IncrementCounters(int count)
		{
			Data.TestCasesRetrievedCount += count;
			Data.TestCasesCurrentDataWindowSize += count;
		}

		protected override int GetCurrentDataWindowSize()
		{
			return Data.TestCasesCurrentDataWindowSize;
		}

		protected override void ResetCurrentDataWindowSize()
		{
			Data.TestCasesCurrentDataWindowSize = 0;
		}

		protected override int GetTotalRetrievedEntitiesCount()
		{
			return Data.TestCasesRetrievedCount;
		}

		protected override void ResetTotalRetrievedEntitiesCount()
		{
			Data.TestCasesRetrievedCount = 0;
		}

		protected override TestCaseQuery CreateQuery()
		{
			return new TestCaseQuery();
		}
	}
}