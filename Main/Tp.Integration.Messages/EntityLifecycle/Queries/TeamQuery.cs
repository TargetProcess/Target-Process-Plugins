using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Queries
{
	[Serializable]
	public class RetrieveAllTeamsQuery : QueryBase
	{
		public override DtoType DtoType
		{
			get { return new DtoType(typeof(TeamDTO)); }
		}
	}

	[Serializable]
	public class TeamQueryResult : QueryResult<TeamDTO>, ISagaMessage
	{
	}
}
