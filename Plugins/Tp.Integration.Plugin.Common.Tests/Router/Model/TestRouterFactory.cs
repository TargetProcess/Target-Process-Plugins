using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Interfaces;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Log;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Pump;

namespace Tp.Integration.Plugin.Common.Tests.Router.Model
{
	class TestRouterFactory : IRouterFactory<TestMessage>, IProducerConsumerFactory<TestMessage>
	{
		private readonly Waiter _waiter;
		private readonly Action<IList<TestMessage>> _orderChecker;
		private readonly Dictionary<string, Subject<TestMessage>> _subjects;

		public TestRouterFactory(Waiter waiter, Action<IList<TestMessage>> orderChecker)
		{
			_waiter = waiter;
			_orderChecker = orderChecker;
			_subjects = new Dictionary<string, Subject<TestMessage>>();
		}

		public IMessageSource<TestMessage> CreateSource(string sourceName)
		{
			return new MessageSource<TestMessage>(sourceName, GetOrCreateSubject(sourceName));
		}

		public IMessageConsumer<TestMessage> CreateConsumer(IMessageSource<TestMessage> messageSource)
		{
			var consumer = new MessageConsumer<TestMessage>(messageSource, Scheduler.ThreadPool)
			       	{
			       		While = m => TestMessage.IsNotStopMessage(messageSource.Name, m)
			       	};
			_waiter.Register(consumer);
			consumer.AddObserver(new AssertObserver(_orderChecker));
			return consumer;
		}
		
		public IMessageProducer<TestMessage> CreateProducer(IMessageSource<TestMessage> messageSource)
		{
			return new TestMessageProducer(GetOrCreateSubject(messageSource.Name));
		}

		public IMessageConsumer<TestMessage> CreateRouter(IMessageSource<TestMessage> messageSource, IProducerConsumerFactory<TestMessage> factory, Func<TestMessage, string> routeBy)
		{
			var router = new TestMessageRouter(messageSource, factory, routeBy, Scheduler.CurrentThread, new LoggerContextSensitive());
			return router;
		}
		
		private ISubject<TestMessage> GetOrCreateSubject(string sourceName)
		{
			Subject<TestMessage> subject;
			if (!_subjects.TryGetValue(sourceName, out subject))
			{
				subject = new Subject<TestMessage>();
				_subjects.Add(sourceName, subject);
			}
			return subject;
		}
	}
}