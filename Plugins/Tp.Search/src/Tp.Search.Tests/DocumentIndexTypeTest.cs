using System;
using System.Linq;
using NUnit.Framework;
using Tp.Integration.Common;
using Tp.Search.Model.Document;

namespace Tp.Search.Tests
{
    [TestFixture]
    [Category("PartPlugins1")]
    public class DocumentIndexTypeTest : SearchTestBase
    {
        [Test]
        public void Test()
        {
            var metadata = new DocumentIndexMetadata();
            var fields = metadata.DocumentIndexTypes.SelectMany(t => t.DocumentFields.Concat(t.IndexFields));
            foreach (var f in fields.OfType<AssignableField>().Distinct())
            {
                Console.WriteLine(f);
            }
        }
    }
}
