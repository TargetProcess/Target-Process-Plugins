// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Messages
{
	[Serializable]
	public class RoleCreatedMessage : EntityCreatedMessage<RoleDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class RoleDeletedMessage : EntityDeletedMessage<RoleDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class RoleUpdatedMessage : EntityUpdatedMessage<RoleDTO, RoleField>, ISagaMessage
	{
		
	}
}