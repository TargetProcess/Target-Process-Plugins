// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NUnit.Framework;
using Tp.Integration.Plugin.TestRunImport.FrameworkTypes;
using Tp.Testing.Common.NBehave;

namespace Tp.TestRunImport.Tests.JUnit
{
	[TestFixture]
	public class JUnitCreateTestPlanRunSagaSpecs : CreateTestPlanRunSagaSpecsBase
	{
		[Test]
		public void ShouldCreateTestPlanRunForProfileWithoutRegExpSpecified()
		{
			@"Given a list of test cases in test plan:
					|Name          |
					|Test Multiply |
					|Test Substract|
					|Test Add      |
					|Test Divide   |
				And projectId '11'
				And testPlanId '101'
				And test result file path is '\\JUnit\\SimpleTestCaseTestResult.xml'
				And passive mode is turned OFF
				And current plugin profile settings saved under name 'Profile_1'
			When plugin imports results
			Then local message TestRunImportResultDetectedLocalMessage with '4' results should be sent
				And TestPlanRunCreatedMessage with valid TestPlanRun should be sent from TargetProcess
				And TestCaseRunQuery with valid TestPlanRunId should be sent to TargetProcess
				And TestCaseRunQueryResult should be sent from TargetProcess for TestCaseRuns: Test Multiply, Test Substract, Test Add, Test Divide
				And UpdateCommand for TestCaseRunDTO should be sent to TargetProcess:
					|Name          |State  |Runned|
					|Test Multiply |Failed |Yes   |
					|Test Substract|Passed |Yes   |
					|Test Add      |Passed |Yes   |
					|Test Divide   |Ignored|Yes   |
				And TestCaseRunUpdatedMessage should be sent from TargetProcess for TestCaseRuns: Test Multiply, Test Substract, Test Add, Test Divide
				And UpdateCommand for TestCaseDTO should be sent to TargetProcess for TestCaseRuns: Test Multiply, Test Substract, Test Add, Test Divide
			"
				.Execute(In.Context<JUnitTestRunImportActionSteps>());
		}

		[Test]
		public void ShouldCreateTestPlanRunForProfileWithTestIdRegExpSpecified()
		{
			@"Given a list of test cases in test plan:
					|Name          |
					|Test Multiply |
					|Test Substract|
					|Test Add      |
					|Test Divide   |
				And projectId '11'
				And testPlanId '101'
				And test result file path is '\\JUnit\\TestIdRegExpTestCaseTestResult.xml'
				And passive mode is turned OFF
				And RegExp '_(?<testId>\d+)$'
				And current plugin profile settings saved under name 'Profile_1'
			When plugin imports results
			Then local message TestRunImportResultDetectedLocalMessage with '4' results should be sent
				And TestPlanRunCreatedMessage with valid TestPlanRun should be sent from TargetProcess
				And TestCaseRunQuery with valid TestPlanRunId should be sent to TargetProcess
				And TestCaseRunQueryResult should be sent from TargetProcess for TestCaseRuns: Test Multiply, Test Substract, Test Add, Test Divide
				And UpdateCommand for TestCaseRunDTO should be sent to TargetProcess:
					|Name          |State  |Runned|
					|Test Multiply |Failed |Yes   |
					|Test Substract|Passed |Yes   |
					|Test Add      |Passed |Yes   |
					|Test Divide   |Ignored|Yes   |
				And TestCaseRunUpdatedMessage should be sent from TargetProcess for TestCaseRuns: Test Multiply, Test Substract, Test Add, Test Divide
				And UpdateCommand for TestCaseDTO should be sent to TargetProcess for TestCaseRuns: Test Multiply, Test Substract, Test Add, Test Divide
			"
				.Execute(In.Context<JUnitTestRunImportActionSteps>());
		}

