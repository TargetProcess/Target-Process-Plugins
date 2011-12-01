// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Commands
{
	[Serializable]
	public class CreateFeatureCommand : CreateEntityCommand<FeatureDTO>
	{
		public CreateFeatureCommand(FeatureDTO dto) : base(dto)
		{
		}
	}

	[Serializable]
	public class UpdateFeatureCommand : UpdateEntityCommand<FeatureDTO>
	{
		public UpdateFeatureCommand(FeatureDTO dto) : base(dto)
		{
		}

		public UpdateFeatureCommand(FeatureDTO dto, Enum[] changedFields) : base(dto, changedFields)
		{
		}
	}

	[Serializable]
	public class DeleteFeatureCommand : DeleteEntityCommand<FeatureDTO>
	{
		public DeleteFeatureCommand(int id) : base(id)
		{
		}
	}
}