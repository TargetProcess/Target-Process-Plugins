// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Messages
{
	[Serializable]
	public class TimeCreatedMessage : EntityCreatedMessage<TimeDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class TimeDeletedMessage : EntityDeletedMessage<TimeDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class TimeUpdatedMessage : EntityUpdatedMessage<TimeDTO, TimeField>, ISagaMessage
	{
		
	}
}