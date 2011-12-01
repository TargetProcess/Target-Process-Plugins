// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Commands
{
	[Serializable]
	public class CreateImpedimentCommand : CreateEntityCommand<ImpedimentDTO>
	{
		public CreateImpedimentCommand(ImpedimentDTO dto) : base(dto)
		{
		}
	}

	[Serializable]
	public class UpdateImpedimentCommand : UpdateEntityCommand<ImpedimentDTO>
	{
		public UpdateImpedimentCommand(ImpedimentDTO dto) : base(dto)
		{
		}

		public UpdateImpedimentCommand(ImpedimentDTO dto, Enum[] changedFields) : base(dto, changedFields)
		{
		}
	}

	[Serializable]
	public class DeleteImpedimentCommand : DeleteEntityCommand<ImpedimentDTO>
	{
		public DeleteImpedimentCommand(int id) : base(id)
		{
		}
	}
}