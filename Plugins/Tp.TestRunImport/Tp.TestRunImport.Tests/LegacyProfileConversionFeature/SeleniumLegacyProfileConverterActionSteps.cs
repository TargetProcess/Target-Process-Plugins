// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NBehave.Narrator.Framework;
using Tp.Integration.Plugin.TestRunImport.FrameworkTypes;
using Tp.TestRunImport.LegacyProfileConversion.LegacyProfileConvertors;
using Tp.Testing.Common.NUnit;

namespace Tp.TestRunImport.Tests.LegacyProfileConversionFeature
{
	[ActionSteps]
	public class SeleniumLegacyProfileConverterActionSteps :
		TestRunImportLegacyProfileConverterActionSteps<SeleniumTestRunImportLegacyProfileConvertor>
	{
		protected override FrameworkTypes FrameworkType
		{
			get { return FrameworkTypes.Selenium; }
		}

		protected override string SettingsXmlNode
		{
			get { return "SeleniumSettings"; }
		}

		protected override string PluginName
		{
			get { return "Automatic Selenium Test Run Import"; }
		}

		[Then("test run results should be posted to the remote Url")]
		public void ShouldPostResultsToRemoteUrl()
		{
			Profile.PostResultsToRemoteUrl.Should(Be.EqualTo(true));
			Profile.ResultsFilePath.Should(Be.EqualTo(string.Empty));
		}
	}
}