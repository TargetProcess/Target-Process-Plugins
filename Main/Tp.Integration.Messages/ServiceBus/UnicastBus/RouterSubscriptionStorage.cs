// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

namespace Tp.Integration.Messages.ServiceBus
{
	using System.Collections.Generic;

	using NServiceBus.Unicast.Subscriptions;

	public class RouterSubscriptionStorage
		: ISubscriptionStorage
	{
		public string ProxyQueue { get; set; }

		public void Subscribe(string client, IList<string> messageTypes)
		{
		}

		public void Unsubscribe(string client, IList<string> messageTypes)
		{
		}

		public IList<string> GetSubscribersForMessage(IList<string> messageTypes)
		{
			return new List<string> { this.ProxyQueue };
		}

		public void Init()
		{
		}
	}
}
