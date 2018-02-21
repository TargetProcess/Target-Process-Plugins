using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Queries
{
    [Serializable]
    public class TestCaseTestPlanQuery : QueryBase
    {
        public int TestPlanId { get; set; }

        public override DtoType DtoType
        {
            get { return new DtoType(typeof(TestCaseTestPlanDTO)); }
        }
    }

    [Serializable]
    public class TestCaseTestPlanQueryResult : QueryResult<TestCaseTestPlanDTO>, ISagaMessage
    {
    }
}
