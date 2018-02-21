// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NUnit.Framework;
using Tp.Testing.Common.NBehave;

namespace Tp.TestRunImport.Tests.LegacyProfileConversionFeature
{
    [TestFixture]
    [Category("PartPlugins1")]
    public class WhenThereIsAnOldTestRunImportPluginProfileSpecs
    {
        [Test]
        public void ShouldConvertAllNUnitPluginProfilesToNewPlugin()
        {
            @"Given account name is 'Account'
					And profile name is 'ProfileName'
					And project 'Pr' created
					And test plan 'TestPlan' for project 'Pr' created
					And sync interval is 1
					And test plan is 'TestPlan'
					And test result file path is 'C:\\SimpleTestCaseTestResult.xml'
					And passive mode is turned OFF
					And regular expression to find test names is '_(?<testId>\d+)$'
					And last detected modification of test result file is '2011-08-11T12:41:54Z'
				When legacy plugin profile from Target Process converted to new plugin profile
				Then plugin 'Test Run Import' should have account 'Account'
					And 'ProfileName' plugin profile should be created
					And correct test run import framework type should de detected
					And project should be 'Pr'
					And test plan should be 'TestPlan'
					And sync interval should be 60
					And test result file path should be 'C:\\SimpleTestCaseTestResult.xml'
					And passive mode should be turned OFF
					And regular expression to find test names should be '_(?<testId>\d+)$'
					And last detected modification of test result file in profile storage should be '2011-08-11T12:41:54Z'"
                .Execute(In.Context<NUnitLegacyProfileConverterActionSteps>());
        }

        [Test]
        public void ShouldConvertAllJUnitPluginProfilesToNewPlugin()
        {
            @"Given account name is 'Account'
					And profile name is 'ProfileName'
					And project 'Pr' created
					And test plan 'TestPlan' for project 'Pr' created
					And sync interval is 1
					And test plan is 'TestPlan'
					And test result file path is 'C:\\SimpleTestCaseTestResult.xml'
					And passive mode is turned OFF
					And regular expression to find test names is '_(?<testId>\d+)$'
					And last detected modification of test result file is '2011-08-11T12:41:54Z'
				When legacy plugin profile from Target Process converted to new plugin profile
				Then plugin 'Test Run Import' should have account 'Account'
					And 'ProfileName' plugin profile should be created
					And correct test run import framework type should de detected
					And project should be 'Pr'
					And test plan should be 'TestPlan'
					And sync interval should be 60
					And test result file path should be 'C:\\SimpleTestCaseTestResult.xml'
					And passive mode should be turned OFF
					And regular expression to find test names should be '_(?<testId>\d+)$'
					And last detected modification of test result file in profile storage should be '2011-08-11T12:41:54Z'"
                .Execute(In.Context<JUnitLegacyProfileConverterActionSteps>());
        }

        [Test]
        public void ShouldConvertAllSeleniumPluginProfilesToNewPluginWhenResultFilePathSpecified()
        {
            @"Given account name is 'Account'
					And profile name is 'ProfileName'
					And project 'Pr' created
					And test plan 'TestPlan' for project 'Pr' created
					And sync interval is 1
					And test plan is 'TestPlan'
					And test result file path is 'C:\\SimpleTestCaseTestResult.xml'
					And passive mode is turned ON
					And regular expression to find test names is '_(?<testId>\d+)$'
					And last detected modification of test result file is '2011-08-11T12:41:54Z'
				When legacy plugin profile from Target Process converted to new plugin profile
				Then plugin 'Test Run Import' should have account 'Account'
					And 'ProfileName' plugin profile should be created
					And correct test run import framework type should de detected
					And project should be 'Pr'
					And test plan should be 'TestPlan'
					And sync interval should be 60
					And test result file path should be 'C:\\SimpleTestCaseTestResult.xml'
					And passive mode should be turned ON
					And regular expression to find test names should be '_(?<testId>\d+)$'
					And last detected modification of test result file in profile storage should be '2011-08-11T12:41:54Z'"
                .Execute(In.Context<SeleniumLegacyProfileConverterActionSteps>());
        }

        [Test]
        public void ShouldConvertAllSeleniumPluginProfilesToNewPluginWhenResultFilePathNotSpecified()
        {
            @"Given account name is 'Account'
					And profile name is 'ProfileName'
					And project 'Pr' created
					And test plan 'TestPlan' for project 'Pr' created
					And sync interval is 1
					And test plan is 'TestPlan'
					And passive mode is turned ON
					And regular expression to find test names is '_(?<testId>\d+)$'
					And last detected modification of test result file is '2011-08-11T12:41:54Z'
				When legacy plugin profile from Target Process converted to new plugin profile
				Then plugin 'Test Run Import' should have account 'Account'
					And 'ProfileName' plugin profile should be created
					And correct test run import framework type should de detected
					And project should be 'Pr'
					And test plan should be 'TestPlan'
					And sync interval should be 60
					And test run results should be posted to the remote Url
					And passive mode should be turned ON
					And regular expression to find test names should be '_(?<testId>\d+)$'
					And last detected modification of test result file in profile storage should be '2011-08-11T12:41:54Z'"
                .Execute(In.Context<SeleniumLegacyProfileConverterActionSteps>());
        }
    }
}
