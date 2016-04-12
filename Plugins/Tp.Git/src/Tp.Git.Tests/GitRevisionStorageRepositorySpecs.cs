using NUnit.Framework;
using Tp.Core;
using Tp.Git.RevisionStorage;
using Tp.SourceControl.VersionControlSystem;
using Tp.Testing.Common.NUnit;

namespace Tp.Git.Tests
{
    [TestFixture]
    [Category("PartPlugins1")]
	public class GitRevisionStorageRepositorySpecs
	{
		[Bug(54401)]
		public void ShouldGetKeyForCommitWithLongComment()
		{
			var revisionInfo = new RevisionInfo
				                   {
					                   Author = "User1",
					                   Email = "someone@somewhere.com",
					                   Id = "48588d3bef746129aa66e5fa915da2962a1a4014",
					                   TimeCreated = CurrentDate.Value.AddMonths(-1),
					                   Comment =
						                   "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum."
				                   };
			var key = GitRevisionStorageRepository.GetKey(revisionInfo);
			key.Length.Should(Be.EqualTo(255), "key.Length.Should(Be.EqualTo(255))");
		}
	}
}