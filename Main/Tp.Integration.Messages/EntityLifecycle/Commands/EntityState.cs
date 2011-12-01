// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Commands
{
	[Serializable]
	public class CreateEntityStateCommand : CreateEntityCommand<EntityStateDTO>
	{
		public CreateEntityStateCommand(EntityStateDTO dto) : base(dto)
		{
		}
	}

	[Serializable]
	public class UpdateEntityStateCommand : UpdateEntityCommand<EntityStateDTO>
	{
		public UpdateEntityStateCommand(EntityStateDTO dto) : base(dto)
		{
		}

		public UpdateEntityStateCommand(EntityStateDTO dto, Enum[] changedFields) : base(dto, changedFields)
		{
		}
	}

	[Serializable]
	public class DeleteEntityStateCommand : DeleteEntityCommand<EntityStateDTO>
	{
		public DeleteEntityStateCommand(int id) : base(id)
		{
		}
	}
}