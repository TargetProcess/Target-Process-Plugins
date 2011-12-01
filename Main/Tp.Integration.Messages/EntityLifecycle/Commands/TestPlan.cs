// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Commands
{
	[Serializable]
	public class CreateTestPlanCommand : CreateEntityCommand<TestPlanDTO>
	{
		public CreateTestPlanCommand(TestPlanDTO dto) : base(dto)
		{
		}
	}

	[Serializable]
	public class UpdateTestPlanCommand : UpdateEntityCommand<TestPlanDTO>
	{
		public UpdateTestPlanCommand(TestPlanDTO dto) : base(dto)
		{
		}

		public UpdateTestPlanCommand(TestPlanDTO dto, Enum[] changedFields) : base(dto, changedFields)
		{
		}
	}

	[Serializable]
	public class DeleteTestPlanCommand : DeleteEntityCommand<TestPlanDTO>
	{
		public DeleteTestPlanCommand(int id) : base(id)
		{
		}
	}
}