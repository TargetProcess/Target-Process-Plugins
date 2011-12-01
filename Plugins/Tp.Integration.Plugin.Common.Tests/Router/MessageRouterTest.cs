using System.Collections.Generic;
using NUnit.Framework;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Interfaces;
using Tp.Integration.Plugin.Common.Tests.Router.Model;

namespace Tp.Integration.Plugin.Common.Tests.Router
{
	[TestFixture]
	class MessageRouterTest
	{
		[Test]
		public void ChildrenOrderTest()
		{
			IMessageSource<TestMessage> source = Generator.CreateSource(TestRouterHelper.SOURCE, 5, 10, null);
			var waiter = new Waiter();
			var factory = new TestRouterFactory(waiter, messages =>
													{
														IEnumerable<string> expectedOrder = TestRouterHelper.Sequence(0, TestRouterHelper.MESSAGES_COUNT_TO_GENERATE);
														TestRouterHelper.CheckOrder(messages, expectedOrder);
													});
			using (IMessageConsumer<TestMessage> router = factory.CreateRouter(source, factory, m => m.Tag))
			{
				router.Consume(TestRouterHelper.HandleMessage);
				TestRouterHelper.WaitFor(waiter, true);
			}
		}
	}
}