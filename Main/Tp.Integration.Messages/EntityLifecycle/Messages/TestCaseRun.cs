// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Messages
{
	[Serializable]
	public class TestCaseRunCreatedMessage : EntityCreatedMessage<TestCaseRunDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class TestCaseRunDeletedMessage : EntityDeletedMessage<TestCaseRunDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class TestCaseRunUpdatedMessage : EntityUpdatedMessage<TestCaseRunDTO, TestCaseRunField>, ISagaMessage
	{
	}
}