using System;

namespace Tp.Integration.Messages.ServiceBus.Transport.Router.Exceptions
{
	public class MessageConsumerException : ApplicationException
	{
		public MessageConsumerException()
		{
		}

		public MessageConsumerException(string message): base(message)
		{
		}

		public MessageConsumerException(string message, Exception innerException): base(message, innerException)
		{
		}
	}
}