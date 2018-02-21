using NUnit.Framework;
using Tp.Search.Model.Document;
using Tp.Testing.Common.NUnit;

namespace Tp.Search.Tests
{
    [TestFixture]
    [Category("PartPlugins1")]
    public class IndexDataStringServicesTests
    {
        [Test]
        public void EncodeDecodeTest()
        {
            string prefix = "Project";
            int targetId = 200;
            string encoded = IndexDataStringServices.EncodeStringId(targetId, prefix);
            int? decoded = IndexDataStringServices.DecodeStringId(encoded, prefix);
            targetId.Should(Be.EqualTo(decoded), "targetId.Should(Be.EqualTo(decoded))");
        }

        [Test]
        public void EncodeDecodeNullTest()
        {
            string prefix = "Project";
            int? targetId = null;
            string encoded = IndexDataStringServices.EncodeStringId(targetId, prefix);
            int? decoded = IndexDataStringServices.DecodeStringId(encoded, prefix);
            targetId.Should(Be.EqualTo(decoded), "targetId.Should(Be.EqualTo(decoded))");
        }
    }
}
