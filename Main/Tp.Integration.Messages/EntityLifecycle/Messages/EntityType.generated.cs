// 
// THIS FILE IS AUTOGENERATED! ANY MANUAL MODIFICATIONS WILL BE LOST!
//

using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Messages
{
	[Serializable]
	public class EntityTypeCreatedMessage : EntityCreatedMessage<EntityTypeDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class EntityTypeDeletedMessage : EntityDeletedMessage<EntityTypeDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class EntityTypeUpdatedMessage : EntityUpdatedMessage<EntityTypeDTO, EntityTypeField>, ISagaMessage
	{
		
	}

}