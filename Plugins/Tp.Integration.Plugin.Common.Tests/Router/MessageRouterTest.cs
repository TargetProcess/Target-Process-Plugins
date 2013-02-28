using System.Collections;
using NUnit.Framework;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Interfaces;
using Tp.Integration.Plugin.Common.Tests.Router.Model;
using Tp.Testing.Common.NUnit;

namespace Tp.Integration.Plugin.Common.Tests.Router
{
	[TestFixture]
	class MessageRouterTest
	{
		[Test]
		public void ChildrenOrderTest()
		{
			const int sourcesCount = 5;
			IMessageSource<TestMessage> source = MessageGenerator.CreateCompositeSource(TestRouterHelper.SourceName, sourcesCount, TestRouterHelper.MessagesCountToGenerate);
			var waiter = new Waiter(sourcesCount);
			var factory = new TestRouterFactory(waiter, messages => messages.CheckOrder(TestRouterHelper.Sequence(0, TestRouterHelper.MessagesCountToGenerate)));
			using (IMessageConsumer<TestMessage> router = factory.CreateRouter(source, factory, m => m.Tag))
			{
				router.Consume(TestRouterHelper.HandleMessage);
				waiter.Wait(true);
				// we compare to 55, not 50, because each source has stop message at the end.
				((TestMessageRouter) router).ReceiveCallCount.Should(Be.EqualTo(55)); 
			}
		}

		[Test]
		public void RouterShouldHandleMessagesIfTagIsNotProvided()
		{
			IMessageSource<TestMessage> source = MessageGenerator.CreateSource(string.Empty, 5);
			var waiter = new Waiter(1);
			var factory = new TestRouterFactory(waiter, messages => { });
			using (IMessageConsumer<TestMessage> router = factory.CreateRouter(source, factory, m => m.Tag))
			{
				var messagesHandled = 0;
				waiter.Register(router);
				router.While = m => TestMessage.IsNotStopMessage(string.Empty, m);
				router.Consume(message => messagesHandled++);
				waiter.Wait(true);

				messagesHandled.Should(Be.EqualTo(6));
				//Do not receive message from queue, if router handles message by itself. Let the handle logic do this.
				((TestMessageRouter)router).ReceiveCallCount.Should(Be.EqualTo(0));
			}
		}
	}
}