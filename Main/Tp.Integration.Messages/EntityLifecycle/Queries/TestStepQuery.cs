// 
// Copyright (c) 2005-2015 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Queries
{
	[Serializable]
	public class TestStepQuery : QueryBase
	{
		public override DtoType DtoType
		{
			get { return new DtoType(typeof (TestStepDTO)); }
		}

		public int? TestCaseId { get; set; }
	}

	[Serializable]
	public class TestStepQueryResult : QueryResult<TestStepDTO>, ISagaMessage
	{
	}
}