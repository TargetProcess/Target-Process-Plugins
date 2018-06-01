// 
// THIS FILE IS AUTOGENERATED! ANY MANUAL MODIFICATIONS WILL BE LOST!
//

using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Messages
{
	[Serializable]
	public class RuleCreatedMessage : EntityCreatedMessage<RuleDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class RuleDeletedMessage : EntityDeletedMessage<RuleDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class RuleUpdatedMessage : EntityUpdatedMessage<RuleDTO, RuleField>, ISagaMessage
	{
		
	}

}