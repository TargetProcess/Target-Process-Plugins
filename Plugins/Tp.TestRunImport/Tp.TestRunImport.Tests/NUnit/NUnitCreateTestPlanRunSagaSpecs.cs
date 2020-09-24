// 
// Copyright (c) 2005-2017 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NUnit.Framework;
using Tp.Integration.Plugin.TestRunImport.FrameworkTypes;
using Tp.Testing.Common.NBehave;

namespace Tp.TestRunImport.Tests.NUnit
{
    [TestFixture]
    [Category("PartPlugins1")]
    public class NUnitCreateTestPlanRunSagaSpecs : CreateTestPlanRunSagaSpecsBase
    {
        [Test]
        public void ShouldCreateTestPlanRunForProfileWithoutRegExpSpecified()
        {
            @"Given a list of test cases in test plan:
					|Name              |
					|Simple Test Case 1|
					|Simple Test Case 2|
					|Simple Test Case 3|
					|Simple Test Case 4|
				And projectId '11'
				And testPlanId '101'
				And test result file path is '\\NUnit\\SimpleTestCaseTestResult.xml'
				And passive mode is turned OFF
				And current plugin profile settings saved under name 'Profile_1'
			When plugin imports results
			Then local message TestRunImportResultDetectedLocalMessage with '3' results should be sent
				And TestPlanRunCreatedMessage with valid TestPlanRun should be sent from TargetProcess
				And TestCaseRunQuery with valid TestPlanRunId should be sent to TargetProcess
				And TestCaseRunQueryResult should be sent from TargetProcess for TestCaseRuns: Simple Test Case 1, Simple Test Case 2, Simple Test Case 3, Simple Test Case 4
				And UpdateCommand for TestCaseRunDTO should be sent to TargetProcess:
					|Name              |State  |Runned|
					|Simple Test Case 1|Passed |Yes   |
					|Simple Test Case 2|Failed |Yes   |
					|Simple Test Case 4|Ignored|Yes   |
				And TestCaseRunUpdatedMessage should be sent from TargetProcess for TestCaseRuns: Simple Test Case 1, Simple Test Case 2, Simple Test Case 4
				And UpdateCommand for TestCaseDTO should be sent to TargetProcess for TestCaseRuns: Simple Test Case 1, Simple Test Case 2, Simple Test Case 4
			"
                .Execute(In.Context<NUnitTestRunImportActionSteps>());
        }

        [Test]
        public void ShouldCreateNUnit30TestPlanRunForProfileWithoutRegExpSpecified()
        {
            @"Given a list of test cases in test plan:
					|Name              |
					|Simple Test Case 1|
					|Simple Test Case 2|
					|Simple Test Case 3|
					|Simple Test Case 4|
				And projectId '11'
				And testPlanId '101'
				And test result file path is '\\NUnit\\SimpleNUnit30TestCaseTestResult.xml'
				And passive mode is turned OFF
				And current plugin profile settings saved under name 'Profile_1'
			When plugin imports results
			Then local message TestRunImportResultDetectedLocalMessage with '3' results should be sent
				And TestPlanRunCreatedMessage with valid TestPlanRun should be sent from TargetProcess
				And TestCaseRunQuery with valid TestPlanRunId should be sent to TargetProcess
				And TestCaseRunQueryResult should be sent from TargetProcess for TestCaseRuns: Simple Test Case 1, Simple Test Case 2, Simple Test Case 3, Simple Test Case 4
				And UpdateCommand for TestCaseRunDTO should be sent to TargetProcess:
					|Name              |State  |Runned|
					|Simple Test Case 1|Passed |Yes   |
					|Simple Test Case 2|Failed |Yes   |
					|Simple Test Case 4|Ignored|Yes   |
				And TestCaseRunUpdatedMessage should be sent from TargetProcess for TestCaseRuns: Simple Test Case 1, Simple Test Case 2, Simple Test Case 4
				And UpdateCommand for TestCaseDTO should be sent to TargetProcess for TestCaseRuns: Simple Test Case 1, Simple Test Case 2, Simple Test Case 4
			"
                .Execute(In.Context<NUnitTestRunImportActionSteps>());
        }

