// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.TestRunImport;
using Tp.Integration.Plugin.TestRunImport.FrameworkTypes;
using Tp.LegacyProfileConvertsion.Common;

namespace Tp.TestRunImport.LegacyProfileConversion.LegacyProfileConvertors
{
	public class SeleniumTestRunImportLegacyProfileConvertor : TestRunImportLegacyProfileConvertor
	{
		public SeleniumTestRunImportLegacyProfileConvertor(IConvertorArgs args, IAccountCollection accountCollection) : base(args, accountCollection)
		{
		}

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

		protected override void OnBeforeProfileMigrate(TestRunImportPluginProfile profile)
		{
			base.OnBeforeProfileMigrate(profile);

			if (string.IsNullOrEmpty(profile.ResultsFilePath))
			{
				profile.PostResultsToRemoteUrl = true;
			}
		}
	}
}