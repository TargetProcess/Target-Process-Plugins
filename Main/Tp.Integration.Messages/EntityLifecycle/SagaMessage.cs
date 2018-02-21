using System;

namespace Tp.Integration.Messages.EntityLifecycle
{
    /// <summary>
    /// Base class for message that can be involved in saga.
    /// </summary>
    [Serializable]
    public class SagaMessage
    {
        public SagaMessage()
        {
            SagaId = Guid.Empty;
        }

        public Guid SagaId { get; set; }
    }
}
