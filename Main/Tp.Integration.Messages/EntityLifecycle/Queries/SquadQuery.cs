using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Queries
{
    [Serializable]
    public class RetrieveAllSquadsQuery : QueryBase
    {
        public override DtoType DtoType => new DtoType(typeof(SquadDTO));
    }

    [Serializable]
    public class SquadsResult : QueryResult<SquadDTO>, ISagaMessage { }
}
