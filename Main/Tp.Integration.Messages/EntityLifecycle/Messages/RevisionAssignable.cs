// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Messages
{
	[Serializable]
	public class RevisionAssignableCreatedMessage : EntityCreatedMessage<RevisionAssignableDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class RevisionAssignableDeletedMessage : EntityDeletedMessage<RevisionAssignableDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class RevisionAssignableUpdatedMessage : EntityUpdatedMessage<RevisionAssignableDTO, RevisionAssignableField>,
	                                                ISagaMessage
	{
	}
}