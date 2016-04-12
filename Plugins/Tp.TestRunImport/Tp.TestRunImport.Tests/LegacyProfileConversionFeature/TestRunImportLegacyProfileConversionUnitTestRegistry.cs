// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Reflection;
using Tp.Integration.Plugin.TestRunImport;
using Tp.LegacyProfileConversion.Common.Testing;

namespace Tp.TestRunImport.Tests.LegacyProfileConversionFeature
{
	public class TestRunImportLegacyProfileConversionUnitTestRegistry : LegacyProfileConverterUnitTestRegistry
	{
		protected override Assembly PluginAssembly
		{
			get { return typeof(TestRunImportPluginProfile).Assembly; }
		}
	}
}