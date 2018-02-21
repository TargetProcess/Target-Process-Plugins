using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Queries
{
    [Serializable]
    public class SquadIterationQuery : QueryBase
    {
        public override DtoType DtoType
        {
            get { return new DtoType(typeof(SquadIterationDTO)); }
        }
    }

    [Serializable]
    public class SquadIterationQueryResult : QueryResult<SquadIterationDTO>, ISagaMessage
    {
    }
}
