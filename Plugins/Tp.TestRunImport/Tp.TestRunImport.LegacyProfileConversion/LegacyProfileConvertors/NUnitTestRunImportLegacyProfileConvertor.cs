// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.TestRunImport.FrameworkTypes;
using Tp.LegacyProfileConvertsion.Common;

namespace Tp.TestRunImport.LegacyProfileConversion.LegacyProfileConvertors
{
    public class NUnitTestRunImportLegacyProfileConvertor : TestRunImportLegacyProfileConvertor
    {
        public NUnitTestRunImportLegacyProfileConvertor(IConvertorArgs args, IAccountCollection accountCollection)
            : base(args, accountCollection)
        {
        }

        protected override FrameworkTypes FrameworkType => FrameworkTypes.NUnit;

        protected override string SettingsXmlNode => "NUnitSettings";

        protected override string PluginName => "Automatic NUnit Test Run Import";
    }
}
