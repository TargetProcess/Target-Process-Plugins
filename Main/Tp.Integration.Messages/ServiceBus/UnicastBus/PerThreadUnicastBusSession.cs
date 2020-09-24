using System;
using System.Collections.Generic;
using NServiceBus.Unicast.Transport;

namespace Tp.Integration.Messages.ServiceBus.UnicastBus
{
    public class PerThreadUnicastBusSession : IUnicastBusSession
    {
        [ThreadStatic]
        private static TransportMessage _currentMessage;

        [ThreadStatic]
        private static bool _shouldOnceSkipSendReadyMessage;

        [ThreadStatic]
        private static bool _isHandleCurrentMessageLater;

        [ThreadStatic]
        private static bool? _shouldContinueDispatchingCurrentMessageToHandlers;

        [ThreadStatic]
        private static IDictionary<string, string> _outgoingHeaders;

        public TransportMessage CurrentMessage
        {
            get => _currentMessage;
            set => _currentMessage = value;
        }

        public bool ShouldOnceSkipSendReadyMessage
        {
            get => _shouldOnceSkipSendReadyMessage;
            set => _shouldOnceSkipSendReadyMessage = value;
        }

        public bool IsHandleCurrentMessageLater
        {
            get => _isHandleCurrentMessageLater;
            set => _isHandleCurrentMessageLater = value;
        }

        public bool ShouldContinueDispatchingCurrentMessageToHandlers
        {
            get => _shouldContinueDispatchingCurrentMessageToHandlers ?? (_shouldContinueDispatchingCurrentMessageToHandlers = true).Value;
            set => _shouldContinueDispatchingCurrentMessageToHandlers = value;
        }

        public IDictionary<string, string> OutgoingMessageHeaders =>
            _outgoingHeaders ?? (_outgoingHeaders = new Dictionary<string, string>());
    }
}
