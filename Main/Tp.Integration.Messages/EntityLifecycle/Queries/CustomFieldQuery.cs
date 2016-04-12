using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Queries
{
	[Serializable]
	public class CustomFieldQuery : QueryBase
	{
		public string Hql { get; set; }
		public object[] Params { get; set; }

		public override DtoType DtoType
		{
			get { return new DtoType(typeof(CustomFieldDTO)); }
		}
	}

	[Serializable]
	public class CustomFieldQueryResult : QueryResult<CustomFieldDTO>, ISagaMessage
	{
	}
}
