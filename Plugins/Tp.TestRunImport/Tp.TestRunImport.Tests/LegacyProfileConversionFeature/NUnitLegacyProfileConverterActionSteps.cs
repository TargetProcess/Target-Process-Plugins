// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NBehave.Narrator.Framework;
using Tp.Integration.Plugin.TestRunImport.FrameworkTypes;
using Tp.TestRunImport.LegacyProfileConversion.LegacyProfileConvertors;

namespace Tp.TestRunImport.Tests.LegacyProfileConversionFeature
{
	[ActionSteps]
	public class NUnitLegacyProfileConverterActionSteps : TestRunImportLegacyProfileConverterActionSteps<NUnitTestRunImportLegacyProfileConvertor>
	{
		protected override FrameworkTypes FrameworkType
		{
			get { return FrameworkTypes.NUnit; }
		}

		protected override string SettingsXmlNode
		{
			get { return "NUnitSettings"; }
		}

		protected override string PluginName
		{
			get { return "Automatic NUnit Test Run Import"; }
		}
	}
}