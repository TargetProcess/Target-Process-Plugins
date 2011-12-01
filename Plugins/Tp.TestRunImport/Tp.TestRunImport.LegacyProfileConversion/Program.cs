// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using Tp.LegacyProfileConvertsion.Common;
using Tp.TestRunImport.LegacyProfileConversion.LegacyProfileConvertors;

namespace Tp.TestRunImport.LegacyProfileConversion
{
	public class Program
	{
		public static void Main(string[] args)
		{
			new LegacyConvertionRunner<NUnitTestRunImportLegacyProfileConvertor, PluginProfile>().Execute(args);
			new LegacyConvertionRunner<JUnitTestRunImportLegacyProfileConvertor, PluginProfile>().Execute(args);
			new LegacyConvertionRunner<SeleniumTestRunImportLegacyProfileConvertor, PluginProfile>().Execute(args);
		}
	}
}
