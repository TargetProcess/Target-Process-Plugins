// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Validation;
using Tp.Integration.Plugin.TestRunImport.Commands.Data;
using Tp.Integration.Plugin.TestRunImport.TestRunImport;
using Tp.Integration.Common;
using Tp.Integration.Plugin.TestRunImport.Mappers;

namespace Tp.Integration.Plugin.TestRunImport.TestCaseResolvers
{
    public abstract class AbstractTestCaseResolver
    {
        private readonly IActivityLogger _log;

        protected AbstractTestCaseResolver(IActivityLogger log, TestRunImportSettings settings,
            IEnumerable<TestRunImportResultInfo> resultInfos,
            IEnumerable<TestCaseTestPlanDTO> testCaseTestPlans)
        {
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }
            _log = log;
            if (settings == null)
            {
                _log.Error("Ctor member TestRunImportSettings settings is null");
                throw new ArgumentNullException(nameof(settings));
            }
            if (testCaseTestPlans == null)
            {
                _log.Error("Ctor member IEnumerable<TestCaseTestPlanDTO> testCaseTestPlans is null");
                throw new ArgumentNullException(nameof(testCaseTestPlans));
            }
            TestCaseTestPlans = testCaseTestPlans;
            if (resultInfos == null)
            {
                _log.Error("Ctor member IEnumerable<TestRunImportResultInfo> resultInfos is null");
                throw new ArgumentNullException(nameof(resultInfos));
            }
            ResultInfos = resultInfos;
        }

        protected IActivityLogger Log => _log;

        private IEnumerable<TestRunImportResultInfo> ResultInfos { get; set; }
        private IEnumerable<TestCaseTestPlanDTO> _testCaseTestPlans;

        private IEnumerable<TestCaseTestPlanDTO> TestCaseTestPlans
        {
            get { return _testCaseTestPlans; }
            set
            {
                _testCaseTestPlans = value;
                _testCaseTestPlans.ForEach(Add);
            }
        }

        protected abstract void Add(TestCaseTestPlanDTO testCaseTestPlanDto);

        /// <summary>
        /// Resolves the specified name to a collection of test cases matching the specified name.
        /// </summary>
        /// <param name="testName">Name of test case.</param>
        /// <returns>An enuberable over the resolved test cases.</returns>
        protected abstract IEnumerable<TestCaseTestPlanDTO> ResolveTestCases(string testName);

        public CheckMappingResult ResolveTestCaseNames(PluginProfileErrorCollection errors)
        {
            var result = new CheckMappingResult { Errors = errors, NamesMappers = new List<NamesMapper>() };
            if (errors.Count() == 0)
            {
                var mapped = new List<KeyValuePair<int, NamesMapper>>();
                var notMapped = new List<NamesMapper>();
                foreach (var info in ResultInfos)
                {
                    var resolved = ResolveTestCases(info.Name).ToList();
                    if (resolved.Count == 0)
                    {
                        notMapped.Add(new NamesMapper(string.Empty, info.Name));
                        result.NotMappedUnitTests++;
                        continue;
                    }
                    TestRunImportResultInfo resultInfo = info;
                    resolved.ForEach(
                        r =>
                            mapped.Add(new KeyValuePair<int, NamesMapper>(r.TestCaseID.GetValueOrDefault(),
                                new NamesMapper(
                                    string.Format("#{0} - {1}", r.TestCaseID, r.TestCaseName),
                                    resultInfo.Name))));
                    result.Mapped += resolved.Count;
                }
                mapped.OrderBy(x => x.Key).ForEach(o => result.NamesMappers.Add(o.Value));
                result.OverMappedUnitTests = mapped.GroupBy(x => x.Key).Where(g => g.Count() > 1).Count();
                foreach (
                    var tt in
                    TestCaseTestPlans.Where(t => mapped.FirstOrDefault(x => x.Key == t.TestCaseID.GetValueOrDefault()).Value == null)
                        .OrderBy(x => x.TestCaseID.GetValueOrDefault()))
                {
                    result.NamesMappers.Add(new NamesMapper(
                        string.Format("#{0} - {1}", tt.TestCaseID, tt.TestCaseName),
                        string.Empty));
                    result.NotMappedTestCases++;
                }
                notMapped.ForEach(x => result.NamesMappers.Add(x));
            }
            return result;
        }

        public IEnumerable<TestCaseRunDTO> ImportResultsForTestCaseRuns(IEnumerable<TestCaseRunDTO> testCaseRunDtos,
            Action<string> found, Action<string> notFound)
        {
            foreach (var info in ResultInfos)
            {
                var resolved = ResolveTestCases(info.Name).Select(
                    dto => testCaseRunDtos.FirstOrDefault(t => t.TestCaseTestPlanID == dto.TestCaseTestPlanID)).Where(
                    testCaseRunDto => testCaseRunDto != null).ToList();

                if (!resolved.Any())
                {
                    notFound.Invoke(info.Name);
                    continue;
                }

                foreach (var dto in resolved)
                {
                    found.Invoke(info.Name);
                    dto.Passed = info.IsSuccess;
                    if (dto.Passed == null)
                    {
                        dto.Status = TestCaseRunStatusDTO.NotRun;
                    }
                    else
                    {
                        dto.Status = dto.Passed == true ? TestCaseRunStatusDTO.Passed : TestCaseRunStatusDTO.Failed;
                    }
                    dto.RunDate = info.RunDate;
                    dto.Comment = (info.IsSuccess.HasValue && !info.IsSuccess.Value && !string.IsNullOrEmpty(info.Comment))
                        ? info.Comment
                        : null;
                    dto.Runned = true;
                }
            }
            return testCaseRunDtos;
        }
    }
}
