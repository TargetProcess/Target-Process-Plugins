using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Queries
{
    [Serializable]
    public class PriorityQuery : QueryBase
    {
        public override DtoType DtoType => new DtoType(typeof(PriorityDTO));

        public string EntityType { get; set; }
    }

    [Serializable]
    public class PriorityQueryResult : QueryResult<PriorityDTO>, ISagaMessage
    {
    }
}
