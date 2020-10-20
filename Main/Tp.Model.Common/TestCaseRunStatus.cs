namespace Tp.Model.Common
{
    public enum TestCaseRunStatus
    {
        NotRun = 0,
        Passed = 1,
        Failed = 2,
        OnHold = 3,
        Blocked = 4
    }

    public static class TestCaseRunStatusExtenstions
    {
        public static bool IsFinalState(this TestCaseRunStatus status)
        {
            return status == TestCaseRunStatus.Passed || status == TestCaseRunStatus.Failed;
        }
    }
}
