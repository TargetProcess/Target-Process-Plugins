// 
// THIS FILE IS AUTOGENERATED! ANY MANUAL MODIFICATIONS WILL BE LOST!
//

using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Messages
{
	[Serializable]
	public class FreezedTestStepInfoCreatedMessage : EntityCreatedMessage<FreezedTestStepInfoDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class FreezedTestStepInfoDeletedMessage : EntityDeletedMessage<FreezedTestStepInfoDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class FreezedTestStepInfoUpdatedMessage : EntityUpdatedMessage<FreezedTestStepInfoDTO, FreezedTestStepInfoField>, ISagaMessage
	{
		
	}

}