using NUnit.Framework;
using Tp.Search.Model.Document;
using Tp.Testing.Common.NUnit;

namespace Tp.Search.Tests
{
    [TestFixture]
    [Category("PartPlugins1")]
    public class DocumentIdFactoryTests
    {
        [Test]
        public void CreateProjectIdTest()
        {
            var f = new DocumentIdFactory();
            const int projectId = 200;
            string s = f.CreateProjectId(projectId);
            int parsed = f.ParseProjectId(s);
            parsed.Should(Be.EqualTo(projectId), "parsed.Should(Be.EqualTo(projectId))");
        }

        [Test]
        public void CreateProjectZeroTest()
        {
            var f = new DocumentIdFactory();
            const int projectId = 0;
            string s = f.CreateProjectId(projectId);
            int parsed = f.ParseProjectId(s);
            parsed.Should(Be.EqualTo(projectId), "parsed.Should(Be.EqualTo(projectId))");
        }
    }
}
