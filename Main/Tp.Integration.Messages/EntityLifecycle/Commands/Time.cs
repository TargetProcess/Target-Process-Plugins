// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Commands
{
	[Serializable]
	public class CreateTimeCommand : CreateEntityCommand<TimeDTO>
	{
		public CreateTimeCommand(TimeDTO dto) : base(dto)
		{
		}
	}

	[Serializable]
	public class UpdateTimeCommand : UpdateEntityCommand<TimeDTO>
	{
		public UpdateTimeCommand(TimeDTO dto) : base(dto)
		{
		}

		public UpdateTimeCommand(TimeDTO dto, Enum[] changedFields) : base(dto, changedFields)
		{
		}
	}

	[Serializable]
	public class DeleteTimeCommand : DeleteEntityCommand<TimeDTO>
	{
		public DeleteTimeCommand(int id) : base(id)
		{
		}
	}
}