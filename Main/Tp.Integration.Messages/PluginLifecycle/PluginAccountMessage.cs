// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

namespace Tp.Integration.Messages.PluginLifecycle
{
	public class PluginAccountMessageSerialized : IPluginLifecycleMessage
	{
		public string SerializedMessage { get; set; }

		public PluginAccount[] GetAccounts()
		{
			return string.IsNullOrEmpty(SerializedMessage) ? new PluginAccount[]{} : SerializedMessage.Deserialize<PluginAccount[]>();
		}
	}
}