// 
// THIS FILE IS AUTOGENERATED! ANY MANUAL MODIFICATIONS WILL BE LOST!
//

using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Messages
{
	[Serializable]
	public class MilestoneProjectCreatedMessage : EntityCreatedMessage<MilestoneProjectDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class MilestoneProjectDeletedMessage : EntityDeletedMessage<MilestoneProjectDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class MilestoneProjectUpdatedMessage : EntityUpdatedMessage<MilestoneProjectDTO, MilestoneProjectField>, ISagaMessage
	{
		
	}

}