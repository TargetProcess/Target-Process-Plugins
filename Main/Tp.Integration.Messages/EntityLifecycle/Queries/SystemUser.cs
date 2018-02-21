using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Queries
{
    [Serializable]
    public class RetrieveAllSystemUsersQuery : QueryBase
    {
        public override DtoType DtoType => new DtoType(typeof(SystemUserDTO));
    }

    [Serializable]
    public class SystemUserQueryResult : QueryResult<SystemUserDTO>, ISagaMessage
    {
    }
}
