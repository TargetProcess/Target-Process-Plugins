// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System.Collections.Generic;
using Tp.Integration.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.TestRunImport.TestRunImport;

namespace Tp.Integration.Plugin.TestRunImport.TestCaseResolvers
{
	public class SimpleTestCaseResolverFactory : ITestCaseResolverFactory
	{
		private readonly IActivityLogger _log;

		public SimpleTestCaseResolverFactory(IActivityLogger log)
		{
			_log = log;
		}

		public AbstractTestCaseResolver GetResolver(TestRunImportSettings settings, IEnumerable<TestRunImportResultInfo> resultInfos, IEnumerable<TestCaseTestPlanDTO> testCaseTestPlans)
		{
			return string.IsNullOrEmpty(settings.RegExp)
					? new SimpleTestCaseResolver(_log, settings, resultInfos, testCaseTestPlans)
					: new RegExpTestCaseResolver(_log, settings, resultInfos, testCaseTestPlans);
		}
	}
}