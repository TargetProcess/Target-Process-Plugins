// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle
{
	/// <summary>
	/// Base message indicating that some entity was created in TargetProcess.
	/// </summary>
	/// <typeparam name="TEntityDto">The type of created entity.</typeparam>
	[Serializable]
	public class EntityCreatedMessage<TEntityDto> : SagaMessage
		where TEntityDto : DataTransferObject, new()
	{
		public EntityCreatedMessage()
		{
			Dto = new TEntityDto();
		}

		public EntityCreatedMessage(TEntityDto dto)
		{
			Dto = dto;
		}

		public TEntityDto Dto { get; set; }
	}
}