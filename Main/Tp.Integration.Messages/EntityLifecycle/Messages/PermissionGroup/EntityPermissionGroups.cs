using System;
using Tp.Integration.Messages.Entities.PermissionGroups;

namespace Tp.Integration.Messages.EntityLifecycle.Messages.PermissionGroup
{
	[Serializable]
	public class EntityPermissionGroupsCreatedMessage : EntityCreatedMessage<EntityPermissionGroupsDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class EntityPermissionGroupsDeletedMessage : EntityDeletedMessage<EntityPermissionGroupsDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class EntityPermissionGroupsUpdatedMessage : EntityUpdatedMessage<EntityPermissionGroupsDTO, EntityPermissionGroupsField>, ISagaMessage
	{
	}
}
