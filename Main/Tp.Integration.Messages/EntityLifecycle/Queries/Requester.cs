using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Queries
{
    [Serializable]
    public class RetrieveAllRequestersQuery : QueryBase
    {
        public override DtoType DtoType
        {
            get { return new DtoType(typeof(RequesterDTO)); }
        }
    }

    [Serializable]
    public class RequesterQueryResult : QueryResult<RequesterDTO>, ISagaMessage
    {
    }
}
