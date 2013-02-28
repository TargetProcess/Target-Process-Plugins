// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Diagnostics;
using System.Messaging;
using NServiceBus.Utils;
using Tp.Integration.Messages.ServiceBus.Transport.Router.Log;
using log4net;

namespace Tp.Integration.Messages.ServiceBus.Transport
{
	public class PluginQueue : IPluginQueue
	{
		private readonly MessageQueue _queue;

		public PluginQueue(string queueName)
		{
			var q = new MessageQueue(MsmqUtilities.GetFullPath(queueName), false, true);
			_queue = InitializeQueue(q);
			_queueName = queueName;
		}

		private readonly string _queueName;

		public string Name
		{
			get { return _queueName; }
		}

		private static MessageQueue InitializeQueue(MessageQueue q)
		{
			bool transactional;
			try
			{
				transactional = q.Transactional;
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException(
					string.Format("There is a problem with the input queue given: {0}. See the enclosed exception for details.", q.Path),
					ex);
			}

			if (!transactional)
			{
				throw new ArgumentException("Queue must be transactional (" + q.Path + ").");
			}

			var mpf = new MessagePropertyFilter();
			mpf.SetAll();

			q.MessageReadPropertyFilter = mpf;
			return q;
		}

		public void Purge()
		{
			_queue.Purge();
		}

		public void Delete()
		{
			MessageQueue.Delete(_queue.Path);
		}

		public static bool TryDeleteQueue(string queueName, ILoggerContextSensitive log)
		{
			try
			{
				var queue = new PluginQueue(queueName);
				queue.Delete();
				return true;
			}
			catch(Exception e)
			{
				log.Warn(LoggerContext.New(queueName), "Failed to delete queue {0}. Queue does not exist or no permissions".Fmt(queueName), e);
				return false;
			}
		}

		public string FormatName
		{
			get { return _queue.FormatName; }
		}

		[DebuggerNonUserCode] // so that exceptions don't interfere with debugging.
		public void Peek(TimeSpan fromSeconds)
		{
			_queue.Peek(fromSeconds);
		}

		[DebuggerNonUserCode] // so that exceptions don't interfere with debugging.
		public Message Receive(TimeSpan fromSeconds, MessageQueueTransactionType transactionTypeForReceive)
		{
			return _queue.Receive(fromSeconds, transactionTypeForReceive);
		}

		public string IndependentAddressForQueue
		{
			get { return MsmqUtilities.GetIndependentAddressForQueue(_queue); }
		}
	}

	public interface IPluginQueue
	{
		void Purge();
		string FormatName { get; }
		string Name { get; }
		string IndependentAddressForQueue { get; }
		void Peek(TimeSpan fromSeconds);
		Message Receive(TimeSpan fromSeconds, MessageQueueTransactionType transactionTypeForReceive);
	}
}