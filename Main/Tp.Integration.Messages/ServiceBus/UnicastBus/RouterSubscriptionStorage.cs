using System.Collections.Generic;
using NServiceBus.Unicast.Subscriptions;

namespace Tp.Integration.Messages.ServiceBus
{
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
