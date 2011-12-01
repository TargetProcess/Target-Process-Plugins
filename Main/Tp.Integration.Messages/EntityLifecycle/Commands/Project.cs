// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Commands
{
	[Serializable]
	public class CreateProjectCommand : CreateEntityCommand<ProjectDTO>
	{
		public CreateProjectCommand(ProjectDTO dto) : base(dto)
		{
		}
	}

	[Serializable]
	public class UpdateProjectCommand : UpdateEntityCommand<ProjectDTO>
	{
		public UpdateProjectCommand(ProjectDTO dto) : base(dto)
		{
		}

		public UpdateProjectCommand(ProjectDTO dto, Enum[] changedFields) : base(dto, changedFields)
		{
		}
	}

	[Serializable]
	public class DeleteProjectCommand : DeleteEntityCommand<ProjectDTO>
	{
		public DeleteProjectCommand(int id) : base(id)
		{
		}
	}
}