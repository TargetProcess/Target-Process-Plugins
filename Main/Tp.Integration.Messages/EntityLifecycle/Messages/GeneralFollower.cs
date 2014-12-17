using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Messages
{
	[Serializable]
	public class GeneralFollowerCreatedMessage : EntityCreatedMessage<GeneralFollowerDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class GeneralFollowerUpdatedMessage : EntityUpdatedMessage<GeneralFollowerDTO, GeneralFollowerField>, ISagaMessage
	{
	}

	[Serializable]
	public class GeneralFollowerDeletedMessage : EntityDeletedMessage<GeneralFollowerDTO>, ISagaMessage
	{
	}
}