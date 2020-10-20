namespace Tp.Model.Common
{
    public static class SliceConstants
    {
        /// <summary>
        /// Special value that represents No Value coordinate in slice projections. Used in coordinate expressions and filters.
        /// Same hardcoded value used in TestRunItemTable.ParentTestPlanRunSliceID database column (if item has no ParentTestPlanRun)
        /// which represents pre-calculated TestPlanRun axis coordinate projection of test run item used for optimization.
        /// See adr/2020-07-13 - Test run item ID optimization.md
        /// </summary>
        public const string NO_VALUE_ID = "|na|";
    }
}
