using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace Tp.Core
{
	public static class ObservableExtensions
	{
		public static IObservable<T> ToSelfRepairingHotObservable<T>(this Func<IObservable<T>> createOriginSource, Action<Exception> onError)
		{
			Func<IObservable<T>> createSource = null;

			createSource = () => createOriginSource()
				.Catch((Func<Exception, IObservable<T>>) (e =>
				{
					onError(e);

					return createSource();
				}))
				.Publish()
				.RefCount();

			return createSource();
		}

		public static IObservable<T> Iterate<T>(this IEnumerable<IObservable<T>> observables, Action<T> handle,
			IScheduler scheduler, Action<string> trace) where T : class
		{
			return Observable.Create<T>(o =>
			{
				IEnumerator<IObservable<T>> observablesEnumerator = observables.GetEnumerator();
				return scheduler.Schedule(self =>
				{
					bool moveNext = observablesEnumerator.MoveNext();
					if (!moveNext)
					{
						trace("ObservableExtensions.Iterate : Consuming items was completed");
						o.OnCompleted();
						return;
					}
					observablesEnumerator.Current.Subscribe(m =>
					{
						if (m != null)
						{
							try
							{
								handle(m);
								o.OnNext(m);
								self();
							}
							catch (Exception e)
							{
								trace("ObservableExtensions.Iterate : Exception occurs : {0}".Fmt(e.ToString()));
								o.OnError(e);
							}
						}
						else
						{
							self();
						}
					});
				});
			});
		}
	}
}
