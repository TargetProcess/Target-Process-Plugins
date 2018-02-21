using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Queries
{
    [Serializable]
    public class ImpedimentQuery : QueryBase
    {
        public override DtoType DtoType
        {
            get { return new DtoType(typeof(ImpedimentDTO)); }
        }
    }

    [Serializable]
    public class ImpedimentQueryResult : QueryResult<ImpedimentDTO>, ISagaMessage
    {
    }
}
