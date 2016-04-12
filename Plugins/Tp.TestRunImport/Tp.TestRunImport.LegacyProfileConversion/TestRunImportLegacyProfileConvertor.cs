// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Tp.Integration.Common;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.TestRunImport;
using Tp.Integration.Plugin.TestRunImport.FrameworkTypes;
using Tp.Integration.Plugin.TestRunImport.Streams;
using Tp.LegacyProfileConvertsion.Common;
using PluginProfile = Tp.LegacyProfileConvertsion.Common.PluginProfile;

namespace Tp.TestRunImport.LegacyProfileConversion
{
	public abstract class TestRunImportLegacyProfileConvertor : LegacyProfileConvertorBase<PluginProfile>
	{
		protected TestRunImportLegacyProfileConvertor(IConvertorArgs args, IAccountCollection accountCollection) : base(args, accountCollection)
		{
		}

		protected abstract FrameworkTypes FrameworkType { get; }
		protected abstract string SettingsXmlNode { get; }
		protected abstract string PluginName { get; }

		protected override void OnProfileMigrated(IStorageRepository storageRepository, PluginProfile legacyProfile)
		{
			if (TestResultFileLastModifyTimeUtc != DateTime.MinValue)
			{
				storageRepository.Get<LastModifyResult>().Add(new LastModifyResult { ModifyTimeUtcsTicks = TestResultFileLastModifyTimeUtc.Ticks });
			}

			var pluginProfile = storageRepository.GetProfile<TestRunImportPluginProfile>();
			var testPlan = _context.Generals.FirstOrDefault(g => g.GeneralID == pluginProfile.TestPlan);
			if (testPlan == null) return;
			foreach (var testCaseTestPlan in _context.TestCaseTestPlans.Where(x => x.TestPlanID == pluginProfile.TestPlan))
			{
				TestCaseTestPlan plan = testCaseTestPlan;
				var testCase = _context.Generals.FirstOrDefault(g => g.GeneralID == plan.TestCaseID);
				if (testCase == null) continue;
				storageRepository.Get<TestCaseTestPlanDTO>(testCaseTestPlan.TestCaseTestPlanID.ToString()).Add(new TestCaseTestPlanDTO
				{
					ID = testCaseTestPlan.TestCaseTestPlanID,
					TestCaseID = testCaseTestPlan.TestCaseID,
					TestCaseName = testCase.Name,
					TestCaseTestPlanID = testCaseTestPlan.TestCaseTestPlanID,
					TestPlanID = testPlan.GeneralID,
					TestPlanName = testPlan.Name
				});
			}
		}

		protected override IEnumerable<PluginProfile> GetLegacyProfiles()
		{
			return _context.PluginProfiles.Where(x => x.Active == true && x.PluginName == PluginName);
		}

		private static XmlDocument ConvertToXmlDocument(PluginProfile x)
		{
			var document = new XmlDocument();
			document.LoadXml(x.Settings);
			return document;
		}

		private TestRunImportPluginProfile Parse(XmlNode document)
		{
			var result = new TestRunImportPluginProfile {FrameworkType = FrameworkType};

			var root = document.SelectSingleNode(string.Format("./{0}", SettingsXmlNode));
			result.ResultsFilePath = GetValueByName(root, "TestResultFilePathForXMLSerializer");

			var syncIntervalValue = GetValueByName(root, "IntegrationInterval");
			if (!string.IsNullOrEmpty(syncIntervalValue))
			{
				var syncInterval = Int32.Parse(syncIntervalValue);
				if (syncInterval > 0)
				{
					result.SynchronizationInterval = syncInterval * 60;
				}
			}

			var passiveModeValue = GetValueByName(root, "PassiveMode");
			if (!string.IsNullOrEmpty(passiveModeValue))
			{
				result.PassiveMode = bool.Parse(passiveModeValue);
			}

			var testPlanIdValue = GetValueByName(root, "TestPlanID");
			if (!string.IsNullOrEmpty(testPlanIdValue))
			{
				var testPlanId = Int32.Parse(testPlanIdValue);
				var testPlan = _context.TestPlans.FirstOrDefault(x => x.TestPlanID == testPlanId);
				if (testPlan != null)
				{
					result.Project = testPlan.ProjectID.GetValueOrDefault();
					result.TestPlan = testPlan.TestPlanID;
				}
			}

			var regexpValue = GetValueByName(root, "Regexp");
			if (!string.IsNullOrEmpty(regexpValue))
			{
				result.RegExp = regexpValue;
			}

			var testResultFileLastModifyTimeUtcValue = GetValueByName(root, "TestResultFileLastModifyTimeUtc");
			if (!string.IsNullOrEmpty(testResultFileLastModifyTimeUtcValue))
			{
				TestResultFileLastModifyTimeUtc = DateTime.Parse(testResultFileLastModifyTimeUtcValue).ToUniversalTime();
			}

			return result;
		}

		protected virtual void OnBeforeProfileMigrate(TestRunImportPluginProfile profile) { }

		private DateTime TestResultFileLastModifyTimeUtc { get; set; }

		protected override PluginProfileDto ConvertToPluginProfile(PluginProfile legacyProfile)
		{
			var pluginProfileDto = new PluginProfileDto { Name = legacyProfile.ProfileName };

			var document = ConvertToXmlDocument(legacyProfile);
			var converted = Parse(document);
			OnBeforeProfileMigrate(converted);
			pluginProfileDto.Settings = converted;

			return pluginProfileDto;
		}

		private static string GetValueByName(XmlNode root, string pathtoproject)
		{
			var node = root.SelectSingleNode(pathtoproject);
			return node != null ? node.InnerText : string.Empty;
		}
	}
}
