using System;

namespace Tp.Integration.Messages.EntityLifecycle
{
    /// <summary>
    /// Marker interface to indicate that a class represents a message that can be involved into some saga(long-running process).
    /// </summary>
    public interface ISagaMessage : ITargetProcessMessage
    {
        Guid SagaId { get; set; }
    }
}
