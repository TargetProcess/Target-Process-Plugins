using System;
using System.Messaging;

namespace Tp.Integration.Messages.ServiceBus.Transport.Router.MsmqRx
{
    public class MessageEx : IMessageWithId
    {
        private readonly Lazy<string> _accountTag;

        public MessageEx()
        {
            _accountTag = Lazy.Create(() => MessageAccountParser.Instance.Parse(Message).Name);
        }

        public Message Message { get; set; }
        public MessageOrigin MessageOrigin { get; set; }
        public Action<MessageQueueTransaction> DoTransactionReceive { get; set; }
        public Action<MessageQueueTransactionType> DoTransactionTypeReceive { get; set; }

        public string AccountTag => _accountTag.Value;
        public string Id => Message.Id;
    }
}
