// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Commands
{
	[Serializable]
	public class CreateReleaseCommand : CreateEntityCommand<ReleaseDTO>
	{
		public CreateReleaseCommand(ReleaseDTO dto) : base(dto)
		{
		}
	}

	[Serializable]
	public class UpdateReleaseCommand : UpdateEntityCommand<ReleaseDTO>
	{
		public UpdateReleaseCommand(ReleaseDTO dto) : base(dto)
		{
		}

		public UpdateReleaseCommand(ReleaseDTO dto, Enum[] changedFields) : base(dto, changedFields)
		{
		}
	}

	[Serializable]
	public class DeleteReleaseCommand : DeleteEntityCommand<ReleaseDTO>
	{
		public DeleteReleaseCommand(int id) : base(id)
		{
		}
	}
}