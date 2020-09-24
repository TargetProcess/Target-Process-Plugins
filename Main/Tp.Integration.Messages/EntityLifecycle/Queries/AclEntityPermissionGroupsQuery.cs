using System;
using Tp.Integration.Messages.Entities.PermissionGroups;

namespace Tp.Integration.Messages.EntityLifecycle.Queries
{
    [Serializable]
    public class AclEntityPermissionGroupsQuery : PermissionGroupsQuery
    {
        [Serializable]
        public class Entity
        {
            public int Id { get; set; }
            public int EntityTypeId { get; set; }
        }

        public Entity[] Entities { get; set; }

        public override DtoType DtoType => new DtoType(typeof(AclEntityPermissionGroupsDTO));
    }

    [Serializable]
    public class AclEntityPermissionGroupsQueryResult : QueryResult<AclEntityPermissionGroupsDTO>, IPermissionGroupsQueryResult
    {
        public Guid CorrelationId { get; set; }
    }
}
