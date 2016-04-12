using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Messaging;
using System.Threading;
using System.Transactions;
using NServiceBus;
using NServiceBus.Serialization;
using NServiceBus.Unicast.Transport;
using NServiceBus.Unicast.Transport.Msmq;
using NServiceBus.Utils;
using Tp.Integration.Messages.ServiceBus.Serialization;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Interfaces;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Log;
using Tp.Integration.Messages.ServiceBus.Transport.Router.MsmqRx;
using Tp.Integration.Messages.ServiceBus.UnicastBus;
using XmlSerializer = System.Xml.Serialization.XmlSerializer;

namespace Tp.Integration.Messages.ServiceBus.Transport.Router
{
	public class MsmqRoutableTransport : IMsmqTransport
	{
		#region config info

		/// <summary>
		/// The path to the queue the transport will read from.
		/// Only specify the name of the queue - msmq specific address not required.
		/// When using MSMQ v3, only local queues are supported.
		/// </summary>
		public string InputQueue { get; set; }

		public string UiCommandInputQueue
		{
			get { return TpUnicastBus.GetUiQueueName(InputQueue); }
		}

		/// <summary>
		/// Sets the path to the queue the transport will transfer
		/// errors to.
		/// </summary>
		public string ErrorQueue { get; set; }

		/// <summary>
		/// Sets whether or not the transport is transactional.
		/// </summary>
		public bool IsTransactional { get; set; }

		/// <summary>
		/// Sets whether or not the transport should deserialize
		/// the body of the message placed on the queue.
		/// </summary>
		public bool SkipDeserialization { get; set; }

		/// <summary>
		/// Sets whether or not the transport should purge the input
		/// queue when it is started.
		/// </summary>
		public bool PurgeOnStartup { get; set; }

		public RoutableTransportMode RoutableTransportMode { get; set; }

		public void ReceiveMessageLater(TransportMessage m, string address)
		{
			if (!string.IsNullOrEmpty(address))
			{
				Send(m, address);
			}
		}

		private int maxRetries = 5;

		/// <summary>
		/// Sets the maximum number of times a message will be retried
		/// when an exception is thrown as a result of handling the message.
		/// This value is only relevant when <see cref="IsTransactional"/> is true.
		/// </summary>
		/// <remarks>
		/// Default value is 5.
		/// </remarks>
		public int MaxRetries
		{
			get { return maxRetries; }
			set { maxRetries = value; }
		}

		private int secondsToWaitForMessage = 10;

		/// <summary>
		/// Sets the maximum interval of time for when a thread thinks there is a message in the queue
		/// that it tries to receive, until it gives up.
		/// 
		/// Default value is 10.
		/// </summary>
		public int SecondsToWaitForMessage
		{
			get { return secondsToWaitForMessage; }
			set { secondsToWaitForMessage = value; }
		}

		/// <summary>
		/// Property for getting/setting the period of time when the transaction times out.
		/// Only relevant when <see cref="IsTransactional"/> is set to true.
		/// </summary>
		public TimeSpan TransactionTimeout { get; set; }

		/// <summary>
		/// Property for getting/setting the isolation level of the transaction scope.
		/// Only relevant when <see cref="IsTransactional"/> is set to true.
		/// </summary>
		public IsolationLevel IsolationLevel { get; set; }

		/// <summary>
		/// Property indicating that queues will not be created on startup
		/// if they do not already exist.
		/// </summary>
		public bool DoNotCreateQueues { get; set; }

		/// <summary>
		/// Sets the object which will be used to serialize and deserialize messages.
		/// </summary>
		public IMessageSerializer MessageSerializer { get; set; }

		#endregion

		#region ITransport Members

		/// <summary>
		/// Event which indicates that message processing has started.
		/// </summary>
		public event EventHandler StartedMessageProcessing;

		/// <summary>
		/// Event which indicates that message processing has completed.
		/// </summary>
		public event EventHandler FinishedMessageProcessing;

		/// <summary>
		/// Event which indicates that message processing failed for some reason.
		/// </summary>
		public event EventHandler FailedMessageProcessing;

		/// <summary>
		/// Gets/sets the number of concurrent threads that should be
		/// created for processing the queue.
		/// 
		/// Get returns the actual number of running worker threads, which may
		/// be different than the originally configured value.
		/// 
		/// When used as a setter, this value will be used by the <see cref="Start"/>
		/// method only and will have no effect if called afterwards.
		/// 
		/// To change the number of worker threads at runtime, call <see cref="ChangeNumberOfWorkerThreads"/>.
		/// </summary>
		public int NumberOfWorkerThreads { get; set; }

