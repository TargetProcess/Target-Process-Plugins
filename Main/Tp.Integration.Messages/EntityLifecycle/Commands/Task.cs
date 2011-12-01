// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Commands
{
	[Serializable]
	public class CreateTaskCommand : CreateEntityCommand<TaskDTO>
	{
		public CreateTaskCommand(TaskDTO dto) : base(dto)
		{
		}
	}

	[Serializable]
	public class UpdateTaskCommand : UpdateEntityCommand<TaskDTO>
	{
		public UpdateTaskCommand(TaskDTO dto) : base(dto)
		{
		}

		public UpdateTaskCommand(TaskDTO dto, Enum[] changedFields) : base(dto, changedFields)
		{
		}
	}

	[Serializable]
	public class DeleteTaskCommand : DeleteEntityCommand<TaskDTO>
	{
		public DeleteTaskCommand(int id) : base(id)
		{
		}
	}
}