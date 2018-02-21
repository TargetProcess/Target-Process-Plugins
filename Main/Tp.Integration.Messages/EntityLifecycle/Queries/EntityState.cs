using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Queries
{
    [Serializable]
    public class EntityStateQuery : QueryBase
    {
        public int ProjectId { get; set; }

        public override DtoType DtoType
        {
            get { return new DtoType(typeof(EntityStateDTO)); }
        }
    }

    [Serializable]
    public class EntityStateQueryResult : QueryResult<EntityStateDTO>, ISagaMessage
    {
    }
}
