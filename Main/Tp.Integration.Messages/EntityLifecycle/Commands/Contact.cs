// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Commands
{
	[Serializable]
	public class CreateContactCommand : CreateEntityCommand<ContactDTO>
	{
		public CreateContactCommand(ContactDTO dto) : base(dto)
		{
		}
	}

	[Serializable]
	public class UpdateContactCommand : UpdateEntityCommand<ContactDTO>
	{
		public UpdateContactCommand(ContactDTO dto) : base(dto)
		{
		}

		public UpdateContactCommand(ContactDTO dto, Enum[] changedFields) : base(dto, changedFields)
		{
		}
	}

	[Serializable]
	public class DeleteContactCommand : DeleteEntityCommand<ContactDTO>
	{
		public DeleteContactCommand(int id) : base(id)
		{
		}
	}
}