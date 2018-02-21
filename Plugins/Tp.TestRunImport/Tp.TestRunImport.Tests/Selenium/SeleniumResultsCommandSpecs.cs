// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NUnit.Framework;
using Tp.Testing.Common.NBehave;

namespace Tp.TestRunImport.Tests.Selenium
{
    [TestFixture]
    [Category("PartPlugins1")]
    public class SeleniumResultsCommandSpecs
    {
        [Test]
        public void ShouldImportSimpleSeleniumResults()
        {
            @"Given a list of test cases in test plan:
					|Name             |
					|Build Up         |
					|Bad Password     |
					|Login Patient    |
					|Full Intervention|
				And projectId '11'
				And testPlanId '101'
				And post results to remote Url
				And authenticaton user Id is '1'
				And results remote Url is 'http://localhost/TargetProcess/api/v1/Plugins/Test%20Run%20Import/Profiles/Profile_1/Commands/SeleniumResults?token=YWRtaW46OTRDRDg2Qzg1NjgzQUZDMzg3Qjg2QTVERTAxRTZEQzY='
				And current plugin profile settings saved under name 'Profile_1'
			When command SeleniumResults is sent to TargetProcess
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
        public void ShouldImportTestIdRegExpSeleniumResults()
        {
            @"Given a list of test cases in test plan:
					|Name             |
					|Build Up         |
					|Bad Password     |
					|Login Patient    |
					|Full Intervention|
				And projectId '11'
				And testPlanId '101'
				And post results to remote Url
				And RegExp '_(?<testId>\d+)$'
				And authenticaton user Id is '1'
				And results remote Url is 'http://localhost/TargetProcess/api/v1/Plugins/Test%20Run%20Import/Profiles/Profile_1/Commands/SeleniumResults?token=YWRtaW46OTRDRDg2Qzg1NjgzQUZDMzg3Qjg2QTVERTAxRTZEQzY='
				And current plugin profile settings saved under name 'Profile_1'
			When command SeleniumResults is sent to TargetProcess
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
        public void ShouldImportTestNameRegExpSeleniumResults()
        {
            @"Given a list of test cases in test plan:
					|Name             |
					|Build Up         |
					|Bad Password     |
					|Login Patient    |
					|Full Intervention|
				And projectId '11'
				And testPlanId '101'
				And post results to remote Url
				And RegExp '^(?<testName>[^_]+)_Test$'
				And authenticaton user Id is '1'
				And results remote Url is 'http://localhost/TargetProcess/api/v1/Plugins/Test%20Run%20Import/Profiles/Profile_1/Commands/SeleniumResults?token=YWRtaW46OTRDRDg2Qzg1NjgzQUZDMzg3Qjg2QTVERTAxRTZEQzY='
				And current plugin profile settings saved under name 'Profile_1'
			When command SeleniumResults is sent to TargetProcess
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
    }
}
