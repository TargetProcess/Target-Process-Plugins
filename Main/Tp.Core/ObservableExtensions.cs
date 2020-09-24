using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using Tp.Core.Annotations;

namespace Tp.Core
{
    public static class ObservableExtensions
    {
        /// <summary>
        /// When enabled, <see cref="ToSelfRepairingHotObservable{T}"/> uses custom realization instead of
        /// <see cref="Observable.Catch{TSource,TException}"/> which causes stack growing after each repairing.
        /// See https://plan.tpondemand.com/entity/214281-tp-crashes-with-stack-overflow-in
        /// </summary>
        private static readonly bool _customSelfRepairingHotObservableIsEnabled = ConfigurationManager
            .AppSettings["CustomSelfRepairingHotObservable"].NothingIfNull()
            .Select(value => bool.TryParse(value, out var parsedValue) && parsedValue).GetOrDefault();
        
        [NotNull]
        public static IObservable<T> ToSelfRepairingHotObservable<T>(
            [NotNull] this Func<IObservable<T>> createOriginSource,
            [NotNull] Action<Exception> onError)
        {
            if (!_customSelfRepairingHotObservableIsEnabled)
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
            
            var proxySubject = new Subject<T>();
            
            IDisposable currentUnderlyingSubscription = null;
            var currentUnderlyingSource = createOriginSource();

            return proxySubject.RefCount(SubscribeToCurrentUnderlying);

            IDisposable SubscribeToCurrentUnderlying()
            {
                var subscription = currentUnderlyingSource.Subscribe(proxySubject.OnNext, CreateOnError(), () =>
                {
                    proxySubject.OnCompleted();
                    StopCurrentUnderlyingSubscription();
                    proxySubject.Dispose();
                });
                SwapAndDisposePrevious(ref currentUnderlyingSubscription, subscription);

                return Disposable.Create(StopCurrentUnderlyingSubscription);

                Action<Exception> CreateOnError()
                {
                    return exception =>
                    {
                        StopCurrentUnderlyingSubscription();
                        onError(exception);
                        currentUnderlyingSource = createOriginSource();
                        SubscribeToCurrentUnderlying();
                    };
                }

                void StopCurrentUnderlyingSubscription()
                {
                    SwapAndDisposePrevious(ref currentUnderlyingSubscription, null);
                }
            }
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
        
        private static void SwapAndDisposePrevious<T>(ref T valueHolder, T value) where T : class, IDisposable
        {
            Interlocked.Exchange(ref valueHolder, value)?.Dispose();
        }

        private static IObservable<T> RefCount<T>(
            this IObservable<T> source, Func<IDisposable> connect)
        {
            if (source == null)
                throw new ArgumentNullException(nameof (source));
            var gate = new object();
            var count = 0;
            var connectionToken = (IDisposable) null;
            return Observable.Create<T>(observer =>
            {
                bool flag;
                lock (gate)
                {
                    ++count;
                    flag = count == 1;
                }
                var subscription = source.Subscribe(observer);
                if (flag)
                    connectionToken = connect();
                return Disposable.Create(() =>
                {
                    subscription.Dispose();
                    lock (gate)
                    {
                        --count;
                        if (count != 0)
                            return;
                        connectionToken?.Dispose();
                    }
                });
            });
        }
    }
}
