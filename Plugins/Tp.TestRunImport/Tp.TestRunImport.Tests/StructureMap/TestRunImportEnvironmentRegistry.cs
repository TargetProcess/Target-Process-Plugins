// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Plugin.TestRunImport.StructureMap;
using Tp.TestRunImport.Tests.Context;

namespace Tp.TestRunImport.Tests.StructureMap
{
	public class TestRunImportEnvironmentRegistry : PluginRegistry
	{
		public TestRunImportEnvironmentRegistry()
		{
			For<TestRunImportPluginContext>().HybridHttpOrThreadLocalScoped().Use<TestRunImportPluginContext>();
		}
	}
}