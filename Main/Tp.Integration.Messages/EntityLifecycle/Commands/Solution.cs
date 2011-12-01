// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Commands
{
	[Serializable]
	public class CreateSolutionCommand : CreateEntityCommand<SolutionDTO>
	{
		public CreateSolutionCommand(SolutionDTO dto) : base(dto)
		{
		}
	}

	[Serializable]
	public class UpdateSolutionCommand : UpdateEntityCommand<SolutionDTO>
	{
		public UpdateSolutionCommand(SolutionDTO dto) : base(dto)
		{
		}

		public UpdateSolutionCommand(SolutionDTO dto, Enum[] changedFields) : base(dto, changedFields)
		{
		}
	}

	[Serializable]
	public class DeleteSolutionCommand : DeleteEntityCommand<SolutionDTO>
	{
		public DeleteSolutionCommand(int id) : base(id)
		{
		}
	}
}