        [Test]
        public void ShouldCreateTestPlanRunForProfileWithTestIdRegExpSpecified()
        {
            @"Given a list of test cases in test plan:
					|Name              |
					|Simple Test Case 1|
					|Simple Test Case 2|
					|Simple Test Case 3|
					|Simple Test Case 4|
				And projectId '11'
				And testPlanId '101'
				And test result file path is '\\NUnit\\TestIdRegExpTestCaseTestResult.xml'
				And passive mode is turned OFF
				And RegExp '_(?<testId>\d+)$'
				And current plugin profile settings saved under name 'Profile_1'
			When plugin imports results
			Then local message TestRunImportResultDetectedLocalMessage with '3' results should be sent
				And TestPlanRunCreatedMessage with valid TestPlanRun should be sent from TargetProcess
				And TestCaseRunQuery with valid TestPlanRunId should be sent to TargetProcess
				And TestCaseRunQueryResult should be sent from TargetProcess for TestCaseRuns: Simple Test Case 1, Simple Test Case 2, Simple Test Case 3, Simple Test Case 4
				And UpdateCommand for TestCaseRunDTO should be sent to TargetProcess:
					|Name              |State  |Runned|
					|Simple Test Case 1|Passed |Yes   |
					|Simple Test Case 2|Failed |Yes   |
					|Simple Test Case 4|Ignored|Yes   |
				And TestCaseRunUpdatedMessage should be sent from TargetProcess for TestCaseRuns: Simple Test Case 1, Simple Test Case 2, Simple Test Case 4
				And UpdateCommand for TestCaseDTO should be sent to TargetProcess for TestCaseRuns: Simple Test Case 1, Simple Test Case 2, Simple Test Case 4
			"
                .Execute(In.Context<NUnitTestRunImportActionSteps>());
        }

        [Test]
        public void ShouldCreateNUnit30TestPlanRunForProfileWithTestIdRegExpSpecified()
        {
            @"Given a list of test cases in test plan:
					|Name              |
					|Simple Test Case 1|
					|Simple Test Case 2|
					|Simple Test Case 3|
					|Simple Test Case 4|
				And projectId '11'
				And testPlanId '101'
				And test result file path is '\\NUnit\\TestIdRegExpNUnit30TestCaseTestResult.xml'
				And passive mode is turned OFF
				And RegExp '_(?<testId>\d+)$'
				And current plugin profile settings saved under name 'Profile_1'
			When plugin imports results
			Then local message TestRunImportResultDetectedLocalMessage with '3' results should be sent
				And TestPlanRunCreatedMessage with valid TestPlanRun should be sent from TargetProcess
				And TestCaseRunQuery with valid TestPlanRunId should be sent to TargetProcess
				And TestCaseRunQueryResult should be sent from TargetProcess for TestCaseRuns: Simple Test Case 1, Simple Test Case 2, Simple Test Case 3, Simple Test Case 4
				And UpdateCommand for TestCaseRunDTO should be sent to TargetProcess:
					|Name              |State  |Runned|
					|Simple Test Case 1|Passed |Yes   |
					|Simple Test Case 2|Failed |Yes   |
					|Simple Test Case 4|Ignored|Yes   |
				And TestCaseRunUpdatedMessage should be sent from TargetProcess for TestCaseRuns: Simple Test Case 1, Simple Test Case 2, Simple Test Case 4
				And UpdateCommand for TestCaseDTO should be sent to TargetProcess for TestCaseRuns: Simple Test Case 1, Simple Test Case 2, Simple Test Case 4
			"
                .Execute(In.Context<NUnitTestRunImportActionSteps>());
        }