		/// <summary>
		/// Event raised when a message has been received in the input queue.
		/// </summary>
		public event EventHandler<TransportMessageReceivedEventArgs> TransportMessageReceived;

		/// <summary>
		/// Gets the address of the input queue.
		/// </summary>
		public string Address
		{
			get { return InputQueue; }
		}

		public IPluginQueueFactory PluginQueueFactory { get; set; }

		/// <summary>
		/// Changes the number of worker threads to the given target,
		/// stopping or starting worker threads as needed.
		/// </summary>
		/// <param name="targetNumberOfWorkerThreads"></param>
		public void ChangeNumberOfWorkerThreads(int targetNumberOfWorkerThreads)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Starts the transport.
		/// </summary>
		public void Start()
		{
			if (RoutableTransportMode == RoutableTransportMode.OnDemand)
			{
				int workersThreads;
				int ioThreads;
				ThreadPool.GetMaxThreads(out workersThreads, out ioThreads);
				ThreadPool.SetMaxThreads(100 * Environment.ProcessorCount, ioThreads);
				ThreadPool.SetMinThreads(50, 50);
			}

			CheckConfiguration();
			CreateQueuesIfNecessary();
			if (ErrorQueue != null)
			{
				_errorQueue = new MessageQueue(MsmqUtilities.GetFullPath(ErrorQueue));
			}
			if (!string.IsNullOrEmpty(InputQueue))
			{
				IPluginQueue inputQueue = PluginQueueFactory.Create(InputQueue);
				IPluginQueue commandQueue = PluginQueueFactory.Create(UiCommandInputQueue);
				if (PurgeOnStartup)
				{
					inputQueue.Purge();
					commandQueue.Purge();
				}
				Logger.Info(LoggerContext.New(inputQueue.Name), "starting...");
				Logger.Info(LoggerContext.New(commandQueue.Name), "starting...");

				var factory = new MsmqRouterFactory(Logger, TimeSpan.FromSeconds(SecondsToWaitForMessage), GetTransactionTypeForSend,
					GetTransactionTypeForReceive());
				_inputQueueRouter = CreateAndStartMainMessageConsumer(factory);
				_uiQueueRouter = CreateAndStartUiMessageConsumer(factory);
				Logger.Info(LoggerContext.New(inputQueue.Name), "started.");
				Logger.Info(LoggerContext.New(commandQueue.Name), "started.");
				_queue = inputQueue;
			}
		}

		private string GetQueueNameToRouteMessageIn(MessageEx m)
		{
			string accountName = m.AccountTag;
			return string.IsNullOrEmpty(accountName) ? null : GetQueueName(accountName);
		}

		public string GetQueueName(string accountName)
		{
			if (RoutableTransportMode == RoutableTransportMode.OnSite)
			{
				return InputQueue;
			}

			return InputQueue + ("." + accountName);
		}

		public bool TryDeleteQueue(string accountName)
		{
			string queueName = GetQueueName(accountName);
			_inputQueueRouter.Dispose(queueName);
			return PluginQueue.TryDeleteQueue(queueName, Logger);
		}

		public bool TryDeleteUiQueue(string accountName)
		{
			return PluginQueue.TryDeleteQueue(TpUnicastBus.GetUiQueueName(GetQueueName(accountName)), Logger);
		}

		private IMessageConsumer<MessageEx> CreateAndStartMainMessageConsumer(MsmqRouterFactory factory)
		{
			IMessageConsumer<MessageEx> consumer;
			IMessageSource<MessageEx> messageSource = factory.CreateSource(InputQueue, false);
			switch (RoutableTransportMode)
			{
				case RoutableTransportMode.OnSite:
					consumer = factory.CreateConsumer(messageSource);
					break;
				case RoutableTransportMode.OnDemand:
					consumer = factory.CreateRouter(messageSource, factory, GetQueueNameToRouteMessageIn);
					break;
				default:
					throw new ApplicationException(string.Format("{0} plugin hosting mode is not supported", RoutableTransportMode.ToString()));
			}

			consumer.IsTransactional = IsTransactional;
			consumer.IsolationLevel = IsolationLevel;
			consumer.TransactionTimeout = TransactionTimeout;

			consumer.Consume(Handle);
			return consumer;
		}

