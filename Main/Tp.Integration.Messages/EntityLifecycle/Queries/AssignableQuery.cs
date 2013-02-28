// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Queries
{
	[Serializable]
	public class AssignableQuery : QueryBase
	{
		public string Hql { get; set; }
		public object[] Params { get; set; }

		public override DtoType DtoType
		{
			get { return new DtoType(typeof(AssignableDTO)); }
		}
	}

	[Serializable]
	public class AssignableQueryResult : QueryResult<AssignableDTO>, ISagaMessage
	{
	}
}