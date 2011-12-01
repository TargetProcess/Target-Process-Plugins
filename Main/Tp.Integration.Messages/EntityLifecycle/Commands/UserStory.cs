// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Commands
{
	[Serializable]
	public class CreateUserStoryCommand : CreateEntityCommand<UserStoryDTO>
	{
		public CreateUserStoryCommand(UserStoryDTO dto) : base(dto)
		{
		}
	}

	[Serializable]
	public class UpdateUserStoryCommand : UpdateEntityCommand<UserStoryDTO>
	{
		public UpdateUserStoryCommand(UserStoryDTO dto) : base(dto)
		{
		}

		public UpdateUserStoryCommand(UserStoryDTO dto, Enum[] changedFields) : base(dto, changedFields)
		{
		}
	}

	[Serializable]
	public class DeleteUserStoryCommand : DeleteEntityCommand<UserStoryDTO>
	{
		public DeleteUserStoryCommand(int id) : base(id)
		{
		}
	}
}