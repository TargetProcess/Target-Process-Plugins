// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Messages
{
	[Serializable]
	public class EntityStateCreatedMessage : EntityCreatedMessage<EntityStateDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class EntityStateDeletedMessage : EntityDeletedMessage<EntityStateDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class EntityStateUpdatedMessage : EntityUpdatedMessage<EntityStateDTO, EntityStateField>, ISagaMessage
	{
		
	}

	[Serializable]
	public class EntityStatePartAddedMessage : SagaMessage, ISagaMessage
	{
	}
}