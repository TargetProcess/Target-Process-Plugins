// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Queries
{
	[Serializable]
	public class TestCaseTestPlanQuery : QueryBase
	{
		public int TestPlanId { get; set; }

		public override DtoType DtoType
		{
			get { return new DtoType(typeof (TestCaseTestPlanDTO)); }
		}
	}

	[Serializable]
	public class TestCaseTestPlanQueryResult : QueryResult<TestCaseTestPlanDTO>, ISagaMessage
	{
	}
}