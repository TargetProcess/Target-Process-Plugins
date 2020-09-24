// 
// Copyright (c) 2005-2019 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NUnit.Framework;
using Tp.Integration.Plugin.TestRunImport.FrameworkTypes;
using Tp.Testing.Common.NBehave;

namespace Tp.TestRunImport.Tests.Selenium
{
    [TestFixture]
    [Category("PartPlugins1")]
    public class SeleniumCreateTestPlanRunSagaSpecs : CreateTestPlanRunSagaSpecsBase
    {
        [Test]
        public void ShouldCreateTestPlanRunForProfileWithoutRegExpSpecified()
        {
            @"Given a list of test cases in test plan:
					|Name             |
					|Build Up         |
					|Bad Password     |
					|Login Patient    |
					|Full Intervention|
				And projectId '11'
				And testPlanId '101'
				And test result file path is '\\Selenium\\SimpleSeleniumTestResult.txt'
				And passive mode is turned OFF
				And current plugin profile settings saved under name 'Profile_1'
			When plugin imports results
			Then local message TestRunImportResultDetectedLocalMessage with '5' results should be sent
				And TestPlanRunCreatedMessage with valid TestPlanRun should be sent from TargetProcess
				And TestCaseRunQuery with valid TestPlanRunId should be sent to TargetProcess
				And TestCaseRunQueryResult should be sent from TargetProcess for TestCaseRuns: Build Up, Bad Password, Login Patient, Full Intervention
				And UpdateCommand for TestCaseRunDTO should be sent to TargetProcess:
					|Name             |State |Runned|
					|Build Up         |Passed|Yes   |
					|Bad Password     |Passed|Yes   |
					|Login Patient    |Passed|Yes   |
					|Full Intervention|Failed|Yes   |
				And TestCaseRunUpdatedMessage should be sent from TargetProcess for TestCaseRuns: Build Up, Bad Password, Login Patient, Full Intervention
				And UpdateCommand for TestCaseDTO should be sent to TargetProcess for TestCaseRuns: Build Up, Bad Password, Login Patient, Full Intervention
			"
                .Execute(In.Context<SeleniumTestRunImportActionSteps>());
        }

        [Test]
        public void ShouldCreateTestPlanRunForProfileWithTestIdRegExpSpecified()
        {
            @"Given a list of test cases in test plan:
					|Name             |
					|Build Up         |
					|Bad Password     |
					|Login Patient    |
					|Full Intervention|
				And projectId '11'
				And testPlanId '101'
				And test result file path is '\\Selenium\\TestIdRegExpSeleniumTestResult.txt'
				And passive mode is turned OFF
				And RegExp '_(?<testId>\d+)$'
				And current plugin profile settings saved under name 'Profile_1'
			When plugin imports results
			Then local message TestRunImportResultDetectedLocalMessage with '5' results should be sent
				And TestPlanRunCreatedMessage with valid TestPlanRun should be sent from TargetProcess
				And TestCaseRunQuery with valid TestPlanRunId should be sent to TargetProcess
				And TestCaseRunQueryResult should be sent from TargetProcess for TestCaseRuns: Build Up, Bad Password, Login Patient, Full Intervention
				And UpdateCommand for TestCaseRunDTO should be sent to TargetProcess:
					|Name             |State |Runned|
					|Build Up         |Passed|Yes   |
					|Bad Password     |Passed|Yes   |
					|Login Patient    |Passed|Yes   |
					|Full Intervention|Failed|Yes   |
				And TestCaseRunUpdatedMessage should be sent from TargetProcess for TestCaseRuns: Build Up, Bad Password, Login Patient, Full Intervention
				And UpdateCommand for TestCaseDTO should be sent to TargetProcess for TestCaseRuns: Build Up, Bad Password, Login Patient, Full Intervention
			"
                .Execute(In.Context<SeleniumTestRunImportActionSteps>());
        }

