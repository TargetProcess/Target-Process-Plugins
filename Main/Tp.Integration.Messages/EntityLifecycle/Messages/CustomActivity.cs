// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Messages
{
	[Serializable]
	public class CustomActivityCreatedMessage : EntityCreatedMessage<CustomActivityDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class CustomActivityDeletedMessage : EntityDeletedMessage<CustomActivityDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class CustomActivityUpdatedMessage : EntityUpdatedMessage<CustomActivityDTO, CustomActivityField>, ISagaMessage
	{
		
	}
}