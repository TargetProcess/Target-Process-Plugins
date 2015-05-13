// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NBehave.Narrator.Framework;
using NUnit.Framework;
using StructureMap;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Messages.EntityLifecycle.Queries;
using Tp.Integration.Messages.Ticker;
using Tp.Integration.Plugin.TestRunImport;
using Tp.Integration.Plugin.TestRunImport.FrameworkTypes;
using Tp.Integration.Plugin.TestRunImport.Messages;
using Tp.Integration.Plugin.TestRunImport.TestRunImport;
using Tp.Integration.Testing.Common;
using Tp.TestRunImport.Tests.Context;
using Tp.TestRunImport.Tests.StructureMap;
using Tp.Testing.Common.NUnit;

namespace Tp.TestRunImport.Tests
{
	[ActionSteps]
	public abstract class ImportResultsTestRunImportActionSteps
	{
		private const int CreatedTestPlanRunId = 100;
		private List<TestCaseTestPlanDTO> _testCaseTestPlanDtos;
		protected TestRunImportPluginProfile Settings { get; private set; }
		private readonly DirectoryInfo _dir =
			new DirectoryInfo(new Uri(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase)).AbsolutePath);

		[BeforeScenario]
		public void BeforeScenario()
		{
			ObjectFactory.Configure(
				x =>
				x.For<TransportMock>().Use(TransportMock.CreateWithoutStructureMapClear(typeof(TestRunImportPluginProfile).Assembly)));
			ObjectFactory.Configure(x => x.AddRegistry<TestRunImportEnvironmentRegistry>());
			Settings = new TestRunImportPluginProfile { FrameworkType = FrameworkType };
			_testCaseTestPlanDtos = new List<TestCaseTestPlanDTO>();
		}

		[AfterScenario]
		public virtual void OnAfterScenario()
		{
			Context.Transport.LocalQueue.Clear();
			Context.Transport.TpQueue.Clear();
		}

		[Given("a list of test cases in test plan:")]
		public void ListOfTestCasesForTestPlan(string name)
		{
			_testCaseTestPlanDtos.Add(new TestCaseTestPlanDTO
										{
											TestCaseID = 1001 + _testCaseTestPlanDtos.Count,
											TestCaseName = name.Trim(),
											TestCaseTestPlanID = 10001 + _testCaseTestPlanDtos.Count
										});
		}

		[Given("projectId '$projectId'")]
		public void GivenProjectId(int projectId)
		{
			Settings.Project = projectId;
		}

		[Given("testPlanId '$testPlanId'")]
		public void GivenTestPlanId(int testPlanId)
		{
			Settings.TestPlan = testPlanId;
		}

		[Given("sync interval is $syncInterval")]
		public void SetSyncInterval(int syncInterval)
		{
			Settings.TestPlan = syncInterval;
		}

		[Given("test result file path is '$testResultFilePath'")]
		public void SetTestResultFilePath(string testResultFilePath)
		{
			var uri = new Uri(testResultFilePath);
			Settings.ResultsFilePath = string.Compare(uri.Scheme, "file", StringComparison.InvariantCultureIgnoreCase) == 0
										? _dir.FullName + testResultFilePath
										: testResultFilePath;
		}

		[Given("RegExp '$regExp'")]
		public void GiventestPlanId(string regExp)
		{
			Settings.RegExp = regExp;
		}

		[Given("passive mode is turned ON")]
		public void TurnPassiveModeOn()
		{
			TurnPassiveMode(true);
		}

		[Given("passive mode is turned OFF")]
		public void TurnPassiveModeOff()
		{
			TurnPassiveMode(false);
		}

		private void TurnPassiveMode(bool mode)
		{
			Settings.PassiveMode = mode;
		}

		[Given("current plugin profile settings saved under name '$profileName'")]
		public void CurrentPluginProfileIsSaved(string profileName)
		{
			Context.Transport.On<TestCaseTestPlanQuery>(cmd => cmd.TestPlanId == 101).Reply(
				x =>
				_testCaseTestPlanDtos.Select(
					testCaseTestPlanDto =>
					new TestCaseTestPlanQueryResult { Dtos = new[] { testCaseTestPlanDto }, QueryResultCount = _testCaseTestPlanDtos.Count }).Cast<ISagaMessage>().
					ToArray());
			Context.AddProfile(profileName, Settings);
			MockCreateTestPlanRunSagaMessagesLifecycle();
		}

