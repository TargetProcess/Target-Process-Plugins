// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Commands
{
	[Serializable]
	public class CreateRequestCommand : CreateEntityCommand<RequestDTO>
	{
		public CreateRequestCommand(RequestDTO dto) : base(dto)
		{
		}
	}

	[Serializable]
	public class UpdateRequestCommand : UpdateEntityCommand<RequestDTO>
	{
		public UpdateRequestCommand(RequestDTO dto) : base(dto)
		{
		}

		public UpdateRequestCommand(RequestDTO dto, Enum[] changedFields) : base(dto, changedFields)
		{
		}
	}

	[Serializable]
	public class DeleteRequestCommand : DeleteEntityCommand<RequestDTO>
	{
		public DeleteRequestCommand(int id) : base(id)
		{
		}
	}
}