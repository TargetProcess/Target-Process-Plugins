// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Commands
{
	[Serializable]
	public class CreateProcessCommand : CreateEntityCommand<ProcessDTO>
	{
		public CreateProcessCommand(ProcessDTO dto) : base(dto)
		{
		}
	}

	[Serializable]
	public class UpdateProcessCommand : UpdateEntityCommand<ProcessDTO>
	{
		public UpdateProcessCommand(ProcessDTO dto) : base(dto)
		{
		}

		public UpdateProcessCommand(ProcessDTO dto, Enum[] changedFields) : base(dto, changedFields)
		{
		}
	}

	[Serializable]
	public class DeleteProcessCommand : DeleteEntityCommand<ProcessDTO>
	{
		public DeleteProcessCommand(int id) : base(id)
		{
		}
	}
}