﻿// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Queries
{
	[Serializable]
	public class EntityStateQuery : QueryBase
	{
		public int ProjectId { get; set; }

		public override DtoType DtoType
		{
			get { return new DtoType(typeof (EntityStateDTO)); }
		}
	}

	[Serializable]
	public class EntityStateQueryResult : QueryResult<EntityStateDTO>, ISagaMessage
	{
	}
}