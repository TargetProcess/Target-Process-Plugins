using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Queries
{
    [Serializable]
    public class TestCaseRunQuery : QueryBase
    {
        public int TestPlanRunId { get; set; }

        public override DtoType DtoType
        {
            get { return new DtoType(typeof(TestCaseRunDTO)); }
        }
    }

    [Serializable]
    public class TestCaseRunQueryResult : QueryResult<TestCaseRunDTO>, ISagaMessage
    {
    }
}