        [Test]
        public void ShouldCreateTestPlanRunForProfileWithTestNameRegExpSpecified()
        {
            @"Given a list of test cases in test plan:
					|Name             |
					|Build Up         |
					|Bad Password     |
					|Login Patient    |
					|Full Intervention|
				And projectId '11'
				And testPlanId '101'
				And test result file path is '\\Selenium\\TestNameRegExpSeleniumTestResult.txt'
				And passive mode is turned OFF
				And RegExp '^(?<testName>[^_]+)_Test$'
				And current plugin profile settings saved under name 'Profile_1'
			When plugin imports results
			Then local message TestRunImportResultDetectedLocalMessage with '5' results should be sent
				And TestPlanRunCreatedMessage with valid TestPlanRun should be sent from TargetProcess
				And TestCaseRunQuery with valid TestPlanRunId should be sent to TargetProcess
				And TestCaseRunQueryResult should be sent from TargetProcess for TestCaseRuns: Build Up, Bad Password, Login Patient, Full Intervention
				And UpdateCommand for TestCaseRunDTO should be sent to TargetProcess:
					|Name             |State |Runned|
					|Build Up         |Passed|Yes   |
					|Bad Password     |Passed|Yes   |
					|Login Patient    |Passed|Yes   |
					|Full Intervention|Failed|Yes   |
				And TestCaseRunUpdatedMessage should be sent from TargetProcess for TestCaseRuns: Build Up, Bad Password, Login Patient, Full Intervention
				And UpdateCommand for TestCaseDTO should be sent to TargetProcess for TestCaseRuns: Build Up, Bad Password, Login Patient, Full Intervention
			"
                .Execute(In.Context<SeleniumTestRunImportActionSteps>());
        }

        [Test]
        public void ShouldCreateTestPlanRunFromResultsOnHttpForProfileWithoutRegExpSpecified()
        {
            @"Given a list of test cases in test plan:
					|Name             |
					|Build Up         |
					|Bad Password     |
					|Login Patient    |
					|Full Intervention|
				And projectId '11'
				And testPlanId '101'
				And test result file path is 'http://127.0.0.1:2123/SimpleSeleniumTestResult.txt'
				And passive mode is turned OFF
				And current plugin profile settings saved under name 'Profile_1'
			When plugin imports results
			Then local message TestRunImportResultDetectedLocalMessage with '5' results should be sent
				And TestPlanRunCreatedMessage with valid TestPlanRun should be sent from TargetProcess
				And TestCaseRunQuery with valid TestPlanRunId should be sent to TargetProcess
				And TestCaseRunQueryResult should be sent from TargetProcess for TestCaseRuns: Build Up, Bad Password, Login Patient, Full Intervention
				And UpdateCommand for TestCaseRunDTO should be sent to TargetProcess:
					|Name             |State |Runned|
					|Build Up         |Passed|Yes   |
					|Bad Password     |Passed|Yes   |
					|Login Patient    |Passed|Yes   |
					|Full Intervention|Failed|Yes   |
				And TestCaseRunUpdatedMessage should be sent from TargetProcess for TestCaseRuns: Build Up, Bad Password, Login Patient, Full Intervention
				And UpdateCommand for TestCaseDTO should be sent to TargetProcess for TestCaseRuns: Build Up, Bad Password, Login Patient, Full Intervention
			"
                .Execute(In.Context<SeleniumTestRunImportActionSteps>());
        }

        [Test]
        public void ShouldCreateTestPlanRunFromResultsOnHttpForProfileWithTestIdRegExpSpecified()
        {
            @"Given a list of test cases in test plan:
					|Name             |
					|Build Up         |
					|Bad Password     |
					|Login Patient    |
					|Full Intervention|
				And projectId '11'
				And testPlanId '101'
				And test result file path is 'http://127.0.0.1:2123/TestIdRegExpSeleniumTestResult.txt'
				And passive mode is turned OFF
				And RegExp '_(?<testId>\d+)$'
				And current plugin profile settings saved under name 'Profile_1'
			When plugin imports results
			Then local message TestRunImportResultDetectedLocalMessage with '5' results should be sent
				And TestPlanRunCreatedMessage with valid TestPlanRun should be sent from TargetProcess
				And TestCaseRunQuery with valid TestPlanRunId should be sent to TargetProcess
				And TestCaseRunQueryResult should be sent from TargetProcess for TestCaseRuns: Build Up, Bad Password, Login Patient, Full Intervention
				And UpdateCommand for TestCaseRunDTO should be sent to TargetProcess:
					|Name             |State |Runned|
					|Build Up         |Passed|Yes   |
					|Bad Password     |Passed|Yes   |
					|Login Patient    |Passed|Yes   |
					|Full Intervention|Failed|Yes   |
				And TestCaseRunUpdatedMessage should be sent from TargetProcess for TestCaseRuns: Build Up, Bad Password, Login Patient, Full Intervention
				And UpdateCommand for TestCaseDTO should be sent to TargetProcess for TestCaseRuns: Build Up, Bad Password, Login Patient, Full Intervention
			"
                .Execute(In.Context<SeleniumTestRunImportActionSteps>());
        }

