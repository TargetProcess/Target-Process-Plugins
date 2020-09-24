using System;
using Tp.Integration.Messages.Entities.PermissionGroups;

namespace Tp.Integration.Messages.EntityLifecycle.Messages.PermissionGroup
{
    [Serializable]
    public class AclUserPermissionGroupsCreatedMessage : EntityCreatedMessage<AclUserPermissionGroupsDTO>, ISagaMessage
    {
    }

    [Serializable]
    public class AclUserPermissionGroupsDeletedMessage : EntityDeletedMessage<AclUserPermissionGroupsDTO>, ISagaMessage
    {
    }

    [Serializable]
    public class AclUserPermissionGroupsUpdatedMessage : EntityUpdatedMessage<AclUserPermissionGroupsDTO, AclUserPermissionGroupsDTO.AclUserPermissionGroups>, ISagaMessage
    {
    }
}
