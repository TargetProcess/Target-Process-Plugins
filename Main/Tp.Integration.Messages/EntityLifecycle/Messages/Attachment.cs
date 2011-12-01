// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Messages
{
	[Serializable]
	public class AttachmentCreatedMessage : EntityCreatedMessage<AttachmentDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class AttachmentDeletedMessage : EntityDeletedMessage<AttachmentDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class AttachmentUpdatedMessage : EntityUpdatedMessage<AttachmentDTO, AttachmentField>, ISagaMessage
	{
	}

	[Serializable]
	public class AttachmentPartAddedMessage : SagaMessage, ISagaMessage
	{
	}
}