        [Test]
        public void ShouldCreateTestPlanRunFromResultsOnHttpForProfileWithTestNameRegExpSpecified()
        {
            @"Given a list of test cases in test plan:
					|Name             |
					|Build Up         |
					|Bad Password     |
					|Login Patient    |
					|Full Intervention|
				And projectId '11'
				And testPlanId '101'
				And test result file path is 'http://127.0.0.1:2123/TestNameRegExpSeleniumTestResult.txt'
				And passive mode is turned OFF
				And RegExp '^(?<testName>[^_]+)_Test$'
				And current plugin profile settings saved under name 'Profile_1'
			When plugin imports results
			Then local message TestRunImportResultDetectedLocalMessage with '5' results should be sent
				And TestPlanRunCreatedMessage with valid TestPlanRun should be sent from TargetProcess
				And TestCaseRunQuery with valid TestPlanRunId should be sent to TargetProcess
				And TestCaseRunQueryResult should be sent from TargetProcess for TestCaseRuns: Build Up, Bad Password, Login Patient, Full Intervention
				And UpdateCommand for TestCaseRunDTO should be sent to TargetProcess:
					|Name             |State |Runned|
					|Build Up         |Passed|Yes   |
					|Bad Password     |Passed|Yes   |
					|Login Patient    |Passed|Yes   |
					|Full Intervention|Failed|Yes   |
				And TestCaseRunUpdatedMessage should be sent from TargetProcess for TestCaseRuns: Build Up, Bad Password, Login Patient, Full Intervention
				And UpdateCommand for TestCaseDTO should be sent to TargetProcess for TestCaseRuns: Build Up, Bad Password, Login Patient, Full Intervention
			"
                .Execute(In.Context<SeleniumTestRunImportActionSteps>());
        }

        [Test]
        public void ShouldCreateTestPlanRunFromResultsOnFtpForProfileWithoutRegExpSpecified()
        {
            @"Given a list of test cases in test plan:
					|Name             |
					|Build Up         |
					|Bad Password     |
					|Login Patient    |
					|Full Intervention|
				And projectId '11'
				And testPlanId '101'
				And test result file path is 'ftp://anonymous:anonymous@127.0.0.1:2121/SimpleSeleniumTestResult.txt'
				And passive mode is turned OFF
				And current plugin profile settings saved under name 'Profile_1'
			When plugin imports results
			Then local message TestRunImportResultDetectedLocalMessage with '5' results should be sent
				And TestPlanRunCreatedMessage with valid TestPlanRun should be sent from TargetProcess
				And TestCaseRunQuery with valid TestPlanRunId should be sent to TargetProcess
				And TestCaseRunQueryResult should be sent from TargetProcess for TestCaseRuns: Build Up, Bad Password, Login Patient, Full Intervention
				And UpdateCommand for TestCaseRunDTO should be sent to TargetProcess:
					|Name             |State |Runned|
					|Build Up         |Passed|Yes   |
					|Bad Password     |Passed|Yes   |
					|Login Patient    |Passed|Yes   |
					|Full Intervention|Failed|Yes   |
				And TestCaseRunUpdatedMessage should be sent from TargetProcess for TestCaseRuns: Build Up, Bad Password, Login Patient, Full Intervention
				And UpdateCommand for TestCaseDTO should be sent to TargetProcess for TestCaseRuns: Build Up, Bad Password, Login Patient, Full Intervention
			"
                .Execute(In.Context<SeleniumTestRunImportActionSteps>());
        }

