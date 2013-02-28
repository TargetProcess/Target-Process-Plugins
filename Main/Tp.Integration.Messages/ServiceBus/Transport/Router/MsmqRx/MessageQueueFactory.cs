using System.Collections.Concurrent;
using System.Messaging;
using NServiceBus.Utils;

namespace Tp.Integration.Messages.ServiceBus.Transport.Router.MsmqRx
{
	public class MessageQueueFactory
	{
		private static readonly ConcurrentDictionary<string, object> CheckedQueues = new ConcurrentDictionary<string, object>();

		public static MessageQueue GetOrCreateMessageQueue(string name)
		{
			CheckedQueues.GetOrAdd(name, n =>
				{
					MsmqUtilities.CreateQueueIfNecessary(name);
					return null;
				});

			var q = new MessageQueue(MsmqUtilities.GetFullPath(name), false, true);
			var filter = new MessagePropertyFilter();
			filter.SetAll();
			q.MessageReadPropertyFilter = filter;
			return q;
		}
	}
}
