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
    public class JUnitLegacyProfileConverterActionSteps :
        TestRunImportLegacyProfileConverterActionSteps<JUnitTestRunImportLegacyProfileConvertor>
    {
        protected override FrameworkTypes FrameworkType
        {
            get { return FrameworkTypes.JUnit; }
        }

        protected override string SettingsXmlNode
        {
            get { return "JUnitSettings"; }
        }

        protected override string PluginName
        {
            get { return "Automatic JUnit Test Run Import"; }
        }
    }
}