        [Test]
        public void ShouldCreateTestPlanRunFromResultsOnFtpForProfileWithTestIdRegExpSpecified()
        {
            @"Given a list of test cases in test plan:
					|Name             |
					|Build Up         |
					|Bad Password     |
					|Login Patient    |
					|Full Intervention|
				And projectId '11'
				And testPlanId '101'
				And test result file path is 'ftp://anonymous:anonymous@127.0.0.1:2121/TestIdRegExpSeleniumTestResult.txt'
				And passive mode is turned OFF
				And RegExp '_(?<testId>\d+)$'
				And current plugin profile settings saved under name 'Profile_1'
			When plugin imports results
			Then local message TestRunImportResultDetectedLocalMessage with '5' results should be sent
				And TestPlanRunCreatedMessage with valid TestPlanRun should be sent from TargetProcess
				And TestCaseRunQuery with valid TestPlanRunId should be sent to TargetProcess
				And TestCaseRunQueryResult should be sent from TargetProcess for TestCaseRuns: Build Up, Bad Password, Login Patient, Full Intervention
				And UpdateCommand for TestCaseRunDTO should be sent to TargetProcess:
					|Name             |State |Runned|
					|Build Up         |Passed|Yes   |
					|Bad Password     |Passed|Yes   |
					|Login Patient    |Passed|Yes   |
					|Full Intervention|Failed|Yes   |
				And TestCaseRunUpdatedMessage should be sent from TargetProcess for TestCaseRuns: Build Up, Bad Password, Login Patient, Full Intervention
				And UpdateCommand for TestCaseDTO should be sent to TargetProcess for TestCaseRuns: Build Up, Bad Password, Login Patient, Full Intervention
			"
                .Execute(In.Context<SeleniumTestRunImportActionSteps>());
        }

        [Test]
        public void ShouldCreateTestPlanRunFromResultsOnFtpForProfileWithTestNameRegExpSpecified()
        {
            @"Given a list of test cases in test plan:
					|Name             |
					|Build Up         |
					|Bad Password     |
					|Login Patient    |
					|Full Intervention|
				And projectId '11'
				And testPlanId '101'
				And test result file path is 'ftp://anonymous:anonymous@127.0.0.1:2121/TestNameRegExpSeleniumTestResult.txt'
				And passive mode is turned OFF
				And RegExp '^(?<testName>[^_]+)_Test$'
				And current plugin profile settings saved under name 'Profile_1'
			When plugin imports results
			Then local message TestRunImportResultDetectedLocalMessage with '5' results should be sent
				And TestPlanRunCreatedMessage with valid TestPlanRun should be sent from TargetProcess
				And TestCaseRunQuery with valid TestPlanRunId should be sent to TargetProcess
				And TestCaseRunQueryResult should be sent from TargetProcess for TestCaseRuns: Build Up, Bad Password, Login Patient, Full Intervention
				And UpdateCommand for TestCaseRunDTO should be sent to TargetProcess:
					|Name             |State |Runned|
					|Build Up         |Passed|Yes   |
					|Bad Password     |Passed|Yes   |
					|Login Patient    |Passed|Yes   |
					|Full Intervention|Failed|Yes   |
				And TestCaseRunUpdatedMessage should be sent from TargetProcess for TestCaseRuns: Build Up, Bad Password, Login Patient, Full Intervention
				And UpdateCommand for TestCaseDTO should be sent to TargetProcess for TestCaseRuns: Build Up, Bad Password, Login Patient, Full Intervention
			"
                .Execute(In.Context<SeleniumTestRunImportActionSteps>());
        }

        [Test]
        public void ShouldCreateTestPlanRunForTestNGProfileWithoutRegExpSpecified()
        {
            @"Given a list of test cases in test plan:
					|Name             |
					|Build Up         |
					|Bad Password     |
					|Login Patient    |
					|Full Intervention|
				And projectId '11'
				And testPlanId '101'
				And test result file path is '\\Selenium\\SimpleSeleniumTestNGTestResult.xml'
				And passive mode is turned OFF
				And current plugin profile settings saved under name 'Profile_1'
			When plugin imports results
			Then local message TestRunImportResultDetectedLocalMessage with '5' results should be sent
				And TestPlanRunCreatedMessage with valid TestPlanRun should be sent from TargetProcess
				And TestCaseRunQuery with valid TestPlanRunId should be sent to TargetProcess
				And TestCaseRunQueryResult should be sent from TargetProcess for TestCaseRuns: Build Up, Bad Password, Login Patient, Full Intervention
				And UpdateCommand for TestCaseRunDTO should be sent to TargetProcess:
					|Name             |State |Runned|
					|Build Up         |Passed|Yes   |
					|Bad Password     |Passed|Yes   |
					|Login Patient    |Passed|Yes   |
					|Full Intervention|Failed|Yes   |
				And TestCaseRunUpdatedMessage should be sent from TargetProcess for TestCaseRuns: Build Up, Bad Password, Login Patient, Full Intervention
				And UpdateCommand for TestCaseDTO should be sent to TargetProcess for TestCaseRuns: Build Up, Bad Password, Login Patient, Full Intervention
			"
                .Execute(In.Context<SeleniumTestRunImportActionSteps>());
        }

        protected override FrameworkTypes FrameworkType => FrameworkTypes.Selenium;
    }
}
