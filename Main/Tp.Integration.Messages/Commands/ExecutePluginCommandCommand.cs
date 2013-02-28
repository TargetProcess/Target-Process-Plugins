// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using NServiceBus;
using Tp.Integration.Messages.PluginLifecycle;

namespace Tp.Integration.Messages.Commands
{
	using Tp.Integration.Messages.EntityLifecycle;

	[Serializable]
	public class ExecutePluginCommandCommand : IPluginLocalMessage
	{
		public string CommandName { get; set; }
		public string Arguments { get; set; }
	}
}