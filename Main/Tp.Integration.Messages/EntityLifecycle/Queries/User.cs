using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Queries
{
    [Serializable]
    public class RetrieveAllUsersQuery : QueryBase
    {
        public override DtoType DtoType => new DtoType(typeof(UserDTO));
    }

    [Serializable]
    public class UserQueryResult : QueryResult<UserDTO>, ISagaMessage
    {
    }
}
