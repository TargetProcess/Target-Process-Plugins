// 
// THIS FILE IS AUTOGENERATED! ANY MANUAL MODIFICATIONS WILL BE LOST!
//

using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Commands
{
	[Serializable]
	public class CreateTestCaseRunBugCommand : CreateEntityCommand<TestCaseRunBugDTO>
	{
		public CreateTestCaseRunBugCommand(TestCaseRunBugDTO dto) : base(dto)
		{
		}
	}

	[Serializable]
	public class UpdateTestCaseRunBugCommand : UpdateEntityCommand<TestCaseRunBugDTO>
	{
		public UpdateTestCaseRunBugCommand(TestCaseRunBugDTO dto) : base(dto)
		{
		}

		public UpdateTestCaseRunBugCommand(TestCaseRunBugDTO dto, Enum[] changedFields) : base(dto, changedFields)
		{
		}
	}

	[Serializable]
	public class DeleteTestCaseRunBugCommand : DeleteEntityCommand<TestCaseRunBugDTO>
	{
		public DeleteTestCaseRunBugCommand(int id) : base(id)
		{
		}
	}

}