using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Queries
{
    [Serializable]
    public class RetrieveAllSquadMembersQuery : QueryBase
    {
        public override DtoType DtoType => new DtoType(typeof(SquadMemberDTO));
    }

    [Serializable]
    public class SquadMembersResult : QueryResult<SquadMemberDTO>, ISagaMessage { }
}