		private IMessageConsumer<MessageEx> CreateAndStartUiMessageConsumer(MsmqRouterFactory factory)
		{
			IMessageSource<MessageEx> messageSource = factory.CreateSource(UiCommandInputQueue, false);
			IMessageConsumer<MessageEx> consumer = factory.CreateConsumer(messageSource);
			switch (RoutableTransportMode)
			{
				case RoutableTransportMode.OnSite:
					consumer.Consume(Handle);
					break;
				case RoutableTransportMode.OnDemand:
					consumer.Consume(HandleAsync);
					break;
			}

			return consumer;
		}

		private void CheckConfiguration()
		{
			if (string.IsNullOrEmpty(InputQueue))
			{
				return;
			}
			var machine = MsmqUtilities.GetMachineNameFromLogicalName(InputQueue);
			if (machine.ToLower() != Environment.MachineName.ToLower())
			{
				throw new InvalidOperationException("Input queue must be on the same machine as this process.");
			}
			if (MessageSerializer == null && !SkipDeserialization)
			{
				throw new InvalidOperationException("No message serializer has been configured.");
			}
		}

		private void CreateQueuesIfNecessary()
		{
			if (!DoNotCreateQueues)
			{
				MsmqUtilities.CreateQueueIfNecessary(InputQueue);
				MsmqUtilities.CreateQueueIfNecessary(ErrorQueue);
				MsmqUtilities.CreateQueueIfNecessary(UiCommandInputQueue);
			}
		}

		/// <summary>
		/// Re-queues a message for processing at another time.
		/// </summary>
		/// <param name="m">The message to process later.</param>
		/// <remarks>
		/// This method will place the message onto the back of the queue
		/// which may break message ordering.
		/// </remarks>
		public void ReceiveMessageLater(TransportMessage m)
		{
			if (!string.IsNullOrEmpty(InputQueue))
			{
				Send(m, InputQueue);
			}
		}

		public bool QueueIsNotEmpty()
		{
			return false;
		}

		/// <summary>
		/// Sends a message to the specified destination.
		/// </summary>
		/// <param name="m">The message to send.</param>
		/// <param name="destination">The address of the destination to send the message to.</param>
		public void Send(TransportMessage m, string destination)
		{
			var address = MsmqUtilities.GetFullPath(destination);

			using (var q = new MessageQueue(address, false, true, QueueAccessMode.Send))
			{
				var toSend = new Message();

				if (m.Body == null && m.BodyStream != null)
				{
					toSend.BodyStream = m.BodyStream;
				}
				else
				{
					MessageSerializer.Serialize(m.Body, toSend.BodyStream);
				}

				if (m.CorrelationId != null)
				{
					toSend.CorrelationId = m.CorrelationId;
				}

				toSend.Recoverable = m.Recoverable;

				if (!string.IsNullOrEmpty(m.ReturnAddress))
				{
					toSend.ResponseQueue = new MessageQueue(MsmqUtilities.GetFullPath(m.ReturnAddress), false, true);
				}

				toSend.Label = new MessageLabel(m.WindowsIdentityName, m.IdForCorrelation).ToString();

				if (m.TimeToBeReceived < MessageQueue.InfiniteTimeout)
				{
					toSend.TimeToBeReceived = m.TimeToBeReceived;
				}

				if (m.Headers != null && m.Headers.Count > 0)
				{
					using (var stream = new MemoryStream())
					{
						_headerSerializer.Serialize(stream, m.Headers);
						toSend.Extension = stream.GetBuffer();
					}
				}

				toSend.AppSpecific = (int) m.MessageIntent;

				try
				{
					int attempt = 0;
					while (true)
					{
						try
						{
							q.Send(toSend, GetTransactionTypeForSend());
							break;
						}
						catch (MessageQueueException sendingEx)
						{
							if (sendingEx.MessageQueueErrorCode == MessageQueueErrorCode.InsufficientResources
								&& attempt < SendAttemptCount)
							{
								Thread.Sleep(SendAttemptSleepIfFault);
								attempt++;
								continue;
							}

							throw;
						}
					}
				}
				catch (MessageQueueException ex)
				{
					if (ex.MessageQueueErrorCode == MessageQueueErrorCode.QueueNotFound)
					{
						throw new ConfigurationErrorsException("The destination queue '" + destination +
							"' could not be found. You may have misconfigured the destination for this kind of message (" +
							m.Body[0].GetType().FullName +
							") in the MessageEndpointMappings of the UnicastBusConfig section in your configuration file." +
							"It may also be the case that the given queue just hasn't been created yet, or has been deleted."
							, ex);
					}

					throw;
				}

				m.Id = toSend.Id;
			}
		}

