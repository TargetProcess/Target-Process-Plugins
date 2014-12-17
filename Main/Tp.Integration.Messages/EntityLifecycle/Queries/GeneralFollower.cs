using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Queries
{
	[Serializable]
	public class RetrieveAllGeneralFollowersQuery : QueryBase
	{
		public override DtoType DtoType
		{
			get { return new DtoType(typeof(GeneralFollowerDTO)); }
		}
	}

	[Serializable]
	public class GeneralFollowerQueryResult : QueryResult<GeneralFollowerDTO>, ISagaMessage
	{
	}
}
