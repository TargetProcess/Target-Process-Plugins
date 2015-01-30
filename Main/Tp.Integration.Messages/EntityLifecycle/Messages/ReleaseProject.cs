using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Messages
{
	[Serializable]
	public class ReleaseProjectCreatedMessage : EntityCreatedMessage<ReleaseProjectDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class ReleaseProjectDeletedMessage : EntityDeletedMessage<ReleaseProjectDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class ReleaseProjectUpdatedMessage : EntityUpdatedMessage<ReleaseProjectDTO, ReleaseProjectField>, ISagaMessage
	{
	}
}