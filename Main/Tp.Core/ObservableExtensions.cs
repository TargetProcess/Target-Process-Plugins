using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Tp.Core
{
	public static class ObservableExtensions
	{
		public static IObservable<T> ToSelfRepairingHotObservable<T>(this Func<IObservable<T>> createOriginSource, Action<string> logNext, Action<Exception> logError)
		{
			Func<IObservable<T>> createSource = null;
			createSource = () => createOriginSource()
											.Do(@event => logNext("Publishing event: {0}".Fmt(@event)))
											.Catch((Func<Exception, IObservable<T>>) (e =>
											{
												logError(e);
												return createSource();
											}))
											.Publish()
											.RefCount();
			return createSource();
		}

		private class TracedDisposable : IDisposable
		{
			private readonly IDisposable _disposable;
			private readonly Action<string> _trace;
			public TracedDisposable(IDisposable disposable, Action<string> trace)
			{
				_disposable = disposable;
				_trace = trace;
			}

			public void Dispose()
			{
				try
				{
					_trace("Disposable of type {0} is disposing".Fmt(_disposable.GetType()));
					_disposable.Dispose();
					_trace("Disposable of type {0} is disposed".Fmt(_disposable.GetType()));
				}
				catch(Exception)
				{
					_trace("Disposable of type {0} disposing failed".Fmt(_disposable.GetType()));
					throw;
				}
			}
		}

		public static IObservable<T> Iterate<T>(this IEnumerable<IObservable<T>> observables, Action<T> handle, IScheduler scheduler, Action<string> trace) where T : class
		{
			return Observable.Create<T>(o =>
			{
				IEnumerator<IObservable<T>> observablesEnumerator = observables.GetEnumerator();
				var disposable = new SerialDisposable();
				Action work = null;
				work = () =>
				{
					if (disposable.IsDisposed || !observablesEnumerator.MoveNext())
					{
						trace("ObservableExtensions.Iterate : Consuming items was completed");
						o.OnCompleted();
						return;
					}
					disposable.Disposable = observablesEnumerator.Current.Subscribe(m => scheduler.Schedule(_ =>
					{
						if (m != null)
						{
							try
							{
								handle(m);
								o.OnNext(m);
							}
							catch (Exception e)
							{
								trace("ObservableExtensions.Iterate : Exception occurs : {0}".Fmt(e.ToString()));
								o.OnError(e);
							}
						}
						Scheduler.CurrentThread.Schedule(work);
					}));
				};
				work();
				return new TracedDisposable(disposable, trace);
			});
		}
	}
}