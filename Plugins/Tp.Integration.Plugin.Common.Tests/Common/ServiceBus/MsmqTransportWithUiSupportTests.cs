// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using NUnit.Framework;
using Tp.Integration.Messages.ServiceBus.Transport;
using Tp.Integration.Messages.ServiceBus.Transport.UiPriority;
using Tp.Testing.Common.NUnit;

namespace Tp.Integration.Plugin.Common.Tests.Common.ServiceBus
{
	[TestFixture]
	public class MsmqTransportWithUiSupportTests
	{
		[Test]
		public void ShouldProcessUiMessagesFirst()
		{
			var pluginQueueFactory = new PluginQueueFactoryMock();
			var transport = CreateTransport(pluginQueueFactory);

			var messages = new List<Message> {new Message {Body = "Message1"}, new Message("Message2")};
			var uiMessages = new List<Message> {new Message {Body = "UiMessage1"}, new Message("UiMessage2")};

			pluginQueueFactory.SetMessagesForQueue(transport.InputQueue, messages);
			pluginQueueFactory.SetMessagesForQueue(transport.UICommandInputQueue, uiMessages);

			transport.Start();
			var totalMessageCount = messages.Count + uiMessages.Count;
			for (var i = 0; i < totalMessageCount; i++)
			{
				transport.Process();
			}

			pluginQueueFactory.ProcessedMessages.Select(x => x.Body as string).ToArray().Should(
				Be.EquivalentTo(new[] {"UiMessage1", "UiMessage2", "Message1", "Message2"}));
		}

		private static MsmqUiPriorityTransport CreateTransport(PluginQueueFactoryMock pluginQueueFactory)
		{
			var transport = new MsmqUiPriorityTransport
			                	{
			                		IsTransactional = false,
			                		InputQueue = "InputQueue",
			                		SkipDeserialization = true,
			                		DoNotCreateQueues = true,
			                		NumberOfWorkerThreads = 0,
			                		PluginQueueFactory = pluginQueueFactory
			                	};
			return transport;
		}

		[Test]
		public void ShouldProcessUiMessageWhenItArrives()
		{
			var pluginQueueFactory = new PluginQueueFactoryMock();
			var transport = CreateTransport(pluginQueueFactory);

			var messages = new List<Message> {new Message {Body = "Message1"}, new Message("Message2")};
			var uiMessages = new List<Message>();

			pluginQueueFactory.SetMessagesForQueue(transport.InputQueue, messages);
			pluginQueueFactory.SetMessagesForQueue(transport.UICommandInputQueue, uiMessages);

			transport.Start();
			transport.Process();
			uiMessages.Add(new Message {Body = "UiMessage1"});
			transport.Process();
			transport.Process();

			pluginQueueFactory.ProcessedMessages.Select(x => x.Body as string).ToArray().Should(
				Be.EquivalentTo(new[] {"Message1", "UiMessage1", "Message2"}));
		}
	}

	public class PluginQueueFactoryMock : IPluginQueueFactory
	{
		private readonly Dictionary<string, List<Message>> _messages = new Dictionary<string, List<Message>>();
		private readonly List<Message> _processedMessages = new List<Message>();

		public List<Message> ProcessedMessages
		{
			get { return _processedMessages; }
		}

		public IPluginQueue Create(string queueName)
		{
			return new PluginQueueMock(queueName, _messages[queueName], ProcessedMessages);
		}

		public void SetMessagesForQueue(string queueName, List<Message> messages)
		{
			_messages[queueName] = messages;
		}
	}

	public class PluginQueueMock : IPluginQueue
	{
		private readonly string _queueName;
		private readonly List<Message> _messages;
		private readonly List<Message> _processedMessages;

		public PluginQueueMock(string queueName, List<Message> messages, List<Message> processedMessages)
		{
			_queueName = queueName;
			_messages = messages;
			_processedMessages = processedMessages;
		}

		public List<Message> Messages
		{
			get { return _messages; }
		}

		public void Purge()
		{
			throw new NotImplementedException();
		}

		public string FormatName
		{
			get { throw new NotImplementedException(); }
		}

		public void Peek(TimeSpan fromSeconds)
		{
			if (!_messages.Any())
			{
				throw new Exception("Queue is empty");
			}
		}

		public Message Receive(TimeSpan fromSeconds, MessageQueueTransactionType transactionTypeForReceive)
		{
			var message = _messages.First();
			_messages.Remove(message);
			_processedMessages.Add(message);
			return message;
		}

		public string Name
		{
			get { return _queueName; }
		}

		public string IndependentAddressForQueue
		{
			get { return _queueName; }
		}

	}
}