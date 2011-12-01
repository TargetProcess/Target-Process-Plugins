using System.Messaging;

namespace Tp.Integration.Messages.ServiceBus.Transport.Router.MsmqRx
{
	public struct MessageEx
	{
		public Message Message { get; set; }
		public MessageOrigin MessageOrigin { get; set; }
	}
}