// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NServiceBus;
using Tp.Integration.Messages;
using Tp.Integration.Messages.EntityLifecycle;

namespace Tp.Integration.Plugin.Common.PluginLifecycle
{
	internal class PluginInitializer : PluginInfoSender, IWantCustomInitialization
	{
		public void Init()
		{
			SendInfoMessages();
			SendPluginStartedLocalMessage();
		}

		private void SendPluginStartedLocalMessage()
		{
			// This message has no handlers. It is sent to awake child queues consumers in msmq router.
			foreach (var account in AccountCollection)
			{
				Bus.SendLocalWithContext(new ProfileName(), account.Name, new PluginStartedLocalMessage());
				Bus.SendLocalUiWithContext(new ProfileName(), account.Name, new PluginStartedLocalMessage());
			}
		}
	}

	public class PluginStartedLocalMessage : ITargetProcessMessage
	{
	}
}