// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Commands
{
	[Serializable]
	public class CreateTagCommand : CreateEntityCommand<TagDTO>
	{
		public CreateTagCommand(TagDTO dto) : base(dto)
		{
		}
	}

	[Serializable]
	public class UpdateTagCommand : UpdateEntityCommand<TagDTO>
	{
		public UpdateTagCommand(TagDTO dto) : base(dto)
		{
		}

		public UpdateTagCommand(TagDTO dto, Enum[] changedFields) : base(dto, changedFields)
		{
		}
	}

	[Serializable]
	public class DeleteTagCommand : DeleteEntityCommand<TagDTO>
	{
		public DeleteTagCommand(int id) : base(id)
		{
		}
	}
}