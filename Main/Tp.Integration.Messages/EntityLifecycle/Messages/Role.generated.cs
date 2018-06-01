// 
// THIS FILE IS AUTOGENERATED! ANY MANUAL MODIFICATIONS WILL BE LOST!
//

using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Messages
{
	[Serializable]
	public class RoleCreatedMessage : EntityCreatedMessage<RoleDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class RoleDeletedMessage : EntityDeletedMessage<RoleDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class RoleUpdatedMessage : EntityUpdatedMessage<RoleDTO, RoleField>, ISagaMessage
	{
		
	}

}