		[Test]
		public void ShouldCreateTestPlanRunForProfileWithTestNameRegExpSpecified()
		{
			@"Given a list of test cases in test plan:
					|Name          |
					|Test Multiply |
					|Test Substract|
					|Test Add      |
					|Test Divide   |
				And projectId '11'
				And testPlanId '101'
				And test result file path is '\\JUnit\\TestNameRegExpTestCaseTestResult.xml'
				And passive mode is turned OFF
				And RegExp '^(?<testName>[^_]+)_Test$'
				And current plugin profile settings saved under name 'Profile_1'
			When plugin imports results
			Then local message TestRunImportResultDetectedLocalMessage with '4' results should be sent
				And TestPlanRunCreatedMessage with valid TestPlanRun should be sent from TargetProcess
				And TestCaseRunQuery with valid TestPlanRunId should be sent to TargetProcess
				And TestCaseRunQueryResult should be sent from TargetProcess for TestCaseRuns: Test Multiply, Test Substract, Test Add, Test Divide
				And UpdateCommand for TestCaseRunDTO should be sent to TargetProcess:
					|Name          |State  |Runned|
					|Test Multiply |Failed |Yes   |
					|Test Substract|Passed |Yes   |
					|Test Add      |Passed |Yes   |
					|Test Divide   |Ignored|Yes   |
				And TestCaseRunUpdatedMessage should be sent from TargetProcess for TestCaseRuns: Test Multiply, Test Substract, Test Add, Test Divide
				And UpdateCommand for TestCaseDTO should be sent to TargetProcess for TestCaseRuns: Test Multiply, Test Substract, Test Add, Test Divide
			"
				.Execute(In.Context<JUnitTestRunImportActionSteps>());
		}

		[Test]
		public void ShouldCreateTestPlanRunFromResultsOnHttpForProfileWithoutRegExpSpecified()
		{
			@"Given a list of test cases in test plan:
					|Name          |
					|Test Multiply |
					|Test Substract|
					|Test Add      |
					|Test Divide   |
				And projectId '11'
				And testPlanId '101'
				And test result file path is 'http://127.0.0.1:2123/SimpleTestCaseTestResult.xml'
				And passive mode is turned OFF
				And current plugin profile settings saved under name 'Profile_1'
			When plugin imports results
			Then local message TestRunImportResultDetectedLocalMessage with '4' results should be sent
				And TestPlanRunCreatedMessage with valid TestPlanRun should be sent from TargetProcess
				And TestCaseRunQuery with valid TestPlanRunId should be sent to TargetProcess
				And TestCaseRunQueryResult should be sent from TargetProcess for TestCaseRuns: Test Multiply, Test Substract, Test Add, Test Divide
				And UpdateCommand for TestCaseRunDTO should be sent to TargetProcess:
					|Name          |State  |Runned|
					|Test Multiply |Failed |Yes   |
					|Test Substract|Passed |Yes   |
					|Test Add      |Passed |Yes   |
					|Test Divide   |Ignored|Yes   |
				And TestCaseRunUpdatedMessage should be sent from TargetProcess for TestCaseRuns: Test Multiply, Test Substract, Test Add, Test Divide
				And UpdateCommand for TestCaseDTO should be sent to TargetProcess for TestCaseRuns: Test Multiply, Test Substract, Test Add, Test Divide
			"
				.Execute(In.Context<JUnitTestRunImportActionSteps>());
		}

		[Test]
		public void ShouldCreateTestPlanRunFromResultsOnHttpForProfileWithTestIdRegExpSpecified()
		{
			@"Given a list of test cases in test plan:
					|Name          |
					|Test Multiply |
					|Test Substract|
					|Test Add      |
					|Test Divide   |
				And projectId '11'
				And testPlanId '101'
				And test result file path is 'http://127.0.0.1:2123/TestIdRegExpTestCaseTestResult.xml'
				And passive mode is turned OFF
				And RegExp '_(?<testId>\d+)$'
				And current plugin profile settings saved under name 'Profile_1'
			When plugin imports results
			Then local message TestRunImportResultDetectedLocalMessage with '4' results should be sent
				And TestPlanRunCreatedMessage with valid TestPlanRun should be sent from TargetProcess
				And TestCaseRunQuery with valid TestPlanRunId should be sent to TargetProcess
				And TestCaseRunQueryResult should be sent from TargetProcess for TestCaseRuns: Test Multiply, Test Substract, Test Add, Test Divide
				And UpdateCommand for TestCaseRunDTO should be sent to TargetProcess:
					|Name          |State  |Runned|
					|Test Multiply |Failed |Yes   |
					|Test Substract|Passed |Yes   |
					|Test Add      |Passed |Yes   |
					|Test Divide   |Ignored|Yes   |
				And TestCaseRunUpdatedMessage should be sent from TargetProcess for TestCaseRuns: Test Multiply, Test Substract, Test Add, Test Divide
				And UpdateCommand for TestCaseDTO should be sent to TargetProcess for TestCaseRuns: Test Multiply, Test Substract, Test Add, Test Divide
			"
				.Execute(In.Context<JUnitTestRunImportActionSteps>());
		}

