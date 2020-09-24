using System;
using Tp.Integration.Messages.Entities.PermissionGroups;

namespace Tp.Integration.Messages.EntityLifecycle.Messages.PermissionGroup
{
    [Serializable]
    public class AclEntityPermissionGroupsCreatedMessage : EntityCreatedMessage<AclEntityPermissionGroupsDTO>, ISagaMessage
    {
    }

    [Serializable]
    public class AclEntityPermissionGroupsDeletedMessage : EntityDeletedMessage<AclEntityPermissionGroupsDTO>, ISagaMessage
    {
    }

    [Serializable]
    public class AclEntityPermissionGroupsUpdatedMessage : EntityUpdatedMessage<AclEntityPermissionGroupsDTO, AclEntityPermissionGroupsDTO.AclEntityPermissionsGroups>, ISagaMessage
    {
    }
}
