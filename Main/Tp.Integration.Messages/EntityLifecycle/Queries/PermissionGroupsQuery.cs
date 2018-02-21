using System;

namespace Tp.Integration.Messages.EntityLifecycle.Queries
{
    public abstract class PermissionGroupsQuery : QueryBase
    {
        public Guid CorrelationId { get; set; }
    }
}
