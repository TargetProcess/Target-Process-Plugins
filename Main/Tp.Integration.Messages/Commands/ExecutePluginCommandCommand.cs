// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using NServiceBus;

namespace Tp.Integration.Messages.Commands
{
	[Serializable]
	public class ExecutePluginCommandCommand : IMessage
	{
		public string CommandName { get; set; }
		public string Arguments { get; set; }
	}
}