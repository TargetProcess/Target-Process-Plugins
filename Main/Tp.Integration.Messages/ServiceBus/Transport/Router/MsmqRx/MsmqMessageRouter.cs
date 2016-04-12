using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using StructureMap;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Interfaces;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Log;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Pump;

namespace Tp.Integration.Messages.ServiceBus.Transport.Router.MsmqRx
{
	public class MsmqMessageRouter : MessageRouter<MessageEx>
	{
		protected IRouterChildTagsSource RouterChildTagsSource;

		public MsmqMessageRouter(IMessageSource<MessageEx> messageSource, IProducerConsumerFactory<MessageEx> producerConsumerFactory,
			Func<MessageEx, string> tagMessageProvider, IScheduler scheduler, ILoggerContextSensitive log)
			: base(messageSource, producerConsumerFactory, tagMessageProvider, scheduler, log)
		{
			RouterChildTagsSource = ObjectFactory.GetInstance<IRouterChildTagsSource>();
		}

		protected override void Receive(MessageEx message)
		{
			message.DoReceive();
		}

		protected override IEnumerable<string> GetChildTags()
		{
			return RouterChildTagsSource.GetChildTags();
		}

		protected override bool NeedToHandle(MessageEx m)
		{
			return RouterChildTagsSource.NeedToHandleMessage(m);
		}

		protected override void Preprocess(MessageEx message)
		{
			MessageLabel messageLabel = MessageLabel.Parse(message.Message.Label);
			var idForCorelation = messageLabel.IdForCorrelation;
			if (string.IsNullOrWhiteSpace(idForCorelation))
			{
				idForCorelation = message.Message.Id;
			}
			var newMessageLabel = new MessageLabel(messageLabel.WindowsIdentityName, idForCorelation);
			message.Message.Label = newMessageLabel.ToString();
		}
	}
}
