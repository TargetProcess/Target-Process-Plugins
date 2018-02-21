using NUnit.Framework;
using Tp.Core;
using Tp.Mercurial.RevisionStorage;
using Tp.SourceControl.VersionControlSystem;
using Tp.Testing.Common.NUnit;

namespace Tp.Mercurial.Tests.RevisionStorageTests
{
    [TestFixture]
    [Category("PartPlugins1")]
    public class MercurialRevisionStorageRepositoryTests
    {
        [Test]
        [TestCase("not so big comment")]
        [TestCase("Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.")]
        public void RevisionInfoKeyShouldNotBeGreaterThat(string revisionInfoComment)
        {
            var revisionInfoKey = new MercurialRevisionStorageRepository(null, null).GetRevisionInfoKey(new RevisionInfo
            {
                Author = "User1",
                Email = "someone@somewhere.com",
                Id = "48588d3bef746129aa66e5fa915da2962a1a4014",
                TimeCreated = CurrentDate.Value.AddMonths(-1),
                Comment = revisionInfoComment
            });

            revisionInfoKey.Length.Should(Be.LessThanOrEqualTo(255), "Revision info key should be less then or equal to 255");
        }
    }
}
