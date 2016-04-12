// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using System.Linq;
using NBehave.Narrator.Framework;
using Tp.Integration.Plugin.TestRunImport;
using Tp.Integration.Plugin.TestRunImport.FrameworkTypes;
using Tp.Integration.Plugin.TestRunImport.Streams;
using Tp.LegacyProfileConversion.Common.Testing;
using Tp.LegacyProfileConvertsion.Common;
using Tp.TestRunImport.LegacyProfileConversion;
using Tp.Testing.Common.NUnit;

namespace Tp.TestRunImport.Tests.LegacyProfileConversionFeature
{
	[ActionSteps]
	public abstract class TestRunImportLegacyProfileConverterActionSteps<T> :
		LegacyProfileConverterActionStepsBase<T, TestRunImportLegacyProfileConversionUnitTestRegistry, PluginProfile>
		where T : TestRunImportLegacyProfileConvertor
	{
		[Given("sync interval is $syncInterval")]
		public void SetSyncInterval(int syncInterval)
		{
			SetLegacyProfileValue("IntegrationInterval", syncInterval.ToString());
		}

		[Given("test plan is '$testPlanName'")]
		public void SetTestPlan(string testPlanName)
		{
			SetLegacyProfileValue("TestPlanID", Context.Generals.First(x => x.Name == testPlanName).GeneralID.ToString());
		}

		[Given("test result file path is '$testResultFilePath'")]
		public void SetTestResultFilePath(string testResultFilePath)
		{
			SetLegacyProfileValue("TestResultFilePathForXMLSerializer", testResultFilePath);
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
			SetLegacyProfileValue("PassiveMode", mode.ToString());
		}

		[Given("regular expression to find test names is '$regexp'")]
		public void SetRegExp(string regexp)
		{
			SetLegacyProfileValue("Regexp", regexp.Replace ("&", "&amp;").Replace("'", "&apos;").Replace ("\"", "&quot;").Replace ("<", "&lt;").Replace(">", "&gt;"));
		}

		[Given("last detected modification of test result file is '$modifyTime'")]
		public void SetLastModifyTimeUtc(DateTime modifyTime)
		{
			SetLegacyProfileValue("TestResultFileLastModifyTimeUtc", modifyTime.ToString());
		}

		[Then("test result file path should be '$testResultFilePath'")]
		public void TestResultsFilePathShouldBe(string testResultFilePath)
		{
			Profile.PostResultsToRemoteUrl.Should(Be.EqualTo(false), "Profile.PostResultsToRemoteUrl.Should(Be.EqualTo(false))");
			Profile.ResultsFilePath.Should(Be.EqualTo(testResultFilePath), "Profile.ResultsFilePath.Should(Be.EqualTo(testResultFilePath))");
		}

		[Then("sync interval should be $syncInterval")]
		public void SyncIntervalShouldBe(int syncInterval)
		{
			Profile.SynchronizationInterval.Should(Be.EqualTo(syncInterval), "Profile.SynchronizationInterval.Should(Be.EqualTo(syncInterval))");
		}

		[Then("project should be '$projectAbbr'")]
		public void ProjectShouldBe(string projectAbbr)
		{
			Project project = Context.Projects.First(x => x.Abbreviation == projectAbbr);
			Profile.Project.Should(Be.EqualTo(project.ProjectID), "Profile.Project.Should(Be.EqualTo(project.ProjectID))");
		}

		[Then("test plan should be '$testPlanName'")]
		public void CreateTestPlanForProject(string testPlanName)
		{
			TestPlan testPlan = Context.TestPlans.First(x => x.General.Name == testPlanName);
			Profile.TestPlan.Should(Be.EqualTo(testPlan.TestPlanID), "Profile.TestPlan.Should(Be.EqualTo(testPlan.TestPlanID))");
		}

		[Then("passive mode should be turned ON")]
		public void PassiveModeShouldBeOn()
		{
			PassiveModeShouldBe(true);
		}

		[Given("passive mode should be turned OFF")]
		public void PassiveModeShouldBeOff()
		{
			PassiveModeShouldBe(false);
		}

		private void PassiveModeShouldBe(bool mode)
		{
			Profile.PassiveMode.Should(Be.EqualTo(mode), "Profile.PassiveMode.Should(Be.EqualTo(mode))");
		}

		[Then("regular expression to find test names should be '$regexp'")]
		public void RegExpShouldBe(string regexp)
		{
			Profile.RegExp.Should(Be.EqualTo(regexp), "Profile.RegExp.Should(Be.EqualTo(regexp))");
		}

		[Then("correct test run import framework type should de detected")]
		public void CorrectTestRunImportFrameworkTypeShouldBeDetected()
		{
			Profile.FrameworkType.Should(Be.EqualTo(FrameworkType), "Profile.FrameworkType.Should(Be.EqualTo(FrameworkType))");
		}

		[Then("last detected modification of test result file in profile storage should be '$modifyTime'")]
		public void ProfileStorageShouldContainLastModifyTimeUtc(DateTime modifyTime)
		{
			var lastModifyResult = Account.Profiles.First().Get<LastModifyResult>().First();
			lastModifyResult.Should(Be.Not.Null, "lastModifyResult.Should(Be.Not.Null)");
			modifyTime.ToUniversalTime().Should(Be.EqualTo(new DateTime(lastModifyResult.ModifyTimeUtcsTicks)), "modifyTime.ToUniversalTime().Should(Be.EqualTo(new DateTime(lastModifyResult.ModifyTimeUtcsTicks)))");
		}

		protected static TestRunImportPluginProfile Profile
		{
			get { return Account.Profiles.First().GetProfile<TestRunImportPluginProfile>(); }
		}

		protected abstract FrameworkTypes FrameworkType { get; }
	}
}
