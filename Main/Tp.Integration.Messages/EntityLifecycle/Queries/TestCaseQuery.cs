using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Queries
{
    [Serializable]
    public class TestCaseQuery : QueryBase
    {
        public override DtoType DtoType
        {
            get { return new DtoType(typeof(TestCaseDTO)); }
        }
    }

    [Serializable]
    public class TestCaseQueryResult : QueryResult<TestCaseDTO>, ISagaMessage
    {
    }
}
