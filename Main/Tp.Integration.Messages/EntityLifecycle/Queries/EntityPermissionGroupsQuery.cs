using System;
using Tp.Integration.Messages.Entities.PermissionGroups;

namespace Tp.Integration.Messages.EntityLifecycle.Queries
{
    [Serializable]
    public class EntityPermissionGroupsQuery : PermissionGroupsQuery
    {
        [Serializable]
        public class Entity
        {
            public int Id { get; set; }
            public string ResourceType { get; set; }
            public int? ResourceTypeId { get; set; }
        }

        public Entity[] Entities { get; set; }

        public override DtoType DtoType => new DtoType(typeof(EntityPermissionGroupsDTO));
    }

    [Serializable]
    public class EntityPermissionGroupsQueryResult : QueryResult<EntityPermissionGroupsDTO>, IPermissionGroupsQueryResult
    {
        public Guid CorrelationId { get; set; }
    }
}
