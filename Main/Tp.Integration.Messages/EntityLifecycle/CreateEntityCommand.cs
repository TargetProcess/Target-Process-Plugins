// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle
{
	/// <summary>
	/// Base command for creating entity in TargetProcess.
	/// </summary>
	/// <typeparam name="TEntityDto">The type of entity to create.</typeparam>
	[Serializable]
	public class CreateEntityCommand<TEntityDto> : ICreateEntityCommand<TEntityDto>
		where TEntityDto : DataTransferObject, new()
	{
		public CreateEntityCommand(TEntityDto dto)
		{
			Dto = dto;
		}
		
		/// <summary>
		/// The entity to create.
		/// </summary>
		public TEntityDto Dto { get; set; }
	}
}