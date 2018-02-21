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
