// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Messaging;
using System.Security.Principal;
using System.Threading;
using System.Transactions;
using System.Xml.Serialization;
using NServiceBus;
using NServiceBus.Serialization;
using NServiceBus.Unicast;
using NServiceBus.Unicast.Transport;
using NServiceBus.Unicast.Transport.Msmq;
using NServiceBus.Utils;
using Tp.Integration.Messages.ServiceBus.Transport.Router;
using Tp.Integration.Messages.ServiceBus.UnicastBus;
using log4net;

namespace Tp.Integration.Messages.ServiceBus.Transport.UiPriority
{
	/// <summary>
	/// An MSMQ implementation of <see cref="ITransport"/> for use with
	/// NServiceBus.
	/// </summary>
	/// <remarks>
	/// A transport is used by NServiceBus as a high level abstraction from the 
	/// underlying messaging service being used to transfer messages.
	/// </remarks>
	public class MsmqUiPriorityTransport : IMsmqTransport
	{
		// This constant is used for message prioritization. We search queue for messages with this correlation id and treat them as high priority. 
		// Correlation id is NOT used for purposes it was designed by Microsoft guys.
		//public const string HighPriorityMessageCorrelationId = "b4a0874e-6a92-4920-8e29-bea29a8ff361\\1522841";
		//public const int MessageCountWhenUiMessagesStopPrioritizing = 10000;

		#region config info

		/// <summary>
		/// The path to the queue the transport will read from.
		/// Only specify the name of the queue - msmq specific address not required.
		/// When using MSMQ v3, only local queues are supported.
		/// </summary>
		public string InputQueue { get; set; }

		public string UICommandInputQueue
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

		public string GetQueueName(string accountName)
		{
			return InputQueue;
		}

		public bool TryDeleteQueue(string accountName)
		{
			throw new NotSupportedException();
		}

		public bool TryDeleteUiQueue(string accountName)
		{
			throw new NotSupportedException();
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

		private int secondsToWaitForMessage = 1;

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
		public virtual int NumberOfWorkerThreads
		{
			get
			{
				lock (workerThreads)
					return workerThreads.Count;
			}
			set { numberOfWorkerThreads = value; }
		}

		private int numberOfWorkerThreads;


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
			lock (workerThreads)
			{
				var current = workerThreads.Count;

				if (targetNumberOfWorkerThreads == current)
				{
					return;
				}

				if (targetNumberOfWorkerThreads < current)
				{
					for (var i = targetNumberOfWorkerThreads; i < current; i++)
					{
						workerThreads[i].Stop();
					}

					return;
				}

				if (targetNumberOfWorkerThreads > current)
				{
					for (var i = current; i < targetNumberOfWorkerThreads; i++)
					{
						AddWorkerThread(Process).Start();
					}

					return;
				}
			}
		}

		/// <summary>
		/// Starts the transport.
		/// </summary>
		public void Start()
		{
			CheckConfiguration();
			CreateQueuesIfNecessary();

			if (ErrorQueue != null)
			{
				errorQueue = new MessageQueue(MsmqUtilities.GetFullPath(ErrorQueue));
			}

			if (!string.IsNullOrEmpty(InputQueue))
			{
				_queue = PluginQueueFactory.Create(InputQueue);
				_uiCommandQueue = PluginQueueFactory.Create(UICommandInputQueue);

				if (PurgeOnStartup)
				{
					_queue.Purge();
					_uiCommandQueue.Purge();
				}

				for (var i = 0; i < numberOfWorkerThreads; i++)
				{
					AddWorkerThread(Process).Start();
				}
			}
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
				MsmqUtilities.CreateQueueIfNecessary(UICommandInputQueue);
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

				FillLabel(toSend, m);

				if (m.TimeToBeReceived < MessageQueue.InfiniteTimeout)
				{
					toSend.TimeToBeReceived = m.TimeToBeReceived;
				}

				if (m.Headers != null && m.Headers.Count > 0)
				{
					var sourceOutputheader = m.Headers.FirstOrDefault(h => h.Key == TpUnicastBus.SourceQueue);
					if (sourceOutputheader != null)
					{
						m.Headers.Remove(sourceOutputheader);
					}

					using (var stream = new MemoryStream())
					{
						headerSerializer.Serialize(stream, m.Headers);
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
						                                       "' could not be found. " +
						                                       "It may also be the case that the given queue just hasn't been created yet, or has been deleted."
						                                       , ex);
					}

					throw;
				}

