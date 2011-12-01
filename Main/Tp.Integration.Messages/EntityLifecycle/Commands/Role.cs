// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Commands
{
	[Serializable]
	public class CreateRoleCommand : CreateEntityCommand<RoleDTO>
	{
		public CreateRoleCommand(RoleDTO dto) : base(dto)
		{
		}
	}

	[Serializable]
	public class UpdateRoleCommand : UpdateEntityCommand<RoleDTO>
	{
		public UpdateRoleCommand(RoleDTO dto) : base(dto)
		{
		}

		public UpdateRoleCommand(RoleDTO dto, Enum[] changedFields) : base(dto, changedFields)
		{
		}
	}

	[Serializable]
	public class DeleteRoleCommand : DeleteEntityCommand<RoleDTO>
	{
		public DeleteRoleCommand(int id) : base(id)
		{
		}
	}
}