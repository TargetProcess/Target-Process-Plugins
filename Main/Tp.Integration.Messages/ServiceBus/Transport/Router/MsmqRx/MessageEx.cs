using System;
using System.Messaging;

namespace Tp.Integration.Messages.ServiceBus.Transport.Router.MsmqRx
{
	public class MessageEx
	{
		public Message Message { get; set; }
		public MessageOrigin MessageOrigin { get; set; }
		public Action DoReceive { get; set; }
	}
}