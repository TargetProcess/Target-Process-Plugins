// 
// Copyright (c) 2005-2014 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Queries
{
	[Serializable]
	public class ImpedimentQuery : QueryBase
	{
		public override DtoType DtoType
		{
			get { return new DtoType(typeof(ImpedimentDTO)); }
		}
	}

	[Serializable]
	public class ImpedimentQueryResult : QueryResult<ImpedimentDTO>, ISagaMessage
	{
	}
}