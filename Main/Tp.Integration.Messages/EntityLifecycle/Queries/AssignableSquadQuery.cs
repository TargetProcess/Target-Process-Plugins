using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Queries
{
	[Serializable]
	public class RetrieveAllAssignableSquadsQuery : QueryBase
	{
		public override DtoType DtoType
		{
			get { return new DtoType(typeof(AssignableSquadDTO)); }
		}
	}

	[Serializable]
	public class AssignableSquadQueryResult : QueryResult<AssignableSquadDTO>, ISagaMessage
	{
	}
}
