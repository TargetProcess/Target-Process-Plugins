// 
// THIS FILE IS AUTOGENERATED! ANY MANUAL MODIFICATIONS WILL BE LOST!
//

using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Messages
{
	[Serializable]
	public class TaskCreatedMessage : EntityCreatedMessage<TaskDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class TaskDeletedMessage : EntityDeletedMessage<TaskDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class TaskUpdatedMessage : EntityUpdatedMessage<TaskDTO, TaskField>, ISagaMessage
	{
		
	}

}
