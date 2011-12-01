// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Messages
{
	[Serializable]
	public class PriorityCreatedMessage : EntityCreatedMessage<PriorityDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class PriorityDeletedMessage : EntityDeletedMessage<PriorityDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class PriorityUpdatedMessage : EntityUpdatedMessage<PriorityDTO, PriorityField>, ISagaMessage
	{
	}

	[Serializable]
	public class PriorityPartAddedMessage : SagaMessage, ISagaMessage
	{
	}
}