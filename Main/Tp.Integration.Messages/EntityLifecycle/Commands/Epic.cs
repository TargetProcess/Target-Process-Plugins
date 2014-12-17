// 
// Copyright (c) 2005-2014 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Commands
{
	[Serializable]
	public class CreateEpicCommand : CreateEntityCommand<EpicDTO>
	{
		public CreateEpicCommand(EpicDTO dto)
			: base(dto)
		{
		}
	}

	[Serializable]
	public class UpdateEpicCommand : UpdateEntityCommand<EpicDTO>
	{
		public UpdateEpicCommand(EpicDTO dto)
			: base(dto)
		{
		}

		public UpdateEpicCommand(EpicDTO dto, Enum[] changedFields)
			: base(dto, changedFields)
		{
		}
	}

	[Serializable]
	public class DeleteEpicCommand : DeleteEntityCommand<EpicDTO>
	{
		public DeleteEpicCommand(int id)
			: base(id)
		{
		}
	}
}