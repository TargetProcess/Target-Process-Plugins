// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Commands
{
	[Serializable]
	public class CreateRoleEffortCommand : CreateEntityCommand<RoleEffortDTO>
	{
		public CreateRoleEffortCommand(RoleEffortDTO dto) : base(dto)
		{
		}
	}

	[Serializable]
	public class UpdateRoleEffortCommand : UpdateEntityCommand<RoleEffortDTO>
	{
		public UpdateRoleEffortCommand(RoleEffortDTO dto) : base(dto)
		{
		}

		public UpdateRoleEffortCommand(RoleEffortDTO dto, Enum[] changedFields) : base(dto, changedFields)
		{
		}
	}

	[Serializable]
	public class DeleteRoleEffortCommand : DeleteEntityCommand<RoleEffortDTO>
	{
		public DeleteRoleEffortCommand(int id) : base(id)
		{
		}
	}
}