		[Test]
		public void ShouldCreateTestPlanRunFromResultsOnHttpForProfileWithTestNameRegExpSpecified()
		{
			@"Given a list of test cases in test plan:
					|Name          |
					|Test Multiply |
					|Test Substract|
					|Test Add      |
					|Test Divide   |
				And projectId '11'
				And testPlanId '101'
				And test result file path is 'http://127.0.0.1:2123/TestNameRegExpTestCaseTestResult.xml'
				And passive mode is turned OFF
				And RegExp '^(?<testName>[^_]+)_Test$'
				And current plugin profile settings saved under name 'Profile_1'
			When plugin imports results
			Then local message TestRunImportResultDetectedLocalMessage with '4' results should be sent
				And TestPlanRunCreatedMessage with valid TestPlanRun should be sent from TargetProcess
				And TestCaseRunQuery with valid TestPlanRunId should be sent to TargetProcess
				And TestCaseRunQueryResult should be sent from TargetProcess for TestCaseRuns: Test Multiply, Test Substract, Test Add, Test Divide
				And UpdateCommand for TestCaseRunDTO should be sent to TargetProcess:
					|Name          |State  |Runned|
					|Test Multiply |Failed |Yes   |
					|Test Substract|Passed |Yes   |
					|Test Add      |Passed |Yes   |
					|Test Divide   |Ignored|Yes   |
				And TestCaseRunUpdatedMessage should be sent from TargetProcess for TestCaseRuns: Test Multiply, Test Substract, Test Add, Test Divide
				And UpdateCommand for TestCaseDTO should be sent to TargetProcess for TestCaseRuns: Test Multiply, Test Substract, Test Add, Test Divide
			"
				.Execute(In.Context<JUnitTestRunImportActionSteps>());
		}

		[Test]
		public void ShouldCreateTestPlanRunFromResultsOnFtpForProfileWithoutRegExpSpecified()
		{
			@"Given a list of test cases in test plan:
					|Name          |
					|Test Multiply |
					|Test Substract|
					|Test Add      |
					|Test Divide   |
				And projectId '11'
				And testPlanId '101'
				And test result file path is 'ftp://anonymous:anonymous@127.0.0.1:2121/SimpleTestCaseTestResult.xml'
				And passive mode is turned OFF
				And current plugin profile settings saved under name 'Profile_1'
			When plugin imports results
			Then local message TestRunImportResultDetectedLocalMessage with '4' results should be sent
				And TestPlanRunCreatedMessage with valid TestPlanRun should be sent from TargetProcess
				And TestCaseRunQuery with valid TestPlanRunId should be sent to TargetProcess
				And TestCaseRunQueryResult should be sent from TargetProcess for TestCaseRuns: Test Multiply, Test Substract, Test Add, Test Divide
				And UpdateCommand for TestCaseRunDTO should be sent to TargetProcess:
					|Name          |State  |Runned|
					|Test Multiply |Failed |Yes   |
					|Test Substract|Passed |Yes   |
					|Test Add      |Passed |Yes   |
					|Test Divide   |Ignored|Yes   |
				And TestCaseRunUpdatedMessage should be sent from TargetProcess for TestCaseRuns: Test Multiply, Test Substract, Test Add, Test Divide
				And UpdateCommand for TestCaseDTO should be sent to TargetProcess for TestCaseRuns: Test Multiply, Test Substract, Test Add, Test Divide
			"
				.Execute(In.Context<JUnitTestRunImportActionSteps>());
		}

