// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Commands
{
	[Serializable]
	public class CreateTeamCommand : CreateEntityCommand<TeamDTO>
	{
		public CreateTeamCommand(TeamDTO dto) : base(dto)
		{
		}
	}

	[Serializable]
	public class UpdateTeamCommand : UpdateEntityCommand<TeamDTO>
	{
		public UpdateTeamCommand(TeamDTO dto) : base(dto)
		{
		}

		public UpdateTeamCommand(TeamDTO dto, Enum[] changedFields) : base(dto, changedFields)
		{
		}
	}

	[Serializable]
	public class DeleteTeamCommand : DeleteEntityCommand<TeamDTO>
	{
		public DeleteTeamCommand(int id) : base(id)
		{
		}
	}
}