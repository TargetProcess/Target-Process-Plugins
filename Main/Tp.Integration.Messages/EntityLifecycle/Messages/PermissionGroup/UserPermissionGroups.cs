using System;
using Tp.Integration.Messages.Entities.PermissionGroups;

namespace Tp.Integration.Messages.EntityLifecycle.Messages.PermissionGroup
{
    [Serializable]
    public class UserPermissionGroupsCreatedMessage : EntityCreatedMessage<UserPermissionGroupsDTO>, ISagaMessage
    {
    }

    [Serializable]
    public class UserPermissionGroupsDeletedMessage : EntityDeletedMessage<UserPermissionGroupsDTO>, ISagaMessage
    {
    }

    [Serializable]
    public class UserPermissionGroupsUpdatedMessage : EntityUpdatedMessage<UserPermissionGroupsDTO, UserPermissionGroupsField>, ISagaMessage
    {
    }
}
