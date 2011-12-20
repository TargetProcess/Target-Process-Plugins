using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Tp.Testing.Common.NUnit;

namespace Tp.Integration.Plugin.Common.Tests.Router.Model
{
	static class TestRouterHelper
	{
		public const int MessagesCountToGenerate = 10;
		public const string SourceName = "src";
		public static void CheckOrder(this IEnumerable<TestMessage> result, IEnumerable<string> expectedOrder)
		{
			result.Select(m => m.Body).SequenceEqual(expectedOrder).Should(Be.True);
		}

		public static IEnumerable<string> SequenceExcluding(int start, int count, int excludeIndex)
		{
			return Enumerable.Range(start, excludeIndex)
							.Concat(Enumerable.Range(excludeIndex + 1, count - excludeIndex - 1))
							.Select(i => i.ToString())
							.ToList();
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

		public static void Wait(this Waiter waiter, bool shouldFailIfTimeOut = false)
		{
			bool receiveSignal = waiter.Wait(TimeSpan.FromSeconds(100));
			if (shouldFailIfTimeOut && !receiveSignal)
			{
				Assert.Fail("Timeout exception");
			}
		}
	}
}