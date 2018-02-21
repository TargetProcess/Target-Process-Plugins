// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using Tp.Integration.Common;
using Tp.Integration.Plugin.TestRunImport.TestRunImport;

namespace Tp.Integration.Plugin.TestRunImport.TestCaseResolvers
{
    /// <summary>
    /// Creates new AbstractTestCaseResolver upon request.
    /// </summary>
    public interface ITestCaseResolverFactory
    {
        /// <summary>
        /// Creates new instance if AbstractTestCaseResolver for the specified settings.
        /// </summary>
        /// <param name="settings">
        /// Current test run import plugin settings.
        /// </param>
        /// <param name="resultInfos">
        /// Collection of TestRunImportResultInfos containing information about test run results.
        /// </param>
        /// <param name="testCaseTestPlans">
        /// Collection of TestCaseTestPlanDTOs for current plugin TestPlan.
        /// </param>
        /// <returns>
        /// AbstractTestCaseResolver
        /// </returns>
        AbstractTestCaseResolver GetResolver(TestRunImportSettings settings,
            IEnumerable<TestRunImportResultInfo> resultInfos,
            IEnumerable<TestCaseTestPlanDTO> testCaseTestPlans);
    }
}
