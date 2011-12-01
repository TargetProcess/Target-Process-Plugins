// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Commands
{
	[Serializable]
	public class CreateIterationCommand : CreateEntityCommand<IterationDTO>
	{
		public CreateIterationCommand(IterationDTO dto) : base(dto)
		{
		}
	}

	[Serializable]
	public class UpdateIterationCommand : UpdateEntityCommand<IterationDTO>
	{
		public UpdateIterationCommand(IterationDTO dto) : base(dto)
		{
		}

		public UpdateIterationCommand(IterationDTO dto, Enum[] changedFields) : base(dto, changedFields)
		{
		}
	}

	[Serializable]
	public class DeleteIterationCommand : DeleteEntityCommand<IterationDTO>
	{
		public DeleteIterationCommand(int id) : base(id)
		{
		}
	}
}