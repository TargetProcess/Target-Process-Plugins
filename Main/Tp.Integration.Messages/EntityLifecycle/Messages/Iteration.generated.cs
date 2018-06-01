// 
// THIS FILE IS AUTOGENERATED! ANY MANUAL MODIFICATIONS WILL BE LOST!
//

using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Messages
{
	[Serializable]
	public class IterationCreatedMessage : EntityCreatedMessage<IterationDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class IterationDeletedMessage : EntityDeletedMessage<IterationDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class IterationUpdatedMessage : EntityUpdatedMessage<IterationDTO, IterationField>, ISagaMessage
	{
		
	}

}