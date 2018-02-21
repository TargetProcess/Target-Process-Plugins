// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.TestRunImport.FrameworkTypes;
using Tp.LegacyProfileConvertsion.Common;

namespace Tp.TestRunImport.LegacyProfileConversion.LegacyProfileConvertors
{
    public class JUnitTestRunImportLegacyProfileConvertor : TestRunImportLegacyProfileConvertor
    {
        public JUnitTestRunImportLegacyProfileConvertor(IConvertorArgs args, IAccountCollection accountCollection)
            : base(args, accountCollection)
        {
        }

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
