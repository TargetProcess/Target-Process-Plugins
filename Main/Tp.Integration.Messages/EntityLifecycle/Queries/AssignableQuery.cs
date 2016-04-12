using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Queries
{
	[Serializable]
	public class AssignableQuery : QueryBase
	{
		public int? ProjectId { get; set; }

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
