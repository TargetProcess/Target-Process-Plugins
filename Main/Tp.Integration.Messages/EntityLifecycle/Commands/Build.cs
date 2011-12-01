// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Commands
{
	[Serializable]
	public class CreateBuildCommand : CreateEntityCommand<BuildDTO>
	{
		public CreateBuildCommand(BuildDTO dto) : base(dto)
		{
		}
	}

	[Serializable]
	public class UpdateBuildCommand : UpdateEntityCommand<BuildDTO>
	{
		public UpdateBuildCommand(BuildDTO dto, Enum[] changedFields) : base(dto, changedFields)
		{
		}

		public UpdateBuildCommand(BuildDTO dto) : base(dto)
		{
		}
	}

	[Serializable]
	public class DeleteBuildCommand : DeleteEntityCommand<BuildDTO>
	{
		public DeleteBuildCommand(int id) : base(id)
		{
		}
	}
}