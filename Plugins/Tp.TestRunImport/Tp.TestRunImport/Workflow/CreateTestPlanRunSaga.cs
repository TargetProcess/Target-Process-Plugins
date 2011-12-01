// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using NServiceBus;
using NServiceBus.Saga;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Messages.EntityLifecycle.Queries;
using Tp.Integration.Messages.TargetProcessLifecycle;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.TestRunImport.Messages;
using Tp.Integration.Plugin.TestRunImport.TestCaseResolvers;
using Tp.Integration.Plugin.TestRunImport.TestRunImport;

namespace Tp.Integration.Plugin.TestRunImport.Workflow
{
	public class CreateTestPlanRunSaga : TpSaga<CreateTestPlanRunSagaData>,
	                                     IAmStartedByMessages<TestRunImportResultDetectedLocalMessage>,
	                                     IHandleMessages<TestPlanRunCreatedMessage>,
	                                     IHandleMessages<TestCaseRunUpdatedMessage>,
	                                     IHandleMessages<TargetProcessExceptionThrownMessage>,
	                                     IHandleMessages<TestCaseRunQueryResult>
	{
		private readonly ITestCaseResolverFactory _resolverFactory;
		private readonly IProfileReadonly _profile;

		public CreateTestPlanRunSaga()
		{
		}

		public CreateTestPlanRunSaga(IProfileReadonly profile, ITestCaseResolverFactory resolverFactory)
		{
			_resolverFactory = resolverFactory;
			_profile = profile;
		}

		public override void ConfigureHowToFindSaga()
		{
			ConfigureMapping<TestPlanRunCreatedMessage>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<TestCaseRunUpdatedMessage>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<TestCaseRunQueryResult>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<TargetProcessExceptionThrownMessage>(saga => saga.Id, message => message.SagaId);
		}

		public void Handle(TestRunImportResultDetectedLocalMessage message)
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

			StorageRepository().Get<TestRunImportResultInfo>().Clear();
			StorageRepository().Get<TestRunImportResultInfo>().AddRange(message.TestRunImportInfo.TestRunImportResults);

			var profile = _profile.GetProfile<TestRunImportPluginProfile>();
			var dateTimeNow = DateTime.Now;

			var dto = new TestPlanRunDTO
			          	{
			          		TestPlanID = profile.TestPlan,
			          		ProjectID = profile.Project,
			          		CreateDate = dateTimeNow,
			          		StartDate = dateTimeNow,
			          		EndDate = dateTimeNow,
			          		CommentOnChangingState = string.Format("State is changed by '{0}' plugin", _profile.Name.Value)
			          	};

			Send(new CreateEntityCommand<TestPlanRunDTO>(dto));
		}

		public void Handle(TestPlanRunCreatedMessage message)
		{
			if (message.Dto.TestPlanRunID.HasValue)
			{
				Log().InfoFormat("Test plan run #{0} - {1} created", message.Dto.TestPlanRunID.Value.ToString(), message.Dto.Name);

				Data.AllTestCaseRunsCount = int.MinValue;
				Data.TestPlanRunId = message.Dto.TestPlanRunID.Value;
				StorageRepository().Get<TestCaseRunDTO>().Clear();

				Log().InfoFormat("Start getting test case runs for test plan run #{0} - {1}", message.Dto.TestPlanRunID.Value.ToString(), message.Dto.Name);

				Send(new TestCaseRunQuery {TestPlanRunId = message.Dto.TestPlanRunID.Value});
			}
			else
			{
				Log().Error("Failed to create test plan run");

				CompleteSaga();
			}
		}

		public void Handle(TestCaseRunQueryResult message)
		{
			Data.AllTestCaseRunsCount = message.QueryResultCount;
			if (message.Dtos != null)
			{
				var testCaseRunDtos = message.Dtos.Where(x => x.TestPlanRunID == Data.TestPlanRunId).ToArray();
				Data.TestCaseRunsRetrievedCount += testCaseRunDtos.Length;
				StorageRepository().Get<TestCaseRunDTO>(Data.TestPlanRunId.ToString()).AddRange(testCaseRunDtos);
			}
			UpdateTestPlanRunsIfNecessary();
		}

		private void SendUpdateTestCaseRun(TestCaseRunDTO dto)
		{
			if (dto.Passed.HasValue && !dto.Passed.Value)
			{
				dto.Comment = string.Format("Result imported by '{0}' plugin", _profile.Name.Value);
			}
			Send(new UpdateCommand {Dto = dto});
		}

