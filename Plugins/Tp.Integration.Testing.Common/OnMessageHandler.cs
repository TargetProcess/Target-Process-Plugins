// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using NServiceBus;
using NServiceBus.Unicast.Transport;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Messages.ServiceBus;

namespace Tp.Integration.Testing.Common
{
	internal class OnMessageHandler<TMessage> : IOnMessageHandler<TMessage>, IDisposable
		where TMessage : IMessage
	{
		public OnMessageHandler(TransportMock transport)
			: this(transport, x => true)
		{
		}

		public OnMessageHandler(TransportMock transport, Func<TMessage, bool> canBeHandled)
		{
			_transport = transport;
			_canBeHandled = canBeHandled;
			transport.PluginMessageSent += OnTransportMessageReceived;
		}

		private void OnTransportMessageReceived(object sender, TransportMessageReceivedEventArgs e)
		{
			if (_messageCreator == null)
				return;


			var messages = e.Message.Body.OfType<TMessage>();
			if (messages.Count() == 0)
			{
				return;
			}

			for (int i = 0; i < messages.Count(); i++)
			{
				var message = messages.ToArray()[i];

				if (!_canBeHandled(message))
					continue;

				if (e.Message.ReturnAddress == _transport.Address)
				{
					_transport.HandleLocalMessage(e.Message.Headers, MessageCreator(e, message));
				}
				else
				{
					_transport.HandleMessageFromTp(e.Message.Headers, MessageCreator(e, message));
				}
			}
		}

		private ISagaMessage[] MessageCreator(TransportMessageReceivedEventArgs e, TMessage message)
		{
			var headerValue = GetHeaderValue(e.Message, BusExtensions.SAGAID_KEY);
			var sagaId = headerValue == null ? Guid.Empty : new Guid(headerValue.Value);
			var result = _messageCreator(message);
			foreach (var sagaMessage in result)
			{
				sagaMessage.SagaId = sagaId;
			}
			return result;
		}

		private static HeaderInfo GetHeaderValue(TransportMessage transportMessage, string headerKey)
		{
			return transportMessage.Headers.Find(x => x.Key == headerKey);
		}

		private Func<TMessage, ISagaMessage[]> _messageCreator;
		private TransportMock _transport;
		private Func<TMessage, bool> _canBeHandled;
		private bool _disposed;

		public void Reply(Func<TMessage, ISagaMessage[]> createMessage)
		{
			_messageCreator = createMessage;
		}

		public void Reply(Func<TMessage, ISagaMessage> createMessage)
		{
			_messageCreator = message => new[] {createMessage(message)};
		}

		public void Dispose()
		{
			if (_disposed) return;
			
			_transport.PluginMessageSent -= OnTransportMessageReceived;
			_transport = null;
			_canBeHandled = null;
			_disposed = true;

			GC.SuppressFinalize(this);
		}
	}
}