		[Test]
		public void ShouldCreateTestPlanRunFromResultsOnFtpForProfileWithTestIdRegExpSpecified()
		{
			@"Given a list of test cases in test plan:
					|Name          |
					|Test Multiply |
					|Test Substract|
					|Test Add      |
					|Test Divide   |
				And projectId '11'
				And testPlanId '101'
				And test result file path is 'ftp://anonymous:anonymous@127.0.0.1:2121/TestIdRegExpTestCaseTestResult.xml'
				And passive mode is turned OFF
				And RegExp '_(?<testId>\d+)$'
				And current plugin profile settings saved under name 'Profile_1'
			When plugin imports results
			Then local message TestRunImportResultDetectedLocalMessage with '4' results should be sent
				And TestPlanRunCreatedMessage with valid TestPlanRun should be sent from TargetProcess
				And TestCaseRunQuery with valid TestPlanRunId should be sent to TargetProcess
				And TestCaseRunQueryResult should be sent from TargetProcess for TestCaseRuns: Test Multiply, Test Substract, Test Add, Test Divide
				And UpdateCommand for TestCaseRunDTO should be sent to TargetProcess:
					|Name          |State  |Runned|
					|Test Multiply |Failed |Yes   |
					|Test Substract|Passed |Yes   |
					|Test Add      |Passed |Yes   |
					|Test Divide   |Ignored|Yes   |
				And TestCaseRunUpdatedMessage should be sent from TargetProcess for TestCaseRuns: Test Multiply, Test Substract, Test Add, Test Divide
				And UpdateCommand for TestCaseDTO should be sent to TargetProcess for TestCaseRuns: Test Multiply, Test Substract, Test Add, Test Divide
			"
				.Execute(In.Context<JUnitTestRunImportActionSteps>());
		}

		[Test]
		public void ShouldCreateTestPlanRunFromResultsOnFtpForProfileWithTestNameRegExpSpecified()
		{
			@"Given a list of test cases in test plan:
					|Name          |
					|Test Multiply |
					|Test Substract|
					|Test Add      |
					|Test Divide   |
				And projectId '11'
				And testPlanId '101'
				And test result file path is 'ftp://anonymous:anonymous@127.0.0.1:2121/TestNameRegExpTestCaseTestResult.xml'
				And passive mode is turned OFF
				And RegExp '^(?<testName>[^_]+)_Test$'
				And current plugin profile settings saved under name 'Profile_1'
			When plugin imports results
			Then local message TestRunImportResultDetectedLocalMessage with '4' results should be sent
				And TestPlanRunCreatedMessage with valid TestPlanRun should be sent from TargetProcess
				And TestCaseRunQuery with valid TestPlanRunId should be sent to TargetProcess
				And TestCaseRunQueryResult should be sent from TargetProcess for TestCaseRuns: Test Multiply, Test Substract, Test Add, Test Divide
				And UpdateCommand for TestCaseRunDTO should be sent to TargetProcess:
					|Name          |State  |Runned|
					|Test Multiply |Failed |Yes   |
					|Test Substract|Passed |Yes   |
					|Test Add      |Passed |Yes   |
					|Test Divide   |Ignored|Yes   |
				And TestCaseRunUpdatedMessage should be sent from TargetProcess for TestCaseRuns: Test Multiply, Test Substract, Test Add, Test Divide
				And UpdateCommand for TestCaseDTO should be sent to TargetProcess for TestCaseRuns: Test Multiply, Test Substract, Test Add, Test Divide
			"
				.Execute(In.Context<JUnitTestRunImportActionSteps>());
		}

		protected override FrameworkTypes FrameworkType
		{
			get { return FrameworkTypes.JUnit; }
		}
	}
}