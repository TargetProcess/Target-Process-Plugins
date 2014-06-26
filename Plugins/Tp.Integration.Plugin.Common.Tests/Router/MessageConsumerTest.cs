// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NUnit.Framework;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Exceptions;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Interfaces;
using Tp.Integration.Plugin.Common.Tests.Router.Model;
using Tp.Testing.Common.NUnit;

namespace Tp.Integration.Plugin.Common.Tests.Router
{
	[TestFixture]
    [Category("PartPlugins1")]
	internal class MessageConsumerTest
	{
		[Test]
		public void CheckIsRunningTest()
		{
			IMessageSource<TestMessage> source = MessageGenerator.CreateSource(TestRouterHelper.SourceName, TestRouterHelper.MessagesCountToGenerate);
			var waiter = new Waiter(waitablesCount:1);
			var factory = new TestRouterFactory(waiter, messages =>{});
			using (IMessageConsumer<TestMessage> consumer = factory.CreateConsumer(source))
			{
				consumer.IsRunning.Should(Be.False);
				consumer.Consume(TestRouterHelper.HandleMessage);
				consumer.IsRunning.Should(Be.True);
				waiter.Wait();
				consumer.IsRunning.Should(Be.True);
			}
		}

		[Test]
		public void CheckOrderTest()
		{
			IMessageSource<TestMessage> source = MessageGenerator.CreateSource(TestRouterHelper.SourceName, TestRouterHelper.MessagesCountToGenerate);
			var waiter = new Waiter(waitablesCount:1);
			var factory = new TestRouterFactory(waiter, messages => messages.CheckOrder(TestRouterHelper.Sequence(0, TestRouterHelper.MessagesCountToGenerate)));
			using(IMessageConsumer<TestMessage> consumer = factory.CreateConsumer(source))
			{
				consumer.Consume(TestRouterHelper.HandleMessage);
				waiter.Wait(true);
			}
		}

		[Test]
		public void SwallowExceptionTest()
		{
			const int errorMessageIndex = 5;
			var waiter = new Waiter(waitablesCount:1);
			IMessageSource<TestMessage> source = MessageGenerator.CreateSource(TestRouterHelper.SourceName, TestRouterHelper.MessagesCountToGenerate, errorMessageIndex, swallowException:true);
			var factory = new TestRouterFactory(waiter, messages => messages.CheckOrder(TestRouterHelper.SequenceExcluding(0, TestRouterHelper.MessagesCountToGenerate, errorMessageIndex)));
			using (IMessageConsumer<TestMessage> consumer = factory.CreateConsumer(source))
			{
				consumer.Consume(TestRouterHelper.HandleMessage);
				waiter.Wait(true);
			}
		}

		[Test, ExpectedException(typeof (MessageConsumerException))]
		public void DenyMonitorChangingOfRunningMessageConsumer()
		{
			IMessageSource<TestMessage> source = MessageGenerator.CreateSource(TestRouterHelper.SourceName, TestRouterHelper.MessagesCountToGenerate);
			var factory = new TestRouterFactory(new Waiter(waitablesCount: 1), messages => messages.CheckOrder(TestRouterHelper.Sequence(0, TestRouterHelper.MessagesCountToGenerate)));
			using (IMessageConsumer<TestMessage> consumer = factory.CreateConsumer(source))
			{
				consumer.Consume(TestRouterHelper.HandleMessage);
				consumer.AddObserver(null);
			}
		}

		[Test, ExpectedException(typeof (MessageConsumerException))]
		public void DenyWhileChangingOfRunningMessageConsumer()
		{
			IMessageSource<TestMessage> source = MessageGenerator.CreateSource(TestRouterHelper.SourceName, TestRouterHelper.MessagesCountToGenerate);
			var factory = new TestRouterFactory(new Waiter(waitablesCount: 1), messages => messages.CheckOrder(TestRouterHelper.Sequence(0, TestRouterHelper.MessagesCountToGenerate)));
			using (IMessageConsumer<TestMessage> consumer = factory.CreateConsumer(source))
			{
				consumer.Consume(TestRouterHelper.HandleMessage);
				consumer.While = null;
			}
		}
	}
}