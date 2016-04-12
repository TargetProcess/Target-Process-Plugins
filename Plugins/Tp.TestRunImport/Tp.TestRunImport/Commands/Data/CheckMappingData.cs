// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// using Tp.Integration.Messages.PluginLifecycle;

using Tp.Integration.Messages.PluginLifecycle;

namespace Tp.Integration.Plugin.TestRunImport.Commands.Data
{
	public class CheckMappingData
	{
		public PluginProfileTypedDto<TestRunImportPluginProfile> Profile { get; set; }
		public TestCaseLightDto[] TestCases { get; set; }
	}
}