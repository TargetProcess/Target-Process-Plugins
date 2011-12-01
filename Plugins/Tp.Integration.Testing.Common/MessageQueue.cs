// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NServiceBus;
using NServiceBus.Serialization;
using NServiceBus.Unicast;
using NServiceBus.Unicast.Transport;
using StructureMap;
using Tp.Integration.Messages.ServiceBus;

namespace Tp.Integration.Testing.Common
{
	public class MessageQueue : Queue<SentMessage>
	{
		public MessageQueue(string queueName)
		{
			_name = queueName;
		}

		private readonly string _name;

		public IEnumerable<MessageInfo<TMessage>> GetMessageInfos<TMessage>() where TMessage : IMessage
		{
			var result = new List<MessageInfo<TMessage>>();

			foreach (var transportMessage in this.Where(transportMessage =>
			                                            transportMessage.Destination == _name ||
			                                            transportMessage.Destination == UnicastBus.GetUiQueueName(_name)))
			{
				result.AddRange(transportMessage.GetMessages<TMessage>());
			}
			return result.ToArray();
		}

		public TMessage[] GetMessages<TMessage>() where TMessage : IMessage
		{
			return GetMessageInfos<TMessage>().Select(x => x.Message).ToArray();
		}
	}

	public class MessageInfo<TMessage> where TMessage : IMessage
	{
		private readonly TMessage _message;
		private readonly TransportMessage _transportMessage;

		public MessageInfo(TMessage message, TransportMessage transportMessage)
		{
			_message = message;
			_transportMessage = transportMessage;
		}

		public TMessage Message
		{
			get { return _message; }
		}

		public Guid SagaId
		{
			get { return new Guid(_transportMessage.Headers.Where(x => x.Key == "SagaId").First().Value); }
		}

		public string AccountName
		{
			get { return _transportMessage.Headers.Where(x => x.Key == BusExtensions.ACCOUNTNAME_KEY).First().Value; }
		}

		public string ProfileName
		{
			get { return _transportMessage.Headers.Where(x => x.Key == BusExtensions.PROFILENAME_KEY).First().Value; }
		}
	}

	public class SentMessage
	{
		public string Destination { get; set; }
		private TransportMessage _msg;

		private static IMessageSerializer Serializer
		{
			get { return ObjectFactory.GetInstance<IMessageSerializer>(); }
		}

		public TransportMessage Message
		{
			get
			{
				_msg.BodyStream.Position = 0;
				_msg.Body = Serializer.Deserialize(_msg.BodyStream);
				return _msg;
			}
			set
			{
				value.BodyStream = new MemoryStream();
				Serializer.Serialize(value.Body, value.BodyStream);
				_msg = value;
			}
		}

		public IEnumerable<MessageInfo<TMessage>> GetMessages<TMessage>() where TMessage : IMessage
		{
			return Message.Messages.OfType<TMessage>().Select(message => new MessageInfo<TMessage>(message, Message));
		}
	}
}