		private void MockCreateTestPlanRunSagaMessagesLifecycle()
		{
			Context.Transport.On<CreateCommand>(cmd => cmd.Dto is TestPlanRunDTO).Reply(
				cmd => new TestPlanRunCreatedMessage
						{
							Dto = new TestPlanRunDTO
									{
										Name = "Test Plun Run 1",
										TestPlanRunID = CreatedTestPlanRunId,
										TestPlanID = ((TestPlanRunDTO)cmd.Dto).TestPlanID,
										ProjectID = ((TestPlanRunDTO)cmd.Dto).ProjectID,
										CreateDate = ((TestPlanRunDTO)cmd.Dto).CreateDate,
										StartDate = ((TestPlanRunDTO)cmd.Dto).StartDate,
										EndDate = ((TestPlanRunDTO)cmd.Dto).EndDate,
										CommentOnChangingState = ((TestPlanRunDTO)cmd.Dto).CommentOnChangingState
									}
						});

			Context.Transport.On<TestCaseRunQuery>(cmd => cmd.TestPlanRunId == CreatedTestPlanRunId).Reply(
				x => _testCaseTestPlanDtos.Select((dto, i) => new TestCaseRunQueryResult
																{
																	Dtos = new[]
				                                              		       	{
				                                              		       		new TestCaseRunDTO
				                                              		       			{
				                                              		       				TestCaseRunID = i + 1,
				                                              		       				TestPlanRunID = CreatedTestPlanRunId,
				                                              		       				Passed = null,
				                                              		       				RunDate = null,
				                                              		       				Runned = false,
				                                              		       				TestCaseTestPlanID = dto.TestCaseTestPlanID,
				                                              		       				Status = TestCaseRunStatusDTO.NotRun
				                                              		       			}
				                                              		       	},
																	QueryResultCount = _testCaseTestPlanDtos.Count
																}).Cast<ISagaMessage>().ToArray());
			Context.Transport.On<UpdateCommand>(cmd => cmd.Dto is TestCaseRunDTO).Reply(
				x => new TestCaseRunUpdatedMessage { Dto = (TestCaseRunDTO)x.Dto });

			Context.Transport.TpQueue.Clear();
		}

		[Given("plugin imports results")]
		protected virtual void PluginImportsResults()
		{
			Context.Transport.HandleLocalMessage(Context.CurrentProfile, new TickMessage());
		}

		[Then("local message TestRunImportResultDetectedLocalMessage with '$resultsCount' results should be sent")]
		public void LocalMessageTestRunImportResultDetectedLocalMessageShouldBeSent(int resultsCount)
		{
			var resultDetectedLocalMessages = Context.Transport.LocalQueue.GetMessages<TestRunImportResultDetectedLocalMessage>();
			resultDetectedLocalMessages.Length.Should(Be.EqualTo(1));
			var detectedLocalMessage = resultDetectedLocalMessages[0];
			detectedLocalMessage.TestRunImportInfo.TestRunImportResults.Should(Be.Not.Null);
			detectedLocalMessage.TestRunImportInfo.TestRunImportResults.Length.Should(Be.EqualTo(resultsCount));
		}

		[Then("TestPlanRunCreatedMessage with valid TestPlanRun should be sent from TargetProcess")]
		public void TestPlanRunCreatedMessageShouldBeSentFromTargetProcess()
		{
			var settings = Context.CurrentProfile.GetProfile<TestRunImportSettings>();
			var testPlanRunsCreated = Context.Transport.LocalQueue.GetMessages<TestPlanRunCreatedMessage>();
			testPlanRunsCreated.Length.Should(Be.EqualTo(1));
			var testPlanRun = testPlanRunsCreated[0];
			testPlanRun.Dto.TestPlanRunID.Should(Be.EqualTo(CreatedTestPlanRunId));
			testPlanRun.Dto.CommentOnChangingState.Should(
				Be.EqualTo(string.Format("State is changed by '{0}' plugin", Context.CurrentProfile.Name.Value)));
			testPlanRun.Dto.ProjectID.Should(Be.EqualTo(settings.Project));
			testPlanRun.Dto.TestPlanID.Should(Be.EqualTo(settings.TestPlan));
		}

		[Then("TestCaseRunQuery with valid TestPlanRunId should be sent to TargetProcess")]
		public void TestCaseRunQueryMessageShouldBeSentToTargetProcess()
		{
			var testCaseRunQueries = Context.Transport.TpQueue.GetMessages<TestCaseRunQuery>();
			testCaseRunQueries.Length.Should(Be.EqualTo(1));
			var testCaseRunQuery = testCaseRunQueries[0];
			testCaseRunQuery.TestPlanRunId.Should(Be.EqualTo(CreatedTestPlanRunId));
		}

		[Then(@"TestCaseRunQueryResult should be sent from TargetProcess for TestCaseRuns: (?<testCaseNames>([^,]+,?\s*)+)")]
		public void TestCaseRunQueryResultMessageShouldBeSentFromTargetProcess(string[] testCaseNames)
		{
			var caseRunQueryResults = Context.Transport.LocalQueue.GetMessages<TestCaseRunQueryResult>();
			caseRunQueryResults.Length.Should(Be.EqualTo(testCaseNames.Length));
			foreach (var dto in testCaseNames.Select(FindTestCaseTestPlanDtoByName))
			{
				dto.Should(Be.Not.Null);
				caseRunQueryResults.FirstOrDefault(x => x.Dtos.First().TestCaseTestPlanID == dto.TestCaseTestPlanID).Should(
					Be.Not.Null);
			}
		}