        [Test]
        public void ShouldCreateTestPlanRunForProfileWithTestNameRegExpSpecified()
        {
            @"Given a list of test cases in test plan:
					|Name              |
					|Simple Test Case 1|
					|Simple Test Case 2|
					|Simple Test Case 3|
					|Simple Test Case 4|
				And projectId '11'
				And testPlanId '101'
				And test result file path is '\\NUnit\\TestNameRegExpTestCaseTestResult.xml'
				And passive mode is turned OFF
				And RegExp '^(?<testName>[^_]+)_Test$'
				And current plugin profile settings saved under name 'Profile_1'
			When plugin imports results
			Then local message TestRunImportResultDetectedLocalMessage with '3' results should be sent
				And TestPlanRunCreatedMessage with valid TestPlanRun should be sent from TargetProcess
				And TestCaseRunQuery with valid TestPlanRunId should be sent to TargetProcess
				And TestCaseRunQueryResult should be sent from TargetProcess for TestCaseRuns: Simple Test Case 1, Simple Test Case 2, Simple Test Case 3, Simple Test Case 4
				And UpdateCommand for TestCaseRunDTO should be sent to TargetProcess:
					|Name              |State  |Runned|
					|Simple Test Case 1|Passed |Yes   |
					|Simple Test Case 2|Failed |Yes   |
					|Simple Test Case 4|Ignored|Yes   |
				And TestCaseRunUpdatedMessage should be sent from TargetProcess for TestCaseRuns: Simple Test Case 1, Simple Test Case 2, Simple Test Case 4
				And UpdateCommand for TestCaseDTO should be sent to TargetProcess for TestCaseRuns: Simple Test Case 1, Simple Test Case 2, Simple Test Case 4
			"
                .Execute(In.Context<NUnitTestRunImportActionSteps>());
        }

        [Test]
        public void ShouldCreateTestPlanRunFromResultsOnHttpForProfileWithoutRegExpSpecified()
        {
            @"Given a list of test cases in test plan:
					|Name              |
					|Simple Test Case 1|
					|Simple Test Case 2|
					|Simple Test Case 3|
					|Simple Test Case 4|
				And projectId '11'
				And testPlanId '101'
				And test result file path is 'http://127.0.0.1:2123/SimpleTestCaseTestResult.xml'
				And passive mode is turned OFF
				And current plugin profile settings saved under name 'Profile_1'
			When plugin imports results
			Then local message TestRunImportResultDetectedLocalMessage with '3' results should be sent
				And TestPlanRunCreatedMessage with valid TestPlanRun should be sent from TargetProcess
				And TestCaseRunQuery with valid TestPlanRunId should be sent to TargetProcess
				And TestCaseRunQueryResult should be sent from TargetProcess for TestCaseRuns: Simple Test Case 1, Simple Test Case 2, Simple Test Case 3, Simple Test Case 4
				And UpdateCommand for TestCaseRunDTO should be sent to TargetProcess:
					|Name              |State  |Runned|
					|Simple Test Case 1|Passed |Yes   |
					|Simple Test Case 2|Failed |Yes   |
					|Simple Test Case 4|Ignored|Yes   |
				And TestCaseRunUpdatedMessage should be sent from TargetProcess for TestCaseRuns: Simple Test Case 1, Simple Test Case 2, Simple Test Case 4
				And UpdateCommand for TestCaseDTO should be sent to TargetProcess for TestCaseRuns: Simple Test Case 1, Simple Test Case 2, Simple Test Case 4
			"
                .Execute(In.Context<NUnitTestRunImportActionSteps>());
        }

        [Test]
        public void ShouldCreateTestPlanRunFromResultsOnHttpForProfileWithTestIdRegExpSpecified()
        {
            @"Given a list of test cases in test plan:
					|Name              |
					|Simple Test Case 1|
					|Simple Test Case 2|
					|Simple Test Case 3|
					|Simple Test Case 4|
				And projectId '11'
				And testPlanId '101'
				And test result file path is 'http://127.0.0.1:2123/TestIdRegExpTestCaseTestResult.xml'
				And passive mode is turned OFF
				And RegExp '_(?<testId>\d+)$'
				And current plugin profile settings saved under name 'Profile_1'
			When plugin imports results
			Then local message TestRunImportResultDetectedLocalMessage with '3' results should be sent
				And TestPlanRunCreatedMessage with valid TestPlanRun should be sent from TargetProcess
				And TestCaseRunQuery with valid TestPlanRunId should be sent to TargetProcess
				And TestCaseRunQueryResult should be sent from TargetProcess for TestCaseRuns: Simple Test Case 1, Simple Test Case 2, Simple Test Case 3, Simple Test Case 4
				And UpdateCommand for TestCaseRunDTO should be sent to TargetProcess:
					|Name              |State  |Runned|
					|Simple Test Case 1|Passed |Yes   |
					|Simple Test Case 2|Failed |Yes   |
					|Simple Test Case 4|Ignored|Yes   |
				And TestCaseRunUpdatedMessage should be sent from TargetProcess for TestCaseRuns: Simple Test Case 1, Simple Test Case 2, Simple Test Case 4
				And UpdateCommand for TestCaseDTO should be sent to TargetProcess for TestCaseRuns: Simple Test Case 1, Simple Test Case 2, Simple Test Case 4
			"
                .Execute(In.Context<NUnitTestRunImportActionSteps>());
        }

