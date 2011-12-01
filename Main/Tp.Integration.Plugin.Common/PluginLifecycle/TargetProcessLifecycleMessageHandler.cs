// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NServiceBus;
using Tp.Integration.Messages.TargetProcessLifecycle;

namespace Tp.Integration.Plugin.Common.PluginLifecycle
{
	public class TargetProcessLifecycleMessageHandler : PluginInfoSender, IHandleMessages<TargetProcessStartedMessage>
	{
		public void Handle(TargetProcessStartedMessage message)
		{
			SendInfoMessages();
		}
	}
}