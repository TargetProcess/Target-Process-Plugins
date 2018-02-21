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

namespace Tp.Integration.Plugin.TestRunImport
{
    public class TestRunImportPluginProfileInitializationSaga
        :
            NewProfileInitializationSaga<TestRunImportPluginProfileInitializationSagaData>,
            IHandleMessages<TestCaseTestPlanQueryResult>
    {
        private readonly IStorageRepository _storageRepository;

        public TestRunImportPluginProfileInitializationSaga()
        {
        }

        public TestRunImportPluginProfileInitializationSaga(IStorageRepository storageRepository)
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
            {
                MarkAsComplete();
            }
        }
    }

    public class TestRunImportPluginProfileInitializationSagaData : ISagaEntity
    {
        public Guid Id { get; set; }
        public string Originator { get; set; }
        public string OriginalMessageId { get; set; }

        public int AllTestCaseTestPlansCount { get; set; }
        public int TestCaseTestPlansRetrievedCount { get; set; }
    }
}
