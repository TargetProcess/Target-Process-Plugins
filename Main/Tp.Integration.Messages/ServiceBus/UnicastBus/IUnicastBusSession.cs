using System.Collections.Concurrent;
using System.Collections.Generic;
using NServiceBus.Unicast;
using NServiceBus.Unicast.Transport;
using Tp.Core.Annotations;

namespace Tp.Integration.Messages.ServiceBus.UnicastBus
{
    /// <summary>
    /// Persists message exchange session for <see cref="TpUnicastBus"/>.
    /// </summary>
    public interface IUnicastBusSession
    {
        /// <summary>
        /// Current processing message.
        /// </summary>
        [CanBeNull]
        TransportMessage CurrentMessage { get; set; }

        /// <summary>
        /// Should sending of ready message be skipped one time or not.
        /// </summary>
        bool ShouldOnceSkipSendReadyMessage { get; set; }

        /// <summary>
        /// Is current message marked to be handled later.
        /// </summary>
        bool IsHandleCurrentMessageLater { get; set; }

        /// <summary>
        /// Should continue dispatching current message to handlers or not.
        /// </summary>
        bool ShouldContinueDispatchingCurrentMessageToHandlers { get; set; }

        /// <summary>
        /// Headers for outgoing message.
        /// </summary>
        [NotNull]
        IDictionary<string, string> OutgoingMessageHeaders { get; }
    }
}
