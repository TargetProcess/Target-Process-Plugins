// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Messages
{
	[Serializable]
	public class SquadIterationCreatedMessage : EntityCreatedMessage<SquadIterationDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class SquadIterationDeletedMessage : EntityDeletedMessage<SquadIterationDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class SquadIterationUpdatedMessage : EntityUpdatedMessage<SquadIterationDTO, SquadIterationField>, ISagaMessage
	{
		
	}
}