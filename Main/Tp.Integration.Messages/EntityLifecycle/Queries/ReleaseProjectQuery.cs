using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Queries
{
    [Serializable]
    public class ReleaseProjectQuery : QueryBase
    {
        public override DtoType DtoType
        {
            get { return new DtoType(typeof(ReleaseProjectDTO)); }
        }
    }

    [Serializable]
    public class ReleaseProjectQueryResult : QueryResult<ReleaseProjectDTO>, ISagaMessage
    {
    }
}
