using System.Messaging;
using NServiceBus.Utils;

namespace Tp.Integration.Messages.ServiceBus.Transport.Router.MsmqRx
{
	public class MessageQueueFactory
	{
		public static MessageQueue GetOrCreateMessageQueue(string name)
		{
			MsmqUtilities.CreateQueueIfNecessary(name);
			var q = new MessageQueue(MsmqUtilities.GetFullPath(name));
			var filter = new MessagePropertyFilter();
			filter.SetAll();
			q.MessageReadPropertyFilter = filter;
			return q;
		}
	}
}