		const int SendAttemptCount = 5;
		const int SendAttemptSleepIfFault = 500;

		#endregion

		#region helper methods

		private void HandleAsync(MessageEx message)
		{
			_messageId = string.Empty;
			ReceiveFromQueue(message, m => ThreadPool.QueueUserWorkItem(state =>
			{
				try
				{
					ProcessMessage(m);
				}
				catch (Exception e)
				{
					Logger.Warn(LoggerContext.New(message.MessageOrigin.Name), "Failed to process message.", e);
					OnFailedMessageProcessing(message);
				}
			}));
		}

		private void Handle(MessageEx message)
		{
			_needToAbort = false;
			_messageId = string.Empty;
			try
			{
				if (IsTransactional)
				{
					new TransactionWrapper().RunInTransaction(() => ReceiveFromQueue(message, ProcessMessage), IsolationLevel, TransactionTimeout);
					ClearFailuresForMessage(_messageId);
				}
				else
				{
					ReceiveFromQueue(message, ProcessMessage);
				}
			}
			catch (AbortHandlingCurrentMessageException)
			{
				//in case AbortHandlingCurrentMessage was called
				return;
			}
			catch (Exception e)
			{
				Logger.Warn(LoggerContext.New(message.MessageOrigin.Name), "Failed to process message.", e);
				if (IsTransactional)
				{
					IncrementFailuresForMessage(_messageId);
				}
				OnFailedMessageProcessing(message);
			}
		}

		private void ReceiveFromQueue(MessageEx message, Action<MessageEx> processMessageAction)
		{
			var m = message.Message;
			if (m == null)
			{
				Logger.Info(LoggerContext.New(message.MessageOrigin.Name), string.Format("Peek returned null message."));
				return;
			}

			message.DoReceive();
			processMessageAction(message);
		}

		private void ProcessMessage(MessageEx message)
		{
			var m = message.Message;
			_messageId = m.Id;
			if (IsTransactional)
			{
				if (HandledMaxRetries(m.Id))
				{
					Logger.Error(LoggerContext.New(message.MessageOrigin.Name),
						string.Format("Message has failed the maximum number of times allowed, ID={0}.", m.Id));
					MoveToErrorQueue(message);
					return;
				}
			}
			//exceptions here will cause a rollback - which is what we want.
			if (StartedMessageProcessing != null)
			{
				StartedMessageProcessing(this, null);
			}
			TransportMessage result = Convert(m);
			if (SkipDeserialization)
			{
				result.BodyStream = m.BodyStream;
			}
			else
			{
				try
				{
					result.Body = Extract(m);
				}
				catch (TypeNotFoundWhileDeserializationException e)
				{
					Logger.Warn(LoggerContext.New(message.MessageOrigin.Name, result), "Could not extract message data.", e);
					OnFinishedMessageProcessing(message);
					return;
				}
				catch (Exception e)
				{
					Logger.Error(LoggerContext.New(message.MessageOrigin.Name, result), "Could not extract message data.", e);
					MoveToErrorQueue(message);
					OnFinishedMessageProcessing(message); // don't care about failures here
					return;
				}
			}
			//care about failures here
			var exceptionNotThrown = OnTransportMessageReceived(result, message);
			//and here
			var otherExNotThrown = OnFinishedMessageProcessing(message);
			//but need to abort takes precedence - failures aren't counted here,
			//so messages aren't moved to the error queue.
			if (_needToAbort)
			{
				throw new AbortHandlingCurrentMessageException();
			}
			if (!(exceptionNotThrown && otherExNotThrown)) //cause rollback
			{
				throw new ApplicationException("Exception occured while processing message.");
			}
		}

		private bool HandledMaxRetries(string messageId)
		{
			_failuresPerMessageLocker.EnterReadLock();

			if (_failuresPerMessage.ContainsKey(messageId) &&
				(_failuresPerMessage[messageId] >= maxRetries))
			{
				_failuresPerMessageLocker.ExitReadLock();
				_failuresPerMessageLocker.EnterWriteLock();
				_failuresPerMessage.Remove(messageId);
				_failuresPerMessageLocker.ExitWriteLock();

				return true;
			}

			_failuresPerMessageLocker.ExitReadLock();
			return false;
		}

