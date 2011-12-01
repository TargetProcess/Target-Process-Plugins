// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Commands
{
	[Serializable]
	public class CreateCommentCommand : CreateEntityCommand<CommentDTO>
	{
		public CreateCommentCommand(CommentDTO dto) : base(dto)
		{
		}
	}

	[Serializable]
	public class UpdateCommentCommand : UpdateEntityCommand<CommentDTO>
	{
		public UpdateCommentCommand(CommentDTO dto) : base(dto)
		{
		}

		public UpdateCommentCommand(CommentDTO dto, Enum[] changedFields) : base(dto, changedFields)
		{
		}
	}

	[Serializable]
	public class DeleteCommentCommand : DeleteEntityCommand<CommentDTO>
	{
		public DeleteCommentCommand(int id) : base(id)
		{
		}
	}
}