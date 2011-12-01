// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Queries
{
	[Serializable]
	public class RetrieveAllProjectsQuery : QueryBase
	{
		public override DtoType DtoType
		{
			get { return new DtoType(typeof (ProjectDTO)); }
		}
	}

	[Serializable]
	public class ProjectQueryResult : QueryResult<ProjectDTO>, ISagaMessage
	{
	}
}