// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Messages
{
	[Serializable]
	public class AttachmentFileCreatedMessage : EntityCreatedMessage<AttachmentFileDTO>, ISagaMessage
	{
	}

	public class AttachmentFileDeletedMessage : EntityDeletedMessage<AttachmentFileDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class AttachmentFileUpdatedMessage : EntityUpdatedMessage<AttachmentFileDTO, AttachmentFileField>, ISagaMessage
	{
	}
}