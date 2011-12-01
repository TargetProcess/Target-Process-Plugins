// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using System.Collections.Generic;
using System.Linq;
using Tp.Integration.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.TestRunImport.TestRunImport;

namespace Tp.Integration.Plugin.TestRunImport.TestCaseResolvers
{
	public class SimpleTestCaseResolver : AbstractTestCaseResolver
	{
		private readonly IDictionary<string, TestCaseTestPlanDTO[]> _testCasesByName =
			new Dictionary<string, TestCaseTestPlanDTO[]>();

		protected internal SimpleTestCaseResolver(IActivityLogger log,
												  TestRunImportSettings settings,
												  IEnumerable<TestRunImportResultInfo> resultInfos,
												  IEnumerable<TestCaseTestPlanDTO> testCaseTestPlans)
			: base(log, settings, resultInfos, testCaseTestPlans)
		{
		}

		protected override void Add(TestCaseTestPlanDTO testCaseTestPlanDto)
		{
			var testCaseName = NormalizeName(testCaseTestPlanDto.TestCaseName);
			if (_testCasesByName.ContainsKey(testCaseName))
			{
				_testCasesByName[testCaseName] =
					new List<TestCaseTestPlanDTO>(_testCasesByName[testCaseName]) { testCaseTestPlanDto }.ToArray();
			}
			else
			{
				_testCasesByName.Add(testCaseName, new[] { testCaseTestPlanDto });
			}
		}

		protected override IEnumerable<TestCaseTestPlanDTO> ResolveTestCases(string testName)
		{
			TestCaseTestPlanDTO[] testCases;

			if (_testCasesByName.TryGetValue(NormalizeName(testName), out testCases))
			{
				foreach (var testCase in testCases)
				{
					yield return testCase;
				}
				yield break;
			}

			var shortName = testName;
			var dotIndex = testName.LastIndexOf('.');
			if (dotIndex != -1)
			{
				shortName = testName.Substring(dotIndex + 1);
			}

			if (_testCasesByName.TryGetValue(NormalizeName(shortName), out testCases))
			{
				foreach (var testCase in testCases)
				{
					yield return testCase;
				}
				yield break;
			}
		}

		/// <summary>
		/// Remove any whitespace and punctuation from the specified name, convert result to lower case.
		/// </summary>
		/// <param name="name">Original, human readable name.</param>
		/// <returns>Mangled name for comparison.</returns>
		private static string NormalizeName(string name)
		{
			var result = new char[name.Length];
			var off = 0;
			foreach (var c in name.Where(Char.IsLetterOrDigit))
			{
				result[off++] = Char.ToLowerInvariant(c);
			}
			return new string(result, 0, off);
		}
	}
}