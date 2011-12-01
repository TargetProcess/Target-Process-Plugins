// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using NServiceBus;
using NServiceBus.Saga;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Queries;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.TestRunImport.Streams;

namespace Tp.Integration.Plugin.TestRunImport
{
	public class TestRunImportPluginUpdatedProfileInitializationSaga :
		UpdatedProfileInitializationSaga<TestRunImportPluginUpdatedProfileInitializationData>,
		IHandleMessages<TestCaseTestPlanQueryResult>
	{
		private readonly IStorageRepository _storageRepository;

		public TestRunImportPluginUpdatedProfileInitializationSaga()
		{
		}

		public TestRunImportPluginUpdatedProfileInitializationSaga(IStorageRepository storageRepository)
		{
			_storageRepository = storageRepository;
		}

		public override void ConfigureHowToFindSaga()
		{
			ConfigureMapping<TestCaseTestPlanQueryResult>(saga => saga.Id, message => message.SagaId);
		}
		
		protected override void OnStartInitialization()
		{
			var profile = _storageRepository.GetProfile<TestRunImportPluginProfile>();
			StorageRepository().Get<TestCaseTestPlanDTO>().Clear();
			StorageRepository().Get<LastModifyResult>().Clear();
			Data.AllTestCaseTestPlansCount = int.MinValue;
			Send(new TestCaseTestPlanQuery { TestPlanId = profile.TestPlan });
		}

		public void Handle(TestCaseTestPlanQueryResult message)
		{
			Data.AllTestCaseTestPlansCount = message.QueryResultCount;
			if (message.Dtos != null)
			{
				Data.TestCaseTestPlansRetrievedCount += message.Dtos.Length;
				foreach (var testCaseTestPlanDto in message.Dtos)
				{
					StorageRepository().Get<TestCaseTestPlanDTO>(testCaseTestPlanDto.TestCaseTestPlanID.ToString()).Add(testCaseTestPlanDto);
				}
			}
			CompleteSagaIfNecessary();
		}

		private void CompleteSagaIfNecessary()
		{
			if (Data.TestCaseTestPlansRetrievedCount == Data.AllTestCaseTestPlansCount)
				MarkAsComplete();
		}
	}

	public class TestRunImportPluginUpdatedProfileInitializationData : ISagaEntity
	{
		public Guid Id { get; set; }
		public string Originator { get; set; }
		public string OriginalMessageId { get; set; }

		public int AllTestCaseTestPlansCount { get; set; }
		public int TestCaseTestPlansRetrievedCount { get; set; }
	}
}