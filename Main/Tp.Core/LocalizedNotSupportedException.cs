using System;
using Tp.I18n;

namespace Tp.Core
{
    public class LocalizedNotSupportedException : NotSupportedException, IFormattedMessageContainer
    {
        public LocalizedNotSupportedException() : this(DefaultMessage)
        {
        }

        public LocalizedNotSupportedException(IFormattedMessage formattedMessage) : base(formattedMessage.Value)
        {
            FormattedMessage = formattedMessage;
        }

        public IFormattedMessage FormattedMessage { get; }

        public static IFormattedMessage DefaultMessage => "Operation is not supported. You need to do this another way.".Localize();
    }
}