		private void ClearFailuresForMessage(string messageId)
		{
			_failuresPerMessageLocker.EnterReadLock();
			if (_failuresPerMessage.ContainsKey(messageId))
			{
				_failuresPerMessageLocker.ExitReadLock();
				_failuresPerMessageLocker.EnterWriteLock();
				_failuresPerMessage.Remove(messageId);
				_failuresPerMessageLocker.ExitWriteLock();
			}
			else
			{
				_failuresPerMessageLocker.ExitReadLock();
			}
		}

		private void IncrementFailuresForMessage(string messageId)
		{
			_failuresPerMessageLocker.EnterWriteLock();
			try
			{
				if (!_failuresPerMessage.ContainsKey(messageId))
				{
					_failuresPerMessage[messageId] = 1;
				}
				else
				{
					_failuresPerMessage[messageId] = _failuresPerMessage[messageId] + 1;
				}
			}
			finally
			{
				_failuresPerMessageLocker.ExitWriteLock();
			}
		}

		/// <summary>
		/// Moves the given message to the configured error queue.
		/// </summary>
		/// <param name="message"></param>
		protected void MoveToErrorQueue(MessageEx message)
		{
			var m = message.Message;
			m.Label = m.Label + string.Format("<{0}>{1}</{0}><{2}>{3}<{2}>", FAILEDQUEUE, message.MessageOrigin.Name, ORIGINALID, m.Id);
			if (_errorQueue != null)
			{
				_errorQueue.Send(m, MessageQueueTransactionType.Single);
			}
		}

		/// <summary>
		/// Causes the processing of the current message to be aborted.
		/// </summary>
		public void AbortHandlingCurrentMessage()
		{
			_needToAbort = true;
		}

		/// <summary>
		/// Converts an MSMQ <see cref="Message"/> into an NServiceBus message.
		/// </summary>
		/// <param name="m">The MSMQ message to convert.</param>
		/// <returns>An NServiceBus message.</returns>
		public TransportMessage Convert(Message m)
		{
			var result = new TransportMessage
			{
				Id = m.Id,
				CorrelationId =
					(m.CorrelationId == "00000000-0000-0000-0000-000000000000\\0"
						? null
						: m.CorrelationId),
				Recoverable = m.Recoverable,
				TimeToBeReceived = m.TimeToBeReceived,
				TimeSent = m.SentTime,
				ReturnAddress = MsmqUtilities.GetIndependentAddressForQueue(m.ResponseQueue),
				MessageIntent =
					Enum.IsDefined(typeof(MessageIntentEnum), m.AppSpecific)
						? (MessageIntentEnum) m.AppSpecific
						: MessageIntentEnum.Send
			};

			FillIdForCorrelationAndWindowsIdentity(result, m);

			if (string.IsNullOrEmpty(result.IdForCorrelation))
			{
				result.IdForCorrelation = result.Id;
			}

			if (m.Extension.Length > 0)
			{
				var stream = new MemoryStream(m.Extension);
				var o = _headerSerializer.Deserialize(stream);
				result.Headers = o as List<HeaderInfo>;
			}
			else
			{
				result.Headers = new List<HeaderInfo>();
			}

			return result;
		}

		/// <summary>
		/// Returns the queue whose process failed processing the given message
		/// by accessing the label of the message.
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		public static string GetFailedQueue(Message m)
		{
			if (!m.Label.Contains(FAILEDQUEUE))
			{
				return null;
			}

			var startIndex = m.Label.IndexOf(string.Format("<{0}>", FAILEDQUEUE)) + FAILEDQUEUE.Length + 2;
			var count = m.Label.IndexOf(string.Format("</{0}>", FAILEDQUEUE)) - startIndex;

			return MsmqUtilities.GetFullPath(m.Label.Substring(startIndex, count));
		}

		/// <summary>
		/// Gets the label of the message stripping out the failed queue.
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		public static string GetLabelWithoutFailedQueue(Message m)
		{
			if (!m.Label.Contains(FAILEDQUEUE))
			{
				return m.Label;
			}

			var startIndex = m.Label.IndexOf(string.Format("<{0}>", FAILEDQUEUE));
			var endIndex = m.Label.IndexOf(string.Format("</{0}>", FAILEDQUEUE));
			endIndex += FAILEDQUEUE.Length + 3;

			return m.Label.Remove(startIndex, endIndex - startIndex);
		}

