// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Commands
{
	[Serializable]
	public class CreateProjectMemberCommand : CreateEntityCommand<ProjectMemberDTO>
	{
		public CreateProjectMemberCommand(ProjectMemberDTO dto) : base(dto)
		{
		}
	}

	[Serializable]
	public class UpdateProjectMemberCommand : UpdateEntityCommand<ProjectMemberDTO>
	{
		public UpdateProjectMemberCommand(ProjectMemberDTO dto) : base(dto)
		{
		}

		public UpdateProjectMemberCommand(ProjectMemberDTO dto, Enum[] changedFields) : base(dto, changedFields)
		{
		}
	}

	[Serializable]
	public class DeleteProjectMemberCommand : DeleteEntityCommand<ProjectMemberDTO>
	{
		public DeleteProjectMemberCommand(int id) : base(id)
		{
		}
	}
}