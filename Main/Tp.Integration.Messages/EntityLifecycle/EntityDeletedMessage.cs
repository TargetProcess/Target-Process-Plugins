// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle
{
	/// <summary>
	/// Base message indicating that some entity was deleted in TargetProcess.
	/// </summary>
	/// <typeparam name="TEntityDto">The type of deleted entity.</typeparam>
	[Serializable]
	public class EntityDeletedMessage<TEntityDto> : EntityCreatedMessage<TEntityDto>
		where TEntityDto : DataTransferObject, new()
	{
	}
}