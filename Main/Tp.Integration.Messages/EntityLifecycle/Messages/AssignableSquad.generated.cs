// 
// THIS FILE IS AUTOGENERATED! ANY MANUAL MODIFICATIONS WILL BE LOST!
//

using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Messages
{
	[Serializable]
	public class AssignableSquadCreatedMessage : EntityCreatedMessage<AssignableSquadDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class AssignableSquadDeletedMessage : EntityDeletedMessage<AssignableSquadDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class AssignableSquadUpdatedMessage : EntityUpdatedMessage<AssignableSquadDTO, AssignableSquadField>, ISagaMessage
	{
		
	}

}