        [Test]
        public void ShouldCreateTestPlanRunFromResultsOnHttpForProfileWithTestNameRegExpSpecified()
        {
            @"Given a list of test cases in test plan:
					|Name              |
					|Simple Test Case 1|
					|Simple Test Case 2|
					|Simple Test Case 3|
					|Simple Test Case 4|
				And projectId '11'
				And testPlanId '101'
				And test result file path is 'http://127.0.0.1:2123/TestNameRegExpTestCaseTestResult.xml'
				And passive mode is turned OFF
				And RegExp '^(?<testName>[^_]+)_Test$'
				And current plugin profile settings saved under name 'Profile_1'
			When plugin imports results
			Then local message TestRunImportResultDetectedLocalMessage with '3' results should be sent
				And TestPlanRunCreatedMessage with valid TestPlanRun should be sent from TargetProcess
				And TestCaseRunQuery with valid TestPlanRunId should be sent to TargetProcess
				And TestCaseRunQueryResult should be sent from TargetProcess for TestCaseRuns: Simple Test Case 1, Simple Test Case 2, Simple Test Case 3, Simple Test Case 4
				And UpdateCommand for TestCaseRunDTO should be sent to TargetProcess:
					|Name              |State  |Runned|
					|Simple Test Case 1|Passed |Yes   |
					|Simple Test Case 2|Failed |Yes   |
					|Simple Test Case 4|Ignored|Yes   |
				And TestCaseRunUpdatedMessage should be sent from TargetProcess for TestCaseRuns: Simple Test Case 1, Simple Test Case 2, Simple Test Case 4
				And UpdateCommand for TestCaseDTO should be sent to TargetProcess for TestCaseRuns: Simple Test Case 1, Simple Test Case 2, Simple Test Case 4
			"
                .Execute(In.Context<NUnitTestRunImportActionSteps>());
        }

        [Test]
        public void ShouldCreateTestPlanRunFromResultsOnFtpForProfileWithoutRegExpSpecified()
        {
            @"Given a list of test cases in test plan:
					|Name              |
					|Simple Test Case 1|
					|Simple Test Case 2|
					|Simple Test Case 3|
					|Simple Test Case 4|
				And projectId '11'
				And testPlanId '101'
				And test result file path is 'ftp://anonymous:anonymous@127.0.0.1:2121/SimpleTestCaseTestResult.xml'
				And passive mode is turned OFF
				And current plugin profile settings saved under name 'Profile_1'
			When plugin imports results
			Then local message TestRunImportResultDetectedLocalMessage with '3' results should be sent
				And TestPlanRunCreatedMessage with valid TestPlanRun should be sent from TargetProcess
				And TestCaseRunQuery with valid TestPlanRunId should be sent to TargetProcess
				And TestCaseRunQueryResult should be sent from TargetProcess for TestCaseRuns: Simple Test Case 1, Simple Test Case 2, Simple Test Case 3, Simple Test Case 4
				And UpdateCommand for TestCaseRunDTO should be sent to TargetProcess:
					|Name              |State  |Runned|
					|Simple Test Case 1|Passed |Yes   |
					|Simple Test Case 2|Failed |Yes   |
					|Simple Test Case 4|Ignored|Yes   |
				And TestCaseRunUpdatedMessage should be sent from TargetProcess for TestCaseRuns: Simple Test Case 1, Simple Test Case 2, Simple Test Case 4
				And UpdateCommand for TestCaseDTO should be sent to TargetProcess for TestCaseRuns: Simple Test Case 1, Simple Test Case 2, Simple Test Case 4
			"
                .Execute(In.Context<NUnitTestRunImportActionSteps>());
        }

