// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Commands
{
	[Serializable]
	public class CreateCustomActivityCommand : CreateEntityCommand<CustomActivityDTO>
	{
		public CreateCustomActivityCommand(CustomActivityDTO dto) : base(dto)
		{
		}
	}

	[Serializable]
	public class UpdateCustomActivityCommand : UpdateEntityCommand<CustomActivityDTO>
	{
		public UpdateCustomActivityCommand(CustomActivityDTO dto) : base(dto)
		{
		}

		public UpdateCustomActivityCommand(CustomActivityDTO dto, Enum[] changedFields) : base(dto, changedFields)
		{
		}
	}

	[Serializable]
	public class DeleteCustomActivityCommand : DeleteEntityCommand<CustomActivityDTO>
	{
		public DeleteCustomActivityCommand(int id) : base(id)
		{
		}
	}
}