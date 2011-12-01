// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Messages
{
	[Serializable]
	public class RuleCreatedMessage : EntityCreatedMessage<RuleDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class RuleDeletedMessage : EntityDeletedMessage<RuleDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class RuleUpdatedMessage : EntityUpdatedMessage<RuleDTO, RuleField>, ISagaMessage
	{
		
	}
}