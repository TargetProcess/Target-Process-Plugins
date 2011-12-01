// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Messages
{
	[Serializable]
	public class SeverityCreatedMessage : EntityCreatedMessage<SeverityDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class SeverityDeletedMessage : EntityDeletedMessage<SeverityDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class SeverityUpdatedMessage : EntityUpdatedMessage<SeverityDTO, SeverityField>, ISagaMessage
	{
	}

	[Serializable]
	public class SeverityPartAddedMessage : SagaMessage, ISagaMessage
	{
	}
}