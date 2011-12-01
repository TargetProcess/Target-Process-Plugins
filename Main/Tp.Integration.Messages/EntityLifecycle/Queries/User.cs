// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Queries
{
	[Serializable]
	public class RetrieveAllUsersQuery : QueryBase
	{
		public override DtoType DtoType
		{
			get { return new DtoType(typeof (UserDTO)); }
		}
	}

	[Serializable]
	public class UserQueryResult : QueryResult<UserDTO>, ISagaMessage
	{
	}
}