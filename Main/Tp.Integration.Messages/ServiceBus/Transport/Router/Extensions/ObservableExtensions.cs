using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Tp.Integration.Messages.ServiceBus.Transport.Router.Extensions
{
	public enum RxAction
	{
		Subscribe,
		OnNext,
		OnError,
		OnCompleted,
		Dispose
	}

	public static class ObservableExtensions
	{
		public static IObservable<TItem> Until<TItem>(this IObservable<TItem> items, Predicate<TItem> @while, Action<TItem> handle, Action @finally, IScheduler scheduler)
		{
			return items.Select(i => Observable.Defer(() => @while(i) 
															? Observable.Start(() => { handle(i); return Tuple.Create(i, true); }, scheduler)
															: Observable.Return(Tuple.Create(i, false))))
						.Concat()
						.Repeat()
						.TakeWhile(_ => _.Item2)
						.Select(_ => _.Item1)
						.Finally(@finally);
		}
		
		public static IObservable<TSource> Trace<TSource>(this IObservable<TSource> source, string name, Action<string, RxAction> trace)
		{
			return Observable.Create<TSource>(o =>
			                                  	{
			                                  		trace(name, RxAction.Subscribe);
			                                  		IDisposable disposable = source.Subscribe(i => { trace(name, RxAction.OnNext); o.OnNext(i); }, 
			                                  		                                          e => { trace(name, RxAction.OnError); o.OnError(e); }, 
			                                  		                                          () => { trace(name, RxAction.OnCompleted); o.OnCompleted(); });
			                                  		return () => { trace(name, RxAction.Dispose); disposable.Dispose(); };
			                                  	});
		}

		public static IObservable<T> Iterate<T>(this IEnumerable<IObservable<T>> observables, Action<T> handle, IScheduler scheduler) where T : class
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
								o.OnError(e);
							}
						}
						Scheduler.CurrentThread.Schedule(work);
					}));
				};
				work();
				return disposable;
			});
		}
	}
}