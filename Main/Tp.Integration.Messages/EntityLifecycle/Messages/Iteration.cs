// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Messages
{
	[Serializable]
	public class IterationCreatedMessage : EntityCreatedMessage<IterationDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class IterationDeletedMessage : EntityDeletedMessage<IterationDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class IterationUpdatedMessage : EntityUpdatedMessage<IterationDTO, IterationField>, ISagaMessage
	{
		
	}
}