using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Queries
{
	[Serializable]
	public class RetrieveAllProcessQuery : QueryBase
	{
		public override DtoType DtoType
		{
			get { return new DtoType(typeof(ProcessDTO)); }
		}
	}

	[Serializable]
	public class ProcessQueryResult : QueryResult<ProcessDTO>, ISagaMessage
	{
	}
}