		public void Handle(TestCaseRunUpdatedMessage message)
		{
			if (message.Dto != null && message.Dto.TestPlanRunID == Data.TestPlanRunId)
			{
				Data.TestCaseRunsUpdatedCount++;
				var testCaseTestPlanDto =
					StorageRepository().Get<TestCaseTestPlanDTO>(message.Dto.TestCaseTestPlanID.ToString()).FirstOrDefault();

				if (testCaseTestPlanDto != null)
				{
					var changedFields = new Enum[]
					                    	{TestCaseField.LastFailureComment, TestCaseField.LastRunDate, TestCaseField.LastStatus};
					var testCaseDto = new TestCaseDTO
					                  	{
					                  		TestCaseID = testCaseTestPlanDto.TestCaseID,
					                  		LastFailureComment = message.Dto.Comment,
					                  		LastRunDate = message.Dto.RunDate,
					                  		LastStatus = message.Dto.Passed
					                  	};
					Send(new UpdateCommand
					     	{
					     		ChangedFields = changedFields.Select(x => x.ToString()).ToArray(),
					     		Dto = testCaseDto
					     	});
				}
			}
			if (Data.NumberOfTestCasesToUpdate == Data.TestCaseRunsUpdatedCount)
			{
				Log().Info("Finished updating test case runs with imported results");

				CompleteSaga();
			}
		}

		private void CompleteSaga()
		{
			Log().Info("Finished importing test results");

			StorageRepository().Get<TestCaseRunDTO>(Data.TestPlanRunId.ToString()).Clear();
			StorageRepository().Get<TestRunImportResultInfo>().Clear();
			MarkAsComplete();
		}

		private void UpdateTestPlanRunsIfNecessary()
		{
			if (Data.TestCaseRunsRetrievedCount != Data.AllTestCaseRunsCount)
				return;

			Log().InfoFormat("Finished getting test case runs for test plan run #{0}. The number of test case runs retrieved is {1}", Data.TestPlanRunId, Data.TestCaseRunsRetrievedCount);

			var testCasesFoundNames = new List<string>();
			var testCasesNotFoundNames = new List<string>();
			var profile = _profile.GetProfile<TestRunImportPluginProfile>();
			var resolver = _resolverFactory.GetResolver(profile, StorageRepository().Get<TestRunImportResultInfo>().ToArray(),
			                                            StorageRepository().Get<TestCaseTestPlanDTO>().ToArray());
			var result =
				resolver.ImportResultsForTestCaseRuns(
					StorageRepository().Get<TestCaseRunDTO>(Data.TestPlanRunId.ToString()).ToArray(), testCasesFoundNames.Add,
					testCasesNotFoundNames.Add);
			var affectedDtos = result.Where(r => r.Runned.HasValue && r.RunDate.HasValue).ToArray();
			Data.NumberOfTestCasesToUpdate = affectedDtos.Length;

			if (testCasesFoundNames.Count > 0)
			{
				Log().InfoFormat("Imported test cases: {0}", string.Join(" ,", testCasesFoundNames.ToArray()));
			}
			if (testCasesNotFoundNames.Count > 0)
			{
				Log().InfoFormat("Not imported test cases: {0}", string.Join(" ,", testCasesNotFoundNames.ToArray()));
			}

			Log().Info("Start updating test case runs with imported results");

			affectedDtos.ForEach(SendUpdateTestCaseRun);
		}

		public void Handle(TargetProcessExceptionThrownMessage message)
		{
			Log().Error(string.Format("Failed to complete NUnit results for TestPlanRun #{0} for plugin '{1}'", Data.TestPlanRunId,
			                  _profile.Name.Value), message.GetException());
			CompleteSaga();
		}
	}

	[Serializable]
	public class CreateTestPlanRunSagaData : ISagaEntity
	{
		public Guid Id { get; set; }
		public string Originator { get; set; }
		public string OriginalMessageId { get; set; }

		public int NumberOfTestCasesToUpdate { get; set; }
		public int TestCaseRunsUpdatedCount { get; set; }
		public int TestPlanRunId { get; set; }
		public int AllTestCaseRunsCount { get; set; }
		public int TestCaseRunsRetrievedCount { get; set; }
	}
}