		private static void FillIdForCorrelationAndWindowsIdentity(TransportMessage result, Message m)
		{
			var messageLabel = MessageLabel.Parse(m.Label);

			if (!string.IsNullOrEmpty(messageLabel.IdForCorrelation))
			{
				result.IdForCorrelation = messageLabel.IdForCorrelation;
			}

			if (!string.IsNullOrEmpty(messageLabel.WindowsIdentityName))
			{
				result.WindowsIdentityName = messageLabel.WindowsIdentityName;
			}
		}

		/// <summary>
		/// Extracts the messages from an MSMQ <see cref="Message"/>.
		/// </summary>
		/// <param name="message">The MSMQ message to extract from.</param>
		/// <returns>An array of handleable messages.</returns>
		private IMessage[] Extract(Message message)
		{
			return MessageSerializer.Deserialize(message.BodyStream);
		}

		/// <summary>
		/// Gets the transaction type to use when receiving a message from the queue.
		/// </summary>
		/// <returns>The transaction type to use.</returns>
		private MessageQueueTransactionType GetTransactionTypeForReceive()
		{
			return IsTransactional ? MessageQueueTransactionType.Automatic : MessageQueueTransactionType.None;
		}

		/// <summary>
		/// Gets the transaction type to use when sending a message.
		/// </summary>
		/// <returns>The transaction type to use.</returns>
		private MessageQueueTransactionType GetTransactionTypeForSend()
		{
			return (IsTransactional && Transaction.Current != null)
				? MessageQueueTransactionType.Automatic
				: MessageQueueTransactionType.Single;
		}

		private bool OnFinishedMessageProcessing(MessageEx message)
		{
			try
			{
				if (FinishedMessageProcessing != null)
				{
					FinishedMessageProcessing(this, null);
				}
			}
			catch (Exception e)
			{
				Logger.Error(LoggerContext.New(message.MessageOrigin.Name), "Failed raising 'finished message processing' event.", e);
				return false;
			}
			return true;
		}

		private bool OnTransportMessageReceived(TransportMessage msg, MessageEx origin)
		{
			try
			{
				if (TransportMessageReceived != null)
				{
					TransportMessageReceived(this, new TransportMessageReceivedEventArgs(msg));
				}

				Logger.Debug(LoggerContext.New(origin.MessageOrigin.Name, msg), "Transport message received");
			}
			catch (Exception e)
			{
				Logger.Warn(LoggerContext.New(origin.MessageOrigin.Name, msg),
					"Failed raising 'transport message received' event for message with ID=" + msg.Id, e);
				return false;
			}
			return true;
		}

		private bool OnFailedMessageProcessing(MessageEx message)
		{
			try
			{
				if (FailedMessageProcessing != null)
				{
					FailedMessageProcessing(this, null);
				}
			}
			catch (Exception e)
			{
				Logger.Warn(LoggerContext.New(message.MessageOrigin.Name), "Failed raising 'failed message processing' event.", e);
				return false;
			}
			return true;
		}

		private ILoggerContextSensitive Logger
		{
			get { return _logger; }
		}

		public void Dispose()
		{
			if (_inputQueueRouter != null)
			{
				_inputQueueRouter.Dispose();
			}
			if (_uiQueueRouter != null)
			{
				_uiQueueRouter.Dispose();
			}
		}

		#endregion

		private static readonly string FAILEDQUEUE = "FailedQ";
		private static readonly string ORIGINALID = "OriginalId";

		private IPluginQueue _queue;
		private MessageQueue _errorQueue;

		private readonly ILoggerContextSensitive _logger = new LoggerContextSensitive();
		private readonly ReaderWriterLockSlim _failuresPerMessageLocker = new ReaderWriterLockSlim();
		private readonly IDictionary<string, int> _failuresPerMessage = new Dictionary<string, int>();
		private readonly XmlSerializer _headerSerializer = new XmlSerializer(typeof(List<HeaderInfo>));
		[ThreadStatic] private static volatile bool _needToAbort;
		[ThreadStatic] private static volatile string _messageId;
		private IMessageConsumer<MessageEx> _inputQueueRouter;
		private IMessageConsumer<MessageEx> _uiQueueRouter;
	}
}
