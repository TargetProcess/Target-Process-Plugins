// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using Tp.Integration.Common;
using Tp.Integration.Messages.PluginLifecycle;

namespace Tp.Integration.Plugin.TestRunImport.Messages
{
	public class TestPlanRunCreatedLocalMessage : IPluginLocalMessage
	{
		public TestPlanRunDTO Dto { get; set; }
	}
}