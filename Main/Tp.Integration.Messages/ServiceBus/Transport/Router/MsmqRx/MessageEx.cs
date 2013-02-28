using System;
using System.Messaging;

namespace Tp.Integration.Messages.ServiceBus.Transport.Router.MsmqRx
{
	public class MessageEx
	{
		private readonly Lazy<string> _accountTag;

		public MessageEx()
		{
			_accountTag = Lazy.Create(() => MessageAccountParser.Instance.Parse(Message).Name);
		}

		public Message Message { get; set; }
		public MessageOrigin MessageOrigin { get; set; }
		public Action DoReceive { get; set; }

		public string AccountTag
		{
			get { return _accountTag.Value; }
		}
	}
}