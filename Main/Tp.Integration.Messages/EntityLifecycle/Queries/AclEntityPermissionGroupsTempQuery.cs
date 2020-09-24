using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Queries
{
    // TODO: Remove after completition of US#183198
    [Serializable]
    public class AclEntityPermissionGroupsTempQuery : PermissionGroupsQuery
    {
        public override DtoType DtoType => new DtoType(typeof(AclEntityPermissionGroupsTempDTO));
    }

    [Serializable]
    public class AclEntityPermissionGroupsTempQueryResult : QueryResult<AclEntityPermissionGroupsTempDTO>, IPermissionGroupsQueryResult
    {
        public Guid CorrelationId { get; set; }
    }
}
