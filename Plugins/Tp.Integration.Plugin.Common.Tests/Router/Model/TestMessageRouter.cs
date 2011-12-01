using System;
using System.Reactive.Concurrency;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Interfaces;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Log;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Pump;

namespace Tp.Integration.Plugin.Common.Tests.Router.Model
{
	class TestMessageRouter : MessageRouter<TestMessage>
	{
		public TestMessageRouter(IMessageSource<TestMessage> messageSource, IProducerConsumerFactory<TestMessage> producerConsumerFactory, Func<TestMessage, string> tagMessageProvider, IScheduler scheduler, ILoggerContextSensitive log)
			: base(messageSource, producerConsumerFactory, tagMessageProvider, scheduler, log)
		{
		}

		protected override void Preprocess(TestMessage message)
		{
		}
	}
}