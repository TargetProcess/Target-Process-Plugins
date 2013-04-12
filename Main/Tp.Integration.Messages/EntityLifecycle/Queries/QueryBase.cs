// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Queries
{
	[Serializable]
	public abstract class QueryBase : ITargetProcessCommand
	{
		public abstract DtoType DtoType { get; }
		public bool IgnoreMessageSizeOverrunFailure { get; set; }
	}

	[Serializable]
	public class QueryResult<TDto> : SagaMessage
		where TDto : DataTransferObject
	{
		public TDto[] Dtos { get; set; }
		public int QueryResultCount { get; set; }
	}
}