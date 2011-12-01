using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Tp.Testing.Common.NUnit;

namespace Tp.Integration.Plugin.Common.Tests.Router.Model
{
	class TestRouterHelper
	{
		public const int MESSAGES_COUNT_TO_GENERATE = 10;
		public const string SOURCE = "src";
		public static void CheckOrder(IEnumerable<TestMessage> result, IEnumerable<string> expectedOrder)
		{
			result.Select(m => m.Body).SequenceEqual(expectedOrder).Should(Be.True);
		}

		public static IEnumerable<string> SequenceExcluding(int start, int count, int excludeIndex)
		{
			var expectedOrder =
				Enumerable.Range(start, excludeIndex).Concat(Enumerable.Range(excludeIndex + 1, count - excludeIndex - 1))
					.Select(i => i.ToString())
					.ToList();
			return expectedOrder;
		}

		public static IEnumerable<string> Sequence(int start, int count)
		{
			return Enumerable.Range(start, count)
				.Select(i => i.ToString())
				.ToList();
		}

		public static void HandleMessage(TestMessage message)
		{
			Console.WriteLine("{0} handled.", message);
		}

		public static void WaitFor(Waiter waiter, bool shouldFailIfTimeOut = false)
		{
			bool receiveSignal = waiter.Wait(TimeSpan.FromSeconds(100));
			if (shouldFailIfTimeOut && !receiveSignal)
			{
				Assert.Fail("Timeout exception");
			}
		}
	}
}