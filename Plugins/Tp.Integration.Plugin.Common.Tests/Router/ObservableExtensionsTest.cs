using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using NUnit.Framework;
using Tp.Testing.Common.NUnit;
using TpObservableExtensions = Tp.Core.ObservableExtensions;
namespace Tp.Integration.Plugin.Common.Tests.Router
{
	[TestFixture]
	class ObservableExtensionsTest
	{
		private class Data
		{
			private readonly int _body;
			public Data(int body)
			{
				_body = body;
			}
			public int Body
			{
				get { return _body; }
			}
		}

		[Test]
		public void IterateErrorHandlingTest()
		{
			var xs = Enumerable.Range(0, 10).Select(x => Observable.Return(new Data(x)));
			int errors = 0;
			Func<IObservable<Data>> createOriginSource = () => TpObservableExtensions.Iterate(xs, x =>
				{
					if (x.Body != 5)
					{
						Console.WriteLine("Handler: " + x.Body);
					}
					else
					{
						throw new ApplicationException();
					}
				}, Scheduler.ThreadPool, s => Console.WriteLine("Trace: " + s));
			var ev = new ManualResetEvent(false);
			var source = TpObservableExtensions.ToSelfRepairingHotObservable(createOriginSource, _ => { }, e =>
				{
					Console.WriteLine(e);
					errors++;
					if (errors == 5)
					{
						ev.Set();
					}
				});
			using (source.Subscribe(x => Console.WriteLine("Observer: " + x.Body), Console.WriteLine, () =>
																											{
																												ev.Set();
																												Console.WriteLine("Observer: completed");
																											}))
			{
				ev.WaitOne(TimeSpan.FromSeconds(5)).Should(Be.True);
			}
		}
	}
}