        [Test]
        public void ShouldCreateTestPlanRunFromResultsOnFtpForProfileWithTestIdRegExpSpecified()
        {
            @"Given a list of test cases in test plan:
					|Name              |
					|Simple Test Case 1|
					|Simple Test Case 2|
					|Simple Test Case 3|
					|Simple Test Case 4|
				And projectId '11'
				And testPlanId '101'
				And test result file path is 'ftp://anonymous:anonymous@127.0.0.1:2121/TestIdRegExpTestCaseTestResult.xml'
				And passive mode is turned OFF
				And RegExp '_(?<testId>\d+)$'
				And current plugin profile settings saved under name 'Profile_1'
			When plugin imports results
			Then local message TestRunImportResultDetectedLocalMessage with '3' results should be sent
				And TestPlanRunCreatedMessage with valid TestPlanRun should be sent from TargetProcess
				And TestCaseRunQuery with valid TestPlanRunId should be sent to TargetProcess
				And TestCaseRunQueryResult should be sent from TargetProcess for TestCaseRuns: Simple Test Case 1, Simple Test Case 2, Simple Test Case 3, Simple Test Case 4
				And UpdateCommand for TestCaseRunDTO should be sent to TargetProcess:
					|Name              |State  |Runned|
					|Simple Test Case 1|Passed |Yes   |
					|Simple Test Case 2|Failed |Yes   |
					|Simple Test Case 4|Ignored|Yes   |
				And TestCaseRunUpdatedMessage should be sent from TargetProcess for TestCaseRuns: Simple Test Case 1, Simple Test Case 2, Simple Test Case 4
				And UpdateCommand for TestCaseDTO should be sent to TargetProcess for TestCaseRuns: Simple Test Case 1, Simple Test Case 2, Simple Test Case 4
			"
                .Execute(In.Context<NUnitTestRunImportActionSteps>());
        }

        [Test]
        public void ShouldCreateTestPlanRunFromResultsOnFtpForProfileWithTestNameRegExpSpecified()
        {
            @"Given a list of test cases in test plan:
					|Name              |
					|Simple Test Case 1|
					|Simple Test Case 2|
					|Simple Test Case 3|
					|Simple Test Case 4|
				And projectId '11'
				And testPlanId '101'
				And test result file path is 'ftp://anonymous:anonymous@127.0.0.1:2121/TestNameRegExpTestCaseTestResult.xml'
				And passive mode is turned OFF
				And RegExp '^(?<testName>[^_]+)_Test$'
				And current plugin profile settings saved under name 'Profile_1'
			When plugin imports results
			Then local message TestRunImportResultDetectedLocalMessage with '3' results should be sent
				And TestPlanRunCreatedMessage with valid TestPlanRun should be sent from TargetProcess
				And TestCaseRunQuery with valid TestPlanRunId should be sent to TargetProcess
				And TestCaseRunQueryResult should be sent from TargetProcess for TestCaseRuns: Simple Test Case 1, Simple Test Case 2, Simple Test Case 3, Simple Test Case 4
				And UpdateCommand for TestCaseRunDTO should be sent to TargetProcess:
					|Name              |State  |Runned|
					|Simple Test Case 1|Passed |Yes   |
					|Simple Test Case 2|Failed |Yes   |
					|Simple Test Case 4|Ignored|Yes   |
				And TestCaseRunUpdatedMessage should be sent from TargetProcess for TestCaseRuns: Simple Test Case 1, Simple Test Case 2, Simple Test Case 4
				And UpdateCommand for TestCaseDTO should be sent to TargetProcess for TestCaseRuns: Simple Test Case 1, Simple Test Case 2, Simple Test Case 4
			"
                .Execute(In.Context<NUnitTestRunImportActionSteps>());
        }

        protected override FrameworkTypes FrameworkType => FrameworkTypes.NUnit;
    }
}
