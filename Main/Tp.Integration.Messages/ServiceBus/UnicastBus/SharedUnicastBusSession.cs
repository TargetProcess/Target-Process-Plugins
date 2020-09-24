using System.Collections.Generic;
using NServiceBus.Unicast.Transport;

namespace Tp.Integration.Messages.ServiceBus.UnicastBus
{
    public class SharedUnicastBusSession : IUnicastBusSession
    {
        private readonly SharedUnicastBusSessionContextHolder _sessionContextHolder;

        public SharedUnicastBusSession(SharedUnicastBusSessionContextHolder sessionContextHolder)
        {
            _sessionContextHolder = sessionContextHolder;
        }

        public TransportMessage CurrentMessage
        {
            get => Context.GetOrAdd(MakeContextKey(nameof(CurrentMessage)), k => default(TransportMessage));
            set => Context.Set(MakeContextKey(nameof(CurrentMessage)), value);
        }

        public bool ShouldOnceSkipSendReadyMessage
        {
            get => Context.GetOrAdd(MakeContextKey(nameof(ShouldOnceSkipSendReadyMessage)), k => false);
            set => Context.Set(MakeContextKey(nameof(ShouldOnceSkipSendReadyMessage)), value);
        }

        public bool IsHandleCurrentMessageLater
        {
            get => Context.GetOrAdd(MakeContextKey(nameof(IsHandleCurrentMessageLater)), k => false);
            set => Context.Set(MakeContextKey(nameof(IsHandleCurrentMessageLater)), value);
        }

        public bool ShouldContinueDispatchingCurrentMessageToHandlers
        {
            get => Context.GetOrAdd(MakeContextKey(nameof(ShouldContinueDispatchingCurrentMessageToHandlers)), k => true);
            set => Context.Set(MakeContextKey(nameof(ShouldContinueDispatchingCurrentMessageToHandlers)), value);
        }

        public IDictionary<string, string> OutgoingMessageHeaders =>
            Context.GetOrAdd(MakeContextKey(nameof(OutgoingMessageHeaders)), _ => new Dictionary<string, string>());

        private string MakeContextKey(string keyPart) => $"Tp.Integration.Messages.{nameof(SharedUnicastBusSession)}:{keyPart}";

        // Context should be resolved per call.
        private SharedUnicastBusSessionContext Context => _sessionContextHolder.Get();
    }
}
