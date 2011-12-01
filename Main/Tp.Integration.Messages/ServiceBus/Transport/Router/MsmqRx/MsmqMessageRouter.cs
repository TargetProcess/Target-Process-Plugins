using System;
using System.Reactive.Concurrency;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Interfaces;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Log;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Pump;

namespace Tp.Integration.Messages.ServiceBus.Transport.Router.MsmqRx
{
	public class MsmqMessageRouter : MessageRouter<MessageEx>
	{
		public MsmqMessageRouter(IMessageSource<MessageEx> messageSource, IProducerConsumerFactory<MessageEx> producerConsumerFactory, Func<MessageEx, string> tagMessageProvider, IScheduler scheduler, ILoggerContextSensitive log)
			: base(messageSource, producerConsumerFactory, tagMessageProvider, scheduler, log)
		{
		}

		protected override void Preprocess(MessageEx message)
		{
			MessageLabel messageLabel = MessageLabel.Parse(message.Message.Label);
			var newMessageLabel = new MessageLabel(messageLabel.WindowsIdentityName, message.Message.Id);
			message.Message.Label = newMessageLabel.ToString();
		}
	}
}