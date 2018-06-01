// 
// THIS FILE IS AUTOGENERATED! ANY MANUAL MODIFICATIONS WILL BE LOST!
//

using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Messages
{
	[Serializable]
	public class TagBundleCreatedMessage : EntityCreatedMessage<TagBundleDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class TagBundleDeletedMessage : EntityDeletedMessage<TagBundleDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class TagBundleUpdatedMessage : EntityUpdatedMessage<TagBundleDTO, TagBundleField>, ISagaMessage
	{
		
	}

}