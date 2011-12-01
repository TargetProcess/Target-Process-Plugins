using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Queries
{
	[Serializable]
	public class RetrieveAllSeveritiesQuery : QueryBase
	{
		public override DtoType DtoType
		{
			get { return new DtoType(typeof(SeverityDTO)); }
		}
	}

	[Serializable]
	public class SeverityQueryResult : QueryResult<SeverityDTO>, ISagaMessage
	{
	}
}