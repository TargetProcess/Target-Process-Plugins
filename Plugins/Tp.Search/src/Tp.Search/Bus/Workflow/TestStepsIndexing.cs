// 
// Copyright (c) 2005-2015 TargetProcess. All rights reserved.
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
	public interface ITestStepIndexingSagaData
	{
		int TestStepsRetrievedCount { get; set; }
		int TestStepsCurrentDataWindowSize { get; set; }
	}

	internal class TestStepsIndexing : IndexAlgorithm<TestStepDTO, ITestStepIndexingSagaData, TestStepQuery>
	{
		private readonly Action<TestStepDTO, IEntityIndexer> _indexMethod;

		public TestStepsIndexing(IEntityIndexer entityIndexer, Func<ITestStepIndexingSagaData> data,
			IEntityTypeProvider entityTypesProvider, Action<ITestStepIndexingSagaData> onComplete,
			Action<TestStepQuery> sendQuery, IActivityLogger logger, Action<TestStepDTO, IEntityIndexer> indexMethod)
			: base(entityIndexer, data, entityTypesProvider, onComplete, sendQuery, logger, "testStep")
		{
			_indexMethod = indexMethod;
		}

		protected override void IndexEntity(TestStepDTO dto)
		{
			_indexMethod(dto, EntityIndexer);
		}

		protected override void OptimizeIndex()
		{
			EntityIndexer.OptimizeTestStepIndex(DocumentIndexOptimizeSetup.ImmediateOptimize);
		}

		protected override void IncrementCounters(int count)
		{
			Data.TestStepsRetrievedCount += count;
			Data.TestStepsCurrentDataWindowSize += count;
		}

		protected override int GetCurrentDataWindowSize()
		{
			return Data.TestStepsCurrentDataWindowSize;
		}

		protected override void ResetCurrentDataWindowSize()
		{
			Data.TestStepsCurrentDataWindowSize = 0;
		}

		protected override int GetTotalRetrievedEntitiesCount()
		{
			return Data.TestStepsRetrievedCount;
		}

		protected override void ResetTotalRetrievedEntitiesCount()
		{
			Data.TestStepsRetrievedCount = 0;
		}

		protected override TestStepQuery CreateQuery()
		{
			return new TestStepQuery();
		}
	}
}