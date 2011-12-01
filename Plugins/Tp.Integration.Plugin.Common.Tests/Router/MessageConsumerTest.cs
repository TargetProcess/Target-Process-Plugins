// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using NUnit.Framework;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Exceptions;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Interfaces;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Pump;
using Tp.Integration.Plugin.Common.Tests.Router.Model;
using Tp.Testing.Common.NUnit;

namespace Tp.Integration.Plugin.Common.Tests.Router
{
	[TestFixture]
	internal class MessageConsumerTest
	{
		[Test]
		public void CheckIsRunningTest()
		{
			var waiter = new Waiter();
			IMessageSource<TestMessage> source = Generator.CreateSource(TestRouterHelper.SOURCE, TestRouterHelper.MESSAGES_COUNT_TO_GENERATE, null);
			using (IMessageConsumer<TestMessage> consumer = CreateConsumer(source, new AssertObserver(messages => { })))
			{
				waiter.Register(consumer);
				consumer.IsRunning.Should(Be.False);
				consumer.Consume(TestRouterHelper.HandleMessage);
				consumer.IsRunning.Should(Be.True);
				TestRouterHelper.WaitFor(waiter);
				consumer.IsRunning.Should(Be.True);
			}
		}

		[Test]
		public void CheckOrderTest()
		{
			IEnumerable<string> expectedOrder = TestRouterHelper.Sequence(0, TestRouterHelper.MESSAGES_COUNT_TO_GENERATE);
			var waiter = new Waiter();
			IMessageSource<TestMessage> source = Generator.CreateSource(TestRouterHelper.SOURCE, TestRouterHelper.MESSAGES_COUNT_TO_GENERATE, null);
			using (IMessageConsumer<TestMessage> consumer = CreateConsumer(source, new AssertObserver(messages => TestRouterHelper.CheckOrder(messages, expectedOrder))))
			{
				waiter.Register(consumer);
				consumer.Consume(TestRouterHelper.HandleMessage);
				TestRouterHelper.WaitFor(waiter, true);
			}
		}

		[Test]
		public void SwallowExceptionTest()
		{
			const int errorMessageIndex = 5;
			IEnumerable<string> expectedOrder = TestRouterHelper.SequenceExcluding(0, TestRouterHelper.MESSAGES_COUNT_TO_GENERATE, errorMessageIndex);
			var waiter = new Waiter();
			IMessageSource<TestMessage> source = Generator.CreateSource(TestRouterHelper.SOURCE, TestRouterHelper.MESSAGES_COUNT_TO_GENERATE, errorMessageIndex);
			var o = source.Catch((Exception e) =>
			                      	{
			                      		Console.WriteLine(e.ToString());
			                      		return source;
			                      	});
			using (var consumer = CreateConsumer(new MessageSource<TestMessage>(source.Name, o), new AssertObserver(messages => TestRouterHelper.CheckOrder(messages, expectedOrder))))
			{
				waiter.Register(consumer);
				consumer.Consume(TestRouterHelper.HandleMessage);
				TestRouterHelper.WaitFor(waiter, true);
			}
		}

		[Test, ExpectedException(typeof (MessageConsumerException))]
		public void DenyMonitorChangingOfRunningMessageConsumer()
		{
			IEnumerable<string> expectedOrder = TestRouterHelper.Sequence(0, TestRouterHelper.MESSAGES_COUNT_TO_GENERATE);
			IMessageSource<TestMessage> source = Generator.CreateSource(TestRouterHelper.SOURCE, TestRouterHelper.MESSAGES_COUNT_TO_GENERATE, null);
			using (IMessageConsumer<TestMessage> consumer = CreateConsumer(source, new AssertObserver(messages => TestRouterHelper.CheckOrder(messages, expectedOrder))))
			{
				consumer.Consume(TestRouterHelper.HandleMessage);
				consumer.AddObserver(null);
			}
		}

		[Test, ExpectedException(typeof (MessageConsumerException))]
		public void DenyWhileChangingOfRunningMessageConsumer()
		{
			IEnumerable<string> expectedOrder = TestRouterHelper.Sequence(0, TestRouterHelper.MESSAGES_COUNT_TO_GENERATE);
			IMessageSource<TestMessage> source = Generator.CreateSource(TestRouterHelper.SOURCE, TestRouterHelper.MESSAGES_COUNT_TO_GENERATE, null);
			using (IMessageConsumer<TestMessage> consumer = CreateConsumer(source, new AssertObserver(messages => TestRouterHelper.CheckOrder(messages, expectedOrder))))
			{
				consumer.Consume(TestRouterHelper.HandleMessage);
				consumer.While = null;
			}
		}

		private static IMessageConsumer<TestMessage> CreateConsumer(IMessageSource<TestMessage> messageSource, IObserver<TestMessage> observer)
		{
			var consumer = new MessageConsumer<TestMessage>(messageSource, Scheduler.ThreadPool)
			               	{
								While = m => TestMessage.IsNotStopMessage(messageSource.Name, m)
			               	};
			consumer.AddObserver(observer);
			return consumer;
		}
	}
}