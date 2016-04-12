using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Queries
{
	[Serializable]
	public class RetrieveAllRolesQuery : QueryBase
	{
		public int ProjectId { get; set; }

		public override DtoType DtoType
		{
			get { return new DtoType(typeof(RoleDTO)); }
		}
	}

	[Serializable]
	public class RoleQueryResult : QueryResult<RoleDTO>, ISagaMessage
	{
	}
}
