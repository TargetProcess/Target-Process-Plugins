// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Commands
{
	[Serializable]
	public class CreateTestPlanRunCommand : CreateEntityCommand<TestPlanRunDTO>
	{
		public CreateTestPlanRunCommand(TestPlanRunDTO dto) : base(dto)
		{
		}
	}

	[Serializable]
	public class UpdateTestPlanRunCommand : UpdateEntityCommand<TestPlanRunDTO>
	{
		public UpdateTestPlanRunCommand(TestPlanRunDTO dto) : base(dto)
		{
		}

		public UpdateTestPlanRunCommand(TestPlanRunDTO dto, Enum[] changedFields) : base(dto, changedFields)
		{
		}
	}

	[Serializable]
	public class DeleteTestPlanRunCommand : DeleteEntityCommand<TestPlanRunDTO>
	{
		public DeleteTestPlanRunCommand(int id) : base(id)
		{
		}
	}
}