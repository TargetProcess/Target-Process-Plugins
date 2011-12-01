// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Commands
{
	[Serializable]
	public class CreateTestCaseCommand : CreateEntityCommand<TestCaseDTO>
	{
		public CreateTestCaseCommand(TestCaseDTO dto) : base(dto)
		{
		}
	}

	[Serializable]
	public class UpdateTestCaseCommand : UpdateEntityCommand<TestCaseDTO>
	{
		public UpdateTestCaseCommand(TestCaseDTO dto) : base(dto)
		{
		}

		public UpdateTestCaseCommand(TestCaseDTO dto, Enum[] changedFields) : base(dto, changedFields)
		{
		}
	}

	[Serializable]
	public class DeleteTestCaseCommand : DeleteEntityCommand<TestCaseDTO>
	{
		public DeleteTestCaseCommand(int id) : base(id)
		{
		}
	}
}