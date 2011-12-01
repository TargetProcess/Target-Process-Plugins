// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Messages
{
	[Serializable]
	public class CustomFieldCreatedMessage : EntityCreatedMessage<CustomFieldDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class CustomFieldDeletedMessage : EntityDeletedMessage<CustomFieldDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class CustomFieldUpdatedMessage : EntityUpdatedMessage<CustomFieldDTO, CustomFieldField>, ISagaMessage
	{
		
	}
}