				m.Id = toSend.Id;
			}
		}

		/// <summary>
		/// Returns the number of messages in the queue.
		/// </summary>
		/// <returns></returns>
		public bool QueueIsNotEmpty()
		{
			return MessageInQueue(_queue);
			//var qMgmt = new MSMQManagementClass();
			//object machine = Environment.MachineName;
			//var missing = Type.Missing;
			//object formatName = _queue.FormatName;

			//qMgmt.Init(ref machine, ref missing, ref formatName);
			//return qMgmt.MessageCount;
		}

		#endregion

		#region helper methods

		private WorkerThread AddWorkerThread(Action action)
		{
			lock (workerThreads)
			{
				var result = new WorkerThread(action);

				workerThreads.Add(result);

				result.Stopped += delegate(object sender, EventArgs e)
				                  	{
				                  		var wt = sender as WorkerThread;
				                  		lock (workerThreads)
				                  			workerThreads.Remove(wt);
				                  	};

				return result;
			}
		}

		/// <summary>
		/// Waits for a message to become available on the input queue
		/// and then receives it.
		/// </summary>
		/// <remarks>
		/// If the queue is transactional the receive operation will be wrapped in a 
		/// transaction.
		/// </remarks>
		public void Process()
		{
			if (MessageInQueue(_uiCommandQueue))
			{
				ReceiveAndProcessMessage(_uiCommandQueue);
			}
			else if (MessageInQueue(_queue))
			{
				ReceiveAndProcessMessage(_queue);
			}
			else
			{
				if (WaitForMessageInQueue(_uiCommandQueue))
				{
					ReceiveAndProcessMessage(_uiCommandQueue);
				}
			}
		}

		private bool MessageInQueue(IPluginQueue queue)
		{
			return MessageInQueueInternal(queue, 0);
		}

		private bool WaitForMessageInQueue(IPluginQueue queue)
		{
			return MessageInQueueInternal(queue, SecondsToWaitForMessage);
		}

		private void ReceiveAndProcessMessage(IPluginQueue queue)
		{
			_needToAbort = false;
			_messageId = string.Empty;

			try
			{
				if (IsTransactional)
				{
					new TransactionWrapper().RunInTransaction(ReceiveFromQueue, queue, IsolationLevel, TransactionTimeout);
				}
				else
				{
					ReceiveFromQueue(queue);
				}

				ClearFailuresForMessage(_messageId);
			}
			catch (AbortHandlingCurrentMessageException)
			{
				//in case AbortHandlingCurrentMessage was called
				return;
			}
			catch
			{
				if (IsTransactional)
				{
					IncrementFailuresForMessage(_messageId);
				}

				OnFailedMessageProcessing();
			}
		}

		/// <summary>
		/// Receives a message from the input queue.
		/// </summary>
		/// <remarks>
		/// If a message is received the <see cref="TransportMessageReceived"/> event will be raised.
		/// </remarks>
		private void ReceiveFromQueue(IPluginQueue queue)
		{
			var m = ReceiveMessageFromQueueAfterPeekWasSuccessful(queue);
			if (m == null)
			{
				return;
			}
			_messageId = m.Id;
			if (IsTransactional)
			{
				if (HandledMaxRetries(m.Id))
				{
					Logger.Error(string.Format("Message has failed the maximum number of times allowed, ID={0}.", m.Id));
					MoveToErrorQueue(m, queue);
					return;
				}
			}

			//exceptions here will cause a rollback - which is what we want.
			if (StartedMessageProcessing != null)
			{
				StartedMessageProcessing(this, null);
			}

			var result = Convert(m);
			var sourceHeader = result.Headers.FirstOrDefault(h => h.Key == TpUnicastBus.SourceQueue);
			if (sourceHeader == null)
			{
				result.Headers.Add(new HeaderInfo
						{
							Key = TpUnicastBus.SourceQueue,
							Value = queue.IndependentAddressForQueue
						});
			}
			else
			{
				sourceHeader.Value = queue.IndependentAddressForQueue;
			}

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
				catch (Exception e)
				{
					Logger.Error("Could not extract message data.", e);

					MoveToErrorQueue(m, queue);

					OnFinishedMessageProcessing(); // don't care about failures here
					return; // deserialization failed - no reason to try again, so don't throw
				}
			}

			//care about failures here
			var exceptionNotThrown = OnTransportMessageReceived(result);
			//and here
			var otherExNotThrown = OnFinishedMessageProcessing();

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
			failuresPerMessageLocker.EnterReadLock();

			if (failuresPerMessage.ContainsKey(messageId) &&
			    (failuresPerMessage[messageId] >= maxRetries))
			{
				failuresPerMessageLocker.ExitReadLock();
				failuresPerMessageLocker.EnterWriteLock();
				failuresPerMessage.Remove(messageId);
				failuresPerMessageLocker.ExitWriteLock();

				return true;
			}

			failuresPerMessageLocker.ExitReadLock();
			return false;
		}

		private void ClearFailuresForMessage(string messageId)
		{
			failuresPerMessageLocker.EnterReadLock();
			if (failuresPerMessage.ContainsKey(messageId))
			{
				failuresPerMessageLocker.ExitReadLock();
				failuresPerMessageLocker.EnterWriteLock();
				failuresPerMessage.Remove(messageId);
				failuresPerMessageLocker.ExitWriteLock();
			}
			else
			{
				failuresPerMessageLocker.ExitReadLock();
			}
		}

		private void IncrementFailuresForMessage(string messageId)
		{
			failuresPerMessageLocker.EnterWriteLock();
			try
			{
				if (!failuresPerMessage.ContainsKey(messageId))
				{
					failuresPerMessage[messageId] = 1;
				}
				else
				{
					failuresPerMessage[messageId] = failuresPerMessage[messageId] + 1;
				}
			}
			finally
			{
				failuresPerMessageLocker.ExitWriteLock();
			}
		}

		[DebuggerNonUserCode] // so that exceptions don't interfere with debugging.
		private bool MessageInQueueInternal(IPluginQueue queue, int waitInterval)
		{
			try
			{
				queue.Peek(TimeSpan.FromSeconds(waitInterval));
				return true;
			}
			catch (MessageQueueException mqe)
			{
				if (mqe.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
				{
					return false;
				}

				if (mqe.MessageQueueErrorCode == MessageQueueErrorCode.AccessDenied)
				{
					Logger.Fatal(
						string.Format(
							"Do not have permission to access queue [{0}]. Make sure that the current user [{1}] has permission to Send, Receive, and Peek  from this queue. NServiceBus will now exit.",
							Address, WindowsIdentity.GetCurrent().Name));
					Thread.Sleep(10000); //long enough for someone to notice
					System.Diagnostics.Process.GetCurrentProcess().Kill();
				}

				if (mqe.MessageQueueErrorCode == MessageQueueErrorCode.ServiceNotAvailable ||
				    mqe.MessageQueueErrorCode == MessageQueueErrorCode.OperationCanceled)
				{
					//this exceptions occur after windows restart. This is normal situation.
					Logger.Warn(
						"Problem in peeking a message from queue: " +
						Enum.GetName(typeof (MessageQueueErrorCode), mqe.MessageQueueErrorCode), mqe);

					Thread.Sleep(100);
				}
				else
				{
					Logger.Error(
						"Problem in peeking a message from queue: " +
						Enum.GetName(typeof (MessageQueueErrorCode), mqe.MessageQueueErrorCode), mqe);
				}

				return false;
			}
			catch (ObjectDisposedException)
			{
				Logger.Fatal("Queue has been disposed. Cannot continue operation. Please restart this process.");
				return false;
			}
			catch (Exception e)
			{
				Logger.Error("Error in peeking a message from queue.", e);
				return false;
			}
		}

		[DebuggerNonUserCode] // so that exceptions don't interfere with debugging.
		private Message ReceiveMessageFromQueueAfterPeekWasSuccessful(IPluginQueue queue)
		{
			try
			{
				//var messageCount = GetNumberOfPendingMessages();
				//if (messageCount < MessageCountWhenUiMessagesStopPrioritizing)
				//{
				//    try
				//    {
				//        return queue.ReceiveByCorrelationId(HighPriorityMessageCorrelationId);
				//    }
				//    catch (Exception)
				//    {
				//        //pass, receive main message now.
				//    }
				//}

				return queue.Receive(TimeSpan.FromSeconds(SecondsToWaitForMessage), GetTransactionTypeForReceive());
			}
			catch (MessageQueueException mqe)
			{
				if (mqe.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
				{
					return null;
				}

				Logger.Error(
					"Problem in receiving message from queue: " +
					Enum.GetName(typeof (MessageQueueErrorCode), mqe.MessageQueueErrorCode), mqe);
				return null;
			}
			catch (ObjectDisposedException)
			{
				Logger.Fatal("Queue has been disposed. Cannot continue operation. Please restart this process.");
				return null;
			}
			catch (Exception e)
			{
				Logger.Error("Error in receiving message from queue.", e);
				return null;
			}
		}

		/// <summary>
		/// Moves the given message to the configured error queue.
		/// </summary>
		/// <param name="m"></param>
		/// <param name="queue"></param>
		protected void MoveToErrorQueue(Message m, IPluginQueue queue)
		{
			m.Label = m.Label +
			          string.Format("<{0}>{1}</{0}><{2}>{3}<{2}>", FAILEDQUEUE,
			                        queue.IndependentAddressForQueue, ORIGINALID, m.Id);

			if (errorQueue != null)
			{
				errorQueue.Send(m, MessageQueueTransactionType.Single);
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
			             			Enum.IsDefined(typeof (MessageIntentEnum), m.AppSpecific)
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
				var o = headerSerializer.Deserialize(stream);
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
			if (m.Label == null)
			{
				return null;
			}

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
			if (m.Label == null)
			{
				return null;
			}

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
			if (m.Label == null)
			{
				return;
			}

			if (m.Label.Contains(IDFORCORRELATION))
			{
				int idStartIndex = m.Label.IndexOf(string.Format("<{0}>", IDFORCORRELATION)) + IDFORCORRELATION.Length + 2;
				int idCount = m.Label.IndexOf(string.Format("</{0}>", IDFORCORRELATION)) - idStartIndex;

				result.IdForCorrelation = m.Label.Substring(idStartIndex, idCount);
			}

			if (m.Label.Contains(WINDOWSIDENTITYNAME))
			{
				int winStartIndex = m.Label.IndexOf(string.Format("<{0}>", WINDOWSIDENTITYNAME)) + WINDOWSIDENTITYNAME.Length + 2;
				int winCount = m.Label.IndexOf(string.Format("</{0}>", WINDOWSIDENTITYNAME)) - winStartIndex;

				result.WindowsIdentityName = m.Label.Substring(winStartIndex, winCount);
			}
		}

		private static void FillLabel(Message toSend, TransportMessage m)
		{
			toSend.Label = string.Format("<{0}>{2}</{0}><{1}>{3}</{1}>", IDFORCORRELATION, WINDOWSIDENTITYNAME,
			                             m.IdForCorrelation, m.WindowsIdentityName);
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
			if (IsTransactional)
			{
				return Transaction.Current != null ? MessageQueueTransactionType.Automatic : MessageQueueTransactionType.Single;
			}

			return MessageQueueTransactionType.Single;
		}

		private bool OnFinishedMessageProcessing()
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
				Logger.Error("Failed raising 'finished message processing' event.", e);
				return false;
			}

			return true;
		}

		private bool OnTransportMessageReceived(TransportMessage msg)
		{
			try
			{
				if (TransportMessageReceived != null)
				{
					TransportMessageReceived(this, new TransportMessageReceivedEventArgs(msg));
				}
			}
			catch (Exception e)
			{
				Logger.Warn("Failed raising 'transport message received' event for message with ID=" + msg.Id, e);
				return false;
			}

			return true;
		}

		private bool OnFailedMessageProcessing()
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
				Logger.Warn("Failed raising 'failed message processing' event.", e);
				return false;
			}

			return true;
		}

		#endregion

		#region members

		private static readonly string IDFORCORRELATION = "CorrId";
		private static readonly string WINDOWSIDENTITYNAME = "WinIdName";
		private static readonly string FAILEDQUEUE = "FailedQ";
		private static readonly string ORIGINALID = "OriginalId";

		private IPluginQueue _queue;
		private IPluginQueue _uiCommandQueue;
		private MessageQueue errorQueue;
		private readonly IList<WorkerThread> workerThreads = new List<WorkerThread>();

		private readonly ReaderWriterLockSlim failuresPerMessageLocker = new ReaderWriterLockSlim();

		/// <summary>
		/// Accessed by multiple threads - lock using failuresPerMessageLocker.
		/// </summary>
		private readonly IDictionary<string, int> failuresPerMessage = new Dictionary<string, int>();

		[ThreadStatic] private static volatile bool _needToAbort;

		[ThreadStatic] private static volatile string _messageId;

		private static readonly ILog Logger = LogManager.GetLogger(typeof (MsmqTransport));

		private readonly XmlSerializer headerSerializer = new XmlSerializer(typeof (List<HeaderInfo>));

		#endregion

		#region IDisposable Members

		/// <summary>
		/// Stops all worker threads and disposes the MSMQ queue.
		/// </summary>
		public void Dispose()
		{
			lock (workerThreads)
				for (var i = 0; i < workerThreads.Count; i++)
				{
					workerThreads[i].Stop();
				}
		}

		#endregion

		const int SendAttemptCount = 5;
		const int SendAttemptSleepIfFault = 500;
	}

	}