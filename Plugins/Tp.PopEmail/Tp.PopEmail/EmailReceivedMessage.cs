// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.PopEmailIntegration.Data;

namespace Tp.PopEmailIntegration
{
	[Serializable]
	public class EmailReceivedMessage : IPluginLocalMessage
	{
		public EmailMessage Mail { get; set; }
	}
}