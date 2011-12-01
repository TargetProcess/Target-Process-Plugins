// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Tp.Integration.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.TestRunImport.TestRunImport;

namespace Tp.Integration.Plugin.TestRunImport.TestCaseResolvers
{
	public class RegExpTestCaseResolver : SimpleTestCaseResolver
	{
		private readonly IDictionary<int, TestCaseTestPlanDTO> _testCasesById = new Dictionary<int, TestCaseTestPlanDTO>();
		private readonly Regex _rx;

		protected internal RegExpTestCaseResolver(IActivityLogger log,
												  TestRunImportSettings settings,
												  IEnumerable<TestRunImportResultInfo> resultInfos,
												  IEnumerable<TestCaseTestPlanDTO> testCaseTestPlans
			)
			: base(log, settings, resultInfos, testCaseTestPlans)
		{
			if (settings.RegExp != null)
			{
				_rx = new Regex(settings.RegExp, RegexOptions.Compiled | RegexOptions.IgnoreCase);
			}
		}

		protected override void Add(TestCaseTestPlanDTO testCaseTestPlanDto)
		{
			_testCasesById.Add(testCaseTestPlanDto.TestCaseID.Value, testCaseTestPlanDto);
			base.Add(testCaseTestPlanDto);
		}

		protected override IEnumerable<TestCaseTestPlanDTO> ResolveTestCases(string testName)
		{
			if (_rx != null)
			{
				MatchCollection matches = _rx.Matches(testName);
				foreach (Match match in matches)
				{
					GroupCollection groups = match.Groups;
					Group g1 = groups[TestRunImportPluginProfile.PatternTestIdGroupName];
					if (g1.Success)
					{
						int id;
						string s = g1.Value;
						if (int.TryParse(s, NumberStyles.AllowLeadingWhite | NumberStyles.AllowLeadingWhite,
										 NumberFormatInfo.InvariantInfo, out id))
						{
							TestCaseTestPlanDTO testCaseTestPlan;
							if (_testCasesById.TryGetValue(id, out testCaseTestPlan))
							{
								yield return testCaseTestPlan;
							}
						}
					}
					else
					{
						Group g2 = groups[TestRunImportPluginProfile.PatternTestNameGroupName];
						if (g2.Success)
						{
							var s = g2.Value;
							var testCases = InvokeResolveTestCasesBase(s);
							foreach (var testCase in testCases)
							{
								yield return testCase;
							}
						}
					}
				}
			}
		}

		private IEnumerable<TestCaseTestPlanDTO> InvokeResolveTestCasesBase(string s)
		{
			return base.ResolveTestCases(s);
		}
	}
}