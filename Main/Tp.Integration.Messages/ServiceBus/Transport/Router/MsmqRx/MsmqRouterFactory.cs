using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Messaging;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Security.Principal;
using System.Threading;
using System.Transactions;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Interfaces;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Log;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Pump;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Utils;

namespace Tp.Integration.Messages.ServiceBus.Transport.Router.MsmqRx
{
    class MsmqRouterFactory : IRouterFactory<MessageEx>, IProducerConsumerFactory<MessageEx>
    {
        private readonly ILoggerContextSensitive _log;
        private readonly TimeSpan _waitMessageTimeout;
        private readonly Stopwatch _stopwatch;
        private int _childrenCount;

        public MsmqRouterFactory(ILoggerContextSensitive log, TimeSpan waitMessageTimeout)
        {
            _stopwatch = new Stopwatch();
            _log = log;
            _waitMessageTimeout = waitMessageTimeout;
        }

        public IMessageSource<MessageEx> CreateSource(string sourceName, bool isChild)
        {
            return new MessageSource<MessageEx>(sourceName, GetMessageStream(sourceName, isChild));
        }

        private IEnumerable<IObservable<MessageEx>> GetMessageStream(string sourceName, bool isChild)
        {
            MessageQueue queue = MessageQueueFactory.GetOrCreateMessageQueue(sourceName);
            var messageOrigin = new MessageOrigin
            {
                Name = sourceName
            };

            Func<IObservable<Message>> recieveObservableFactory = Observable.FromAsyncPattern(
                (callback, obj) =>
                {
                    //Wait infinite time here. Otherwise memory leak occurs.
                    return queue.BeginPeek(TimeSpan.FromMilliseconds(UInt32.MaxValue), obj, callback);
                },
                asyncResult => EndPeekAndReceive(queue, asyncResult));
            while (true)
            {
                var messagesStream = recieveObservableFactory().Select(m => new MessageEx
                    {
                        Message = m,
                        MessageOrigin = messageOrigin,
                        DoTransactionTypeReceive = receiveTransactionType => queue.Receive(TimeSpan.FromSeconds(0), receiveTransactionType),
                        DoTransactionReceive = mqTx => queue.Receive(mqTx)
                })
                    .Catch((MessageQueueException e) =>
                    {
                        if (e.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
                        {
                            _log.Info(LoggerContext.New(sourceName), "Msmq peek timeout occurs");
                            return Observable.Return<MessageEx>(null);
                        }
                        if (e.MessageQueueErrorCode == MessageQueueErrorCode.AccessDenied)
                        {
                            WindowsIdentity windowsIdentity = WindowsIdentity.GetCurrent();

                            string accessDeniedMessage =
                                string.Format(
                                    "Do not have permission to access queue [{0}]. Make sure that the current user [{1}] has permission to Send, Receive, and Peek  from this queue.",
                                    queue.QueueName, windowsIdentity != null ? windowsIdentity.Name : "no windows identity");
                            _log.Fatal(LoggerContext.New(sourceName), accessDeniedMessage, e);
                            try
                            {
                                EventLog.WriteEntry("TargetProcess Plugin", accessDeniedMessage, EventLogEntryType.Error);
                            }
                            catch (Exception)
                            {
                                //skip exception
                            }

                            //kill the process only if we cannot access main queue (in case of OnDemand it is router queue, in case of OnSite it is main queue)
                            if (!isChild)
                            {
                                _log.Fatal(LoggerContext.New(sourceName), "NServiceBus will now exit.", e);
                                Thread.Sleep(10000); //long enough for someone to notice
                                Process.GetCurrentProcess().Kill();
                            }
                        }
                        var message = string.Format("Problem in peeking/receiving a message from queue: {0}",
                            Enum.GetName(typeof(MessageQueueErrorCode), e.MessageQueueErrorCode));
                        if (e.MessageQueueErrorCode == MessageQueueErrorCode.ServiceNotAvailable
                            || e.MessageQueueErrorCode == MessageQueueErrorCode.OperationCanceled)
                        {
                            //this exceptions occur after windows restart. This is normal situation.
                            _log.Fatal(LoggerContext.New(sourceName), message, e);
                        }
                        else
                        {
                            _log.Error(LoggerContext.New(sourceName), message, e);
                        }
                        Thread.Sleep(_waitMessageTimeout);
                        return Observable.Return<MessageEx>(null);
                    })
                    .Catch((ObjectDisposedException e) =>
                    {
                        _log.Fatal(LoggerContext.New(sourceName),
                            "Queue has been disposed. Cannot continue operation. Please restart this process.", e);
                        Thread.Sleep(_waitMessageTimeout);
                        return Observable.Return<MessageEx>(null);
                    })
                    .Catch((Exception e) =>
                    {
                        _log.Error(LoggerContext.New(sourceName), "Error in peeking/receiving a message from queue.", e);
                        Thread.Sleep(_waitMessageTimeout);
                        return Observable.Return<MessageEx>(null);
                    });
                yield return messagesStream;
            }
        }

        [DebuggerNonUserCode]
        private Message EndPeekAndReceive(MessageQueue queue, IAsyncResult asyncResult)
        {
            var message = queue.EndPeek(asyncResult);
            return message;
        }

        public IMessageProducer<MessageEx> CreateProducer(IMessageSource<MessageEx> messageSource)
        {
            return new MsmqMessageProducer(messageSource.Name, _log);
        }

        public IMessageConsumer<MessageEx> CreateConsumer(IMessageSource<MessageEx> messageSource)
        {
            if (!_stopwatch.IsRunning)
            {
                _stopwatch.Start();
            }
            Interlocked.Increment(ref _childrenCount);
            var consumer = new MessageConsumer<MessageEx>(messageSource, Scheduler.ThreadPool, _log);
            consumer.AddObserver(new StopwatchObserver<MessageEx>(_stopwatch, s => _log.Debug(LoggerContext.New(consumer.Name), s),
                s => _log.Info(LoggerContext.New(consumer.Name), s), e => _log.Error(LoggerContext.New(consumer.Name), string.Empty, e),
                null,
                () => Interlocked.Decrement(ref _childrenCount) == 0));
            return consumer;
        }

        public IMessageConsumer<MessageEx> CreateRouter(IMessageSource<MessageEx> messageSource, IProducerConsumerFactory<MessageEx> factory,
            Func<MessageEx, string> routeBy, TransportTransactionMode transactionMode, IsolationLevel isolationLevel, TimeSpan transactionTimeout,
            MessageQueueTransactionType sendTransactionType, MessageQueueTransactionType receiveTransactionType)
        {
            var router = new MsmqMessageRouter(messageSource, factory, routeBy, Scheduler.CurrentThread, _log, transactionMode,
                isolationLevel, transactionTimeout, sendTransactionType, receiveTransactionType);
            router.AddObserver(new StopwatchObserver<MessageEx>(Stopwatch.StartNew(), s => _log.Debug(LoggerContext.New(router.Name), s),
                s => _log.Info(LoggerContext.New(router.Name), s), e => _log.Error(LoggerContext.New(router.Name), string.Empty, e)));
            return router;
        }
    }
}
