// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

namespace Tp.Integration.Messages.ServiceBus.Transport
{
	public class PluginQueueFactory : IPluginQueueFactory
	{
		public IPluginQueue Create(string queueName)
		{
			return new PluginQueue(queueName);
		}
	}

	public interface IPluginQueueFactory
	{
		IPluginQueue Create(string queueName);
	}
}