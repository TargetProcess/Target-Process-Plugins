// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Linq;
using NUnit.Framework;
using StructureMap;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Messages.EntityLifecycle.Queries;
using Tp.Integration.Plugin.Common.Storage;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.TestRunImport;
using Tp.Integration.Plugin.TestRunImport.FrameworkTypes;
using Tp.Integration.Testing.Common;
using Tp.TestRunImport.Tests.Context;
using Tp.TestRunImport.Tests.StructureMap;
using Tp.Testing.Common.NUnit;

namespace Tp.TestRunImport.Tests
{
    [TestFixture]
    [Category("PartPlugins1")]
    public class TestRunImportPluginProfileInitializationSagaSpecs
    {
        private TestRunImportPluginProfile _profileSettings;

        private static readonly TestCaseTestPlanDTO CaseTestPlanDto1 = new TestCaseTestPlanDTO
        {
            TestCaseName = "Test Case 1",
            TestPlanID = 4,
            ID = 1
        };

        private static readonly TestCaseTestPlanDTO CaseTestPlanDto2 = new TestCaseTestPlanDTO
        {
            TestCaseName = "Test Case 2",
            TestPlanID = 4,
            ID = 2
        };

        private static readonly TestCaseTestPlanDTO CaseTestPlanDto3 = new TestCaseTestPlanDTO
        {
            TestCaseName = "Test Case 3",
            TestPlanID = 4,
            ID = 3
        };

        [SetUp]
        public void Init()
        {
            ObjectFactory.Initialize(x => x.AddRegistry<TestRunImportEnvironmentRegistry>());
            ObjectFactory.Configure(
                x =>
                    x.For<TransportMock>()
                        .Use(TransportMock.CreateWithoutStructureMapClear(typeof(TestRunImportPluginProfileInitializationSagaData).Assembly)));

            _profileSettings = new TestRunImportPluginProfile
            {
                PassiveMode = false,
                Project = 1,
                ResultsFilePath = "C:\\SimpleTestCaseTestResult.xml",
                TestPlan = 1,
                FrameworkType = FrameworkTypes.NUnit
            };
        }

        [Test]
        public void WhenProfileInitializedTestCaseTestPlanShouldBeSavedInRepository()
        {
            var queryResult1 = new TestCaseTestPlanQueryResult { Dtos = new[] { CaseTestPlanDto1 }, QueryResultCount = 3 };
            var queryResult2 = new TestCaseTestPlanQueryResult { Dtos = new[] { CaseTestPlanDto2 }, QueryResultCount = 3 };
            var queryResult3 = new TestCaseTestPlanQueryResult { Dtos = new[] { CaseTestPlanDto3 }, QueryResultCount = 3 };
            Context.Transport.On<TestCaseTestPlanQuery>().Reply(x => new ISagaMessage[] { queryResult1, queryResult2, queryResult3 });

            var profile = Context.Transport.AddProfile("Profile_1", _profileSettings);

            profile.Initialized.Should(Be.True, "profile.Initialized.Should(Be.True)");
            AssertUserExistInProfile(CaseTestPlanDto1, profile);
            AssertUserExistInProfile(CaseTestPlanDto2, profile);
            AssertUserExistInProfile(CaseTestPlanDto3, profile);
        }

        private static void AssertUserExistInProfile(TestCaseTestPlanDTO testCaseTestPlanDto, IStorageRepository storageRepository)
        {
            storageRepository.Get<TestCaseTestPlanDTO>(testCaseTestPlanDto.ID.ToString())
                .SingleOrDefault()
                .Should(Be.Not.Null,
                    "storageRepository.Get<TestCaseTestPlanDTO>(testCaseTestPlanDto.ID.ToString()).SingleOrDefault().Should(Be.Not.Null)");
        }

        public TestRunImportPluginContext Context => ObjectFactory.GetInstance<TestRunImportPluginContext>();
    }
}
