// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Queries
{
	[Serializable]
	public class GlobalSettingQueryResult : QueryResult<GlobalSettingDTO>, ISagaMessage
	{
	}

	[Serializable]
	public class RetrieveGlobalSettingQuery : QueryBase
	{
		public override DtoType DtoType
		{
			get { return new DtoType(typeof (GlobalSettingDTO)); }
		}
	}
}