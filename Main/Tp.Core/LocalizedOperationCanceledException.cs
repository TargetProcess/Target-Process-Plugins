using System;
using Tp.I18n;

namespace Tp.Core
{
    public class LocalizedOperationCanceledException : InvalidOperationException, IFormattedMessageContainer
    {
        public LocalizedOperationCanceledException(IFormattedMessage formattedMessage)
            : base(formattedMessage.Value)
        {
            FormattedMessage = formattedMessage;
        }

        public LocalizedOperationCanceledException(IFormattedMessage formattedMessage, Exception innerException)
            : base(formattedMessage.Value, innerException)
        {
            FormattedMessage = formattedMessage;
        }

        public IFormattedMessage FormattedMessage { get; }

        public static IFormattedMessage DefaultMessage => "Operation is cancelled because of server processing timeout or client cancellation.".Localize();
    }
}
