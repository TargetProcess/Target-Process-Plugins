using System;
using Tp.Integration.Messages.Entities.PermissionGroups;

namespace Tp.Integration.Messages.EntityLifecycle.Queries
{
    [Serializable]
    public class AclUserPermissionGroupsQuery : PermissionGroupsQuery
    {
        public int[] UserIds { get; set; }

        public override DtoType DtoType => new DtoType(typeof(AclUserPermissionGroupsDTO));
    }

    [Serializable]
    public class AclUserPermissionGroupsQueryResult : QueryResult<AclUserPermissionGroupsDTO>, IPermissionGroupsQueryResult
    {
        public Guid CorrelationId { get; set; }
    }
}
