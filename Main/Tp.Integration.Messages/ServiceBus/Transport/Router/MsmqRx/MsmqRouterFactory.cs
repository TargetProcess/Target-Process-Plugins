// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Diagnostics;
using System.Messaging;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Security.Principal;
using System.Threading;
using NServiceBus.Utils;
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
		private readonly MessageQueueTransactionType _sendTransactionType;
		private readonly MessageQueueTransactionType _receiveTransactionType;
		private readonly Stopwatch _stopwatch;
		private int _childrenCount;

		public MsmqRouterFactory(ILoggerContextSensitive log, TimeSpan waitMessageTimeout, MessageQueueTransactionType sendTransactionType, MessageQueueTransactionType receiveTransactionType)
		{
			_stopwatch = new Stopwatch();
			_log = log;
			_waitMessageTimeout = waitMessageTimeout;
			_sendTransactionType = sendTransactionType;
			_receiveTransactionType = receiveTransactionType;
		}

		public IMessageSource<MessageEx> CreateSource(string sourceName)
		{
			MessageQueue queue = MessageQueueFactory.GetOrCreateMessageQueue(sourceName);
			var messageOrigin = new MessageOrigin
			                    	{
										Address = MsmqUtilities.GetIndependentAddressForQueue(queue),
										FormatName = queue.FormatName,
										Name = queue.QueueName
			                    	};
			Func<IObservable<Message>> asyncCallAsObservableFactory = Observable.FromAsyncPattern((callback, obj) => queue.BeginPeek(_waitMessageTimeout, obj, callback), asyncResult => EndPeekAndReceive(sourceName + "~source", queue, asyncResult));
			IObservable<Message> messagesStream = null;
			messagesStream = Observable.Defer(asyncCallAsObservableFactory).Catch((MessageQueueException e) =>
			                        	{
			                        		if (e.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
			                        		{
												return messagesStream;
			                        		}
			                        		if (e.MessageQueueErrorCode == MessageQueueErrorCode.AccessDenied)
			                        		{
			                        			WindowsIdentity windowsIdentity = WindowsIdentity.GetCurrent();
			                        			string accessDeniedMessage = string.Format("Do not have permission to access queue [{0}]. Make sure that the current user [{1}] has permission to Send, Receive, and Peek  from this queue. NServiceBus will now exit.", queue.QueueName, windowsIdentity != null ? windowsIdentity.Name : "no windows identity");
												_log.Fatal(LoggerContext.New(sourceName), accessDeniedMessage, e);
												Thread.Sleep(10000); //long enough for someone to notice
			                        			Process.GetCurrentProcess().Kill();
			                        		}
			                        		var message = string.Format("Problem in peeking/receiving a message from queue: {0}", Enum.GetName(typeof (MessageQueueErrorCode), e.MessageQueueErrorCode));
			                        		if (e.MessageQueueErrorCode == MessageQueueErrorCode.ServiceNotAvailable || e.MessageQueueErrorCode == MessageQueueErrorCode.OperationCanceled)
			                        		{
			                        			//this exceptions occur after windows restart. This is normal situation.
												_log.Fatal(LoggerContext.New(sourceName), message, e);
			                        		}
			                        		else
			                        		{
												_log.Error(LoggerContext.New(sourceName), message, e);
			                        		}
			                        		Thread.Sleep(_waitMessageTimeout);
											return messagesStream;
			                        	})
							.Catch((ObjectDisposedException e) =>
				       				{
										_log.Fatal(LoggerContext.New(sourceName), "Queue has been disposed. Cannot continue operation. Please restart this process.", e);
										Thread.Sleep(_waitMessageTimeout);
										return messagesStream;
				       				})
							.Catch((Exception e) =>
				       				{
										_log.Error(LoggerContext.New(sourceName), "Error in peeking/receiving a message from queue.", e);
										Thread.Sleep(_waitMessageTimeout);
										return messagesStream;
				       				});
			var messages = messagesStream.Select(m => new MessageEx
			                                  	{
			                                  		Message = m,
													MessageOrigin = messageOrigin
			                                  	});
			return new MessageSource<MessageEx>(sourceName, messages);
		}

		[DebuggerNonUserCode]
		private Message EndPeekAndReceive(string queueShortName, MessageQueue queue, IAsyncResult asyncResult)
		{
			queue.EndPeek(asyncResult);
			var message = queue.Receive(TimeSpan.FromSeconds(1), _receiveTransactionType);
			_log.Debug(LoggerContext.New(queueShortName), "End peek and receive msg.");
			return message;
		}

		public IMessageProducer<MessageEx> CreateProducer(IMessageSource<MessageEx> messageSource)
		{
			return new MsmqMessageProducer(messageSource.Name, _sendTransactionType, _log);
		}

		public IMessageConsumer<MessageEx> CreateConsumer(IMessageSource<MessageEx> messageSource)
		{
			if (!_stopwatch.IsRunning)
			{
				_stopwatch.Start();
			}
			Interlocked.Increment(ref _childrenCount);
			var consumer = new MessageConsumer<MessageEx>(messageSource, Scheduler.ThreadPool)
			               	{
								While = GetWhile(messageSource.Name)
			               	};
			consumer.AddObserver(new StopwatchObserver<MessageEx>(_stopwatch, s => _log.Debug(LoggerContext.New(consumer.Name), s), e => _log.Error(LoggerContext.New(consumer.Name), string.Empty, e), null, () => Interlocked.Decrement(ref _childrenCount) == 0));
			return consumer;
		}

		public IMessageConsumer<MessageEx> CreateRouter(IMessageSource<MessageEx> messageSource, IProducerConsumerFactory<MessageEx> factory, Func<MessageEx, string> routeBy)
		{
			var router = new MsmqMessageRouter(messageSource, factory, routeBy, Scheduler.CurrentThread, _log)
			       	{
						While = GetWhile(messageSource.Name)
			       	};
			router.AddObserver(new StopwatchObserver<MessageEx>(Stopwatch.StartNew(), s => _log.Debug(LoggerContext.New(router.Name), s), e => _log.Error(LoggerContext.New(router.Name), string.Empty, e)));
			return router;
		}

		private Predicate<MessageEx> GetWhile(string queueName)
		{
#if HANDLE_MESSAGE_CONSUMER_STOP_MESSAGE
			return m => 
			{
				MessageAccount messageLabel = MessageAccountParser.Instance.Parse(m);
				return !(queueName.Contains(messageLabel.Name) && messageLabel.Command == "stop");
			};
#else
			return null;
#endif
		}
	}
}