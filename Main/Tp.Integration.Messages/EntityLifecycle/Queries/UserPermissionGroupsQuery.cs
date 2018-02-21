using System;
using Tp.Integration.Messages.Entities.PermissionGroups;

namespace Tp.Integration.Messages.EntityLifecycle.Queries
{
    [Serializable]
    public class UserPermissionGroupsQuery : PermissionGroupsQuery
    {
        public int[] UserIds { get; set; }

        public override DtoType DtoType => new DtoType(typeof(UserPermissionGroupsDTO));
    }

    [Serializable]
    public class UserPermissionGroupsQueryResult : QueryResult<UserPermissionGroupsDTO>, IPermissionGroupsQueryResult
    {
        public Guid CorrelationId { get; set; }
    }
}
