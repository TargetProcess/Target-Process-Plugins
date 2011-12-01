// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Messages
{
	[Serializable]
	public class RequesterCreatedMessage : EntityCreatedMessage<RequesterDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class RequesterDeletedMessage : EntityDeletedMessage<RequesterDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class RequesterUpdatedMessage : EntityUpdatedMessage<RequesterDTO, RequesterField>, ISagaMessage
	{
	}

	[Serializable]
	public class GeneralUserAttachedToRequestMessage : SagaMessage, ISagaMessage
	{
		public int? RequesterId { get; set; }
		public int? RequestId { get; set; }
	}
}