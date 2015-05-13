// 
// Copyright (c) 2005-2015 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Messages
{
	[Serializable]
	public class TestStepCreatedMessage : EntityCreatedMessage<TestStepDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class TestStepDeletedMessage : EntityDeletedMessage<TestStepDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class TestStepUpdatedMessage : EntityUpdatedMessage<TestStepDTO, TestStepField>, ISagaMessage
	{
	}
}