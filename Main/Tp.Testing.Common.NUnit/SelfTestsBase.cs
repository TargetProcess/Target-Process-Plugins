using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Tp.Testing.Common.NUnit
{
    public abstract class SelfTestsBase
    {
        protected readonly IReadOnlyCollection<Type> TestTypes;
        protected readonly IReadOnlyCollection<MethodInfo> TestMethods;

        protected SelfTestsBase()
        {
            var assembly = GetType().Assembly;

            var testTypes = new List<Type>();
            var testMethods = new List<MethodInfo>();

            foreach (var type in assembly.GetExportedTypes().Where(type => type.IsAbstract == false))
            {
                var testMethodsInType = type.GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(IsTestMethod).ToList();
                if (testMethodsInType.Any())
                {
                    testTypes.Add(type);
                    testMethods.AddRange(testMethodsInType);
                }
            }
            TestTypes = testTypes;
            TestMethods = testMethods;
        }

        [Test]
        public void AllTestFixturesShouldHaveCategoryTest()
        {
            TestTypes.Where(type => type.GetCustomAttributes<CategoryAttribute>().All(x => !x.Name.StartsWith("Part")))
                .Select(x => x.FullName)
                .Should(Be.Empty, "These test fixtures do not have [\"Category(\"Part*\")\"] attribute");
        }

        [Test]
        public void AllTestFixturesShouldHaveTestFixtureAttributeTest()
        {
            TestTypes.Where(type => !type.GetCustomAttributes<TestFixtureAttribute>(true).Any())
                .Select(x => x.FullName)
                .Should(Be.Empty, "These test fixtures do not have [\"TestFixture\"] attribute");
        }

        [Test]
        public void AllIgnoredTestCasesHaveReasonsTest()
        {
            var ignoredTestCases = TestMethods
                .Where(mi => mi.GetCustomAttributes<TestCaseAttribute>()
                    .Any(x => (x.Ignore || x.Ignored) && x.Reason == null));

            ignoredTestCases.Select(x => $"{x.DeclaringType?.FullName}::{x.Name}")
                .Should(Be.Empty, "These test methods have ignored test cases without a reason defined - NUnit xml writer will fail");
        }

        private static bool IsTestMethod(MethodInfo method)
        {
            return method.GetCustomAttributes()
                .Select(x => x.GetType())
                .Any(
                    x => typeof(TestAttribute).IsAssignableFrom(x) || x == typeof(TestCaseAttribute) || x == typeof(TestCaseSourceAttribute));
        }
    }
}
