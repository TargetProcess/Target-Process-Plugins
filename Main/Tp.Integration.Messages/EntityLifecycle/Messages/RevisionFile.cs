using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Messages
{
	[Serializable]
	public class RevisionFileCreatedMessage : EntityCreatedMessage<RevisionFileDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class RevisionFileDeletedMessage : EntityDeletedMessage<RevisionFileDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class RevisionFileUpdatedMessage : EntityUpdatedMessage<RevisionFileDTO, RevisionFileField>, ISagaMessage
	{
	}
}