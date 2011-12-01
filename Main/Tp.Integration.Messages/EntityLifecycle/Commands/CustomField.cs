// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Commands
{
	[Serializable]
	public class CreateCustomFieldCommand : CreateEntityCommand<CustomFieldDTO>
	{
		public CreateCustomFieldCommand(CustomFieldDTO dto) : base(dto)
		{
		}
	}

	[Serializable]
	public class UpdateCustomFieldCommand : UpdateEntityCommand<CustomFieldDTO>
	{
		public UpdateCustomFieldCommand(CustomFieldDTO dto) : base(dto)
		{
		}

		public UpdateCustomFieldCommand(CustomFieldDTO dto, Enum[] changedFields) : base(dto, changedFields)
		{
		}
	}

	[Serializable]
	public class DeleteCustomFieldCommand : DeleteEntityCommand<CustomFieldDTO>
	{
		public DeleteCustomFieldCommand(int id) : base(id)
		{
		}
	}
}