		[Then("UpdateCommand for TestCaseRunDTO should be sent to TargetProcess:")]
		public void UpdateCommandMessageForTestCaseRunDtoShouldBeSentToTargetProcess(string name, string state, string runned)
		{
			var updatedTestCaseRuns =
				Context.Transport.TpQueue.GetMessages<UpdateCommand>().Where(x => x.Dto is TestCaseRunDTO).ToArray();
			var testCaseTestPlanDto = FindTestCaseTestPlanDtoByName(name.Trim());
			testCaseTestPlanDto.Should(Be.Not.Null);
			var updatedDto =
				updatedTestCaseRuns.FirstOrDefault(
					x => ((TestCaseRunDTO)(x.Dto)).TestCaseTestPlanID == testCaseTestPlanDto.TestCaseTestPlanID);
			updatedDto.Should(Be.Not.Null);
			bool? passed = (string.IsNullOrEmpty(state) || string.IsNullOrEmpty(state.Trim()) ||
							string.Compare(state.Trim(), "IGNORED", StringComparison.InvariantCultureIgnoreCase) == 0)
							? (bool?)null
							: string.Compare(state.Trim(), "PASSED", StringComparison.InvariantCultureIgnoreCase) == 0;
			((TestCaseRunDTO)(updatedDto.Dto)).Passed.Should(Be.EqualTo(passed));
			TestCaseRunStatusDTO status;
			if (passed == null)
			{
				status = TestCaseRunStatusDTO.NotRun;
			}
			else
			{
				status = passed == true ? TestCaseRunStatusDTO.Passed : TestCaseRunStatusDTO.Failed;
			}
			((TestCaseRunDTO) (updatedDto.Dto)).Status.Should(Is.EqualTo(status));
			bool? executed = string.IsNullOrEmpty(runned) || string.IsNullOrEmpty(runned.Trim())
								? (bool?)null
								: string.Compare(runned.Trim(), "YES", StringComparison.InvariantCultureIgnoreCase) == 0;
			((TestCaseRunDTO)(updatedDto.Dto)).Runned.Should(Be.EqualTo(executed));
		}

		[Then(@"TestCaseRunUpdatedMessage should be sent from TargetProcess for TestCaseRuns: (?<testCaseNames>([^,]+,?\s*)+)"
			)]
		public void TestCaseRunUpdatedMessageShouldBeSentFromTargetProcess(string[] testCaseNames)
		{
			var caseRunUpdatedMessages = Context.Transport.LocalQueue.GetMessages<TestCaseRunUpdatedMessage>();
			caseRunUpdatedMessages.Length.Should(Be.EqualTo(testCaseNames.Length));
			foreach (var dto in testCaseNames.Select(FindTestCaseTestPlanDtoByName))
			{
				dto.Should(Be.Not.Null);
				caseRunUpdatedMessages.FirstOrDefault(x => x.Dto.TestCaseTestPlanID == dto.TestCaseTestPlanID).Should(Be.Not.Null);
			}
		}

		[Then(
			@"UpdateCommand for TestCaseDTO should be sent to TargetProcess for TestCaseRuns: (?<testCaseNames>([^,]+,?\s*)+)")]
		public void UpdateCommandMessageForTestCaseDtoShouldBeSentFromTargetProcess(string[] testCaseNames)
		{
			var updateTestCaseCommands =
				Context.Transport.TpQueue.GetMessages<UpdateCommand>().Where(x => x.Dto is TestCaseDTO).ToArray();
			updateTestCaseCommands.Length.Should(Be.EqualTo(testCaseNames.Length));
			foreach (var dto in testCaseNames.Select(FindTestCaseTestPlanDtoByName))
			{
				dto.Should(Be.Not.Null);
				updateTestCaseCommands.FirstOrDefault(x => x.Dto.ID == dto.TestCaseID).Should(Be.Not.Null);
			}
		}

		private TestCaseTestPlanDTO FindTestCaseTestPlanDtoByName(string name)
		{
			return
				_testCaseTestPlanDtos.FirstOrDefault(
					x => string.Compare(x.TestCaseName, name, StringComparison.InvariantCulture) == 0);
		}

		public TestRunImportPluginContext Context
		{
			get { return ObjectFactory.GetInstance<TestRunImportPluginContext>(); }
		}

		protected virtual string TypeResourceLocation
		{
			get
			{
				var fullName = GetType().FullName;
				return fullName != null
						? fullName.Replace(string.Format(".{0}", GetType().Name), string.Empty)
						: string.Empty;
			}
		}

		protected abstract FrameworkTypes FrameworkType { get; }
	}
}