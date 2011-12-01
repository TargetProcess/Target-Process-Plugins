// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Messages
{
	[Serializable]
	public class RevisionCreatedMessage : EntityCreatedMessage<RevisionDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class RevisionDeletedMessage : EntityDeletedMessage<RevisionDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class RevisionUpdatedMessage : EntityUpdatedMessage<RevisionDTO, RevisionField>, ISagaMessage
	{
	}
}