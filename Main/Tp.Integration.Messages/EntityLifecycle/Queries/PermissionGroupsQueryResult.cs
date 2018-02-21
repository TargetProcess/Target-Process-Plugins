using System;

namespace Tp.Integration.Messages.EntityLifecycle.Queries
{
    public interface IPermissionGroupsQueryResult : ISagaMessage
    {
        Guid CorrelationId { get; set; }
    }
}
