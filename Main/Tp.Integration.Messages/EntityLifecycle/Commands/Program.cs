// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Commands
{
	[Serializable]
	public class CreateProgramCommand : CreateEntityCommand<ProgramDTO>
	{
		public CreateProgramCommand(ProgramDTO dto) : base(dto)
		{
		}
	}

	[Serializable]
	public class UpdateProgramCommand : UpdateEntityCommand<ProgramDTO>
	{
		public UpdateProgramCommand(ProgramDTO dto) : base(dto)
		{
		}

		public UpdateProgramCommand(ProgramDTO dto, Enum[] changedFields) : base(dto, changedFields)
		{
		}
	}

	[Serializable]
	public class DeleteProgramCommand : DeleteEntityCommand<ProgramDTO>
	{
		public DeleteProgramCommand(int id) : base(id)
		{
		}
	}
}