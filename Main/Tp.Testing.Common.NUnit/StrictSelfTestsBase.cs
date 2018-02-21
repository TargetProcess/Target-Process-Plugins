using System.Linq;
using NUnit.Framework;

namespace Tp.Testing.Common.NUnit
{
    public abstract class StrictSelfTestsBase : SelfTestsBase
    {
        [Test]
        public void AllTestTypesHaveCorrectSuffixesTest()
        {
            TestTypes.Where(x => !(x.Name.EndsWith("Specs") || x.Name.EndsWith("Tests")))
                .Select(x => x.FullName)
                .Should(Be.Empty, "These test fixtures do not have correct suffix - 'Specs' or 'Tests'");
        }

        [Test]
        public void AllTestMethodsHaveCorrectSuffixesTest()
        {
            TestMethods.Where(x => !(x.Name.EndsWith("Spec") || x.Name.EndsWith("Test")))
                .Select(x => $"{x.DeclaringType?.FullName}::{x.Name}")
                .Should(Be.Empty, "These test methods do not have correct suffix - 'Spec' ot 'Test'");
        }
    }
}
