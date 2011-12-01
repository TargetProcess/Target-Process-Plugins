// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Commands
{
	[Serializable]
	public class CreateRequesterCommand : CreateEntityCommand<RequesterDTO>
	{
		public CreateRequesterCommand(RequesterDTO dto) : base(dto)
		{
		}
	}

	[Serializable]
	public class UpdateRequesterCommand : UpdateEntityCommand<RequesterDTO>
	{
		public UpdateRequesterCommand(RequesterDTO dto) : base(dto)
		{
		}

		public UpdateRequesterCommand(RequesterDTO dto, Enum[] changedFields) : base(dto, changedFields)
		{
		}
	}

	[Serializable]
	public class DeleteRequesterCommand : DeleteEntityCommand<RequesterDTO>
	{
		public DeleteRequesterCommand(int id) : base(id)
		{
		}
	}

	[Serializable]
	public class AttachGeneralUserToRequestCommand : ITargetProcessCommand
	{
		public int? RequesterId { get; set; }
		public int? RequestId { get; set; }
	}
}