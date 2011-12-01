using System;
using System.Reactive.Concurrency;
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
		private struct Tuple<T1,T2>
		{
			public T1 First { get; set; }
			public T2 Second { get; set; }
		}

		private static Tuple<TElement1, TElement2> T2<TElement1, TElement2>(TElement1 first, TElement2 second)
		{
			return new Tuple<TElement1, TElement2> { First = first, Second = second };
		} 

		public static IObservable<TItem> Until<TItem>(this IObservable<TItem> items, Predicate<TItem> @while, Action<TItem> handle, Action @finally, IScheduler scheduler)
		{
			return items.Select(i => Observable.Defer(() => @while(i) 
															? Observable.Start(() => { handle(i); return T2(i, true); }, scheduler)
															: Observable.Return(T2(i, false))))
						.Concat()
						.Repeat()
						.TakeWhile(_ => _.Second)
						.Select(_ => _.First)
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
	}
}