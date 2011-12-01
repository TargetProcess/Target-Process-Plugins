// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Messages
{
	[Serializable]
	public class RequestCreatedMessage : EntityCreatedMessage<RequestDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class RequestDeletedMessage : EntityDeletedMessage<RequestDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class RequestUpdatedMessage : EntityUpdatedMessage<RequestDTO, RequestField>, ISagaMessage
	{
	}
}