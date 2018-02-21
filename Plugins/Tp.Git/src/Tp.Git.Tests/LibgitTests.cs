using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;
using NUnit.Framework;
using Tp.Core;
using Tp.Git.VersionControlSystem;
using Tp.Integration.Common;
using Tp.Integration.Plugin.Common.Domain;
using Tp.SourceControl.VersionControlSystem;
using Tp.Testing.Common.NUnit;

namespace Tp.Git.Tests
{
    [TestFixture]
    public class LibgitTests
    {
        public class Storage : List<GitRepositoryFolder>, IStorage<GitRepositoryFolder>
        {
            public bool IsNull => false;

            public void ReplaceWith(params GitRepositoryFolder[] value)
            {
                Clear();
                AddRange(value);
            }

            public void Update(GitRepositoryFolder value, Predicate<GitRepositoryFolder> condition)
            {
                if (this.Any(x => condition(x)))
                {
                    Remove(condition);
                    Add(value);
                }
            }

            public void Remove(Predicate<GitRepositoryFolder> condition)
            {
                var values = this.Where(x => !condition(x)).ToArray();
                Clear();
                AddRange(values);
            }
        }

        [Test]
        public void ShouldCloneOnConstruction()
        {
            var storage = new Storage();            
            var client = new LibgitClient(new GitPluginProfile { Uri = ExtractTestRepository("Clone"), Login = "", Password = ""}, storage);
            var ranges = client.GetFromTillHead(new DateTime(2010, 1, 1), 10);
            Assert.AreEqual(1, storage.Count);
            Assert.IsNotEmpty(ranges);
        }

        [Test]
        public void ShouldNotReturnMergeCommits()
        {            
            var storage = new Storage();
            var client = new LibgitClient(new GitPluginProfile { Uri = ExtractTestRepository("MergeCommits"), Login = "", Password = "" }, storage);
            Assert.AreEqual(4, client.GetFromTillHead(new DateTime(2010, 1, 1), 1).Count());
            Assert.AreEqual(4, client.GetAfterTillHead(new RevisionId { Time = new DateTime(2010, 1, 1) }, 1).Count());
            Assert.AreEqual(4, client.GetFromAndBefore(new RevisionId { Time = new DateTime(2010, 1, 1) }, new RevisionId { Time = new DateTime(2020, 1, 1) }, 1).Count());

            client.GetRevisions(new RevisionId { Time = new DateTime(2010, 1, 1) }, new RevisionId { Time = new DateTime(2020, 1, 1) })
                .Select(r => r.Comment).Should(Be.EquivalentTo(new[] { "Commit1", "Commit2", "Commit3", "Commit4" }), "");
        }

        [Test]
        public void ShouldUseAuthorAndCommitterRightly()
        {
            var storage = new Storage();
            var client = new LibgitClient(new GitPluginProfile { Uri = ExtractTestRepository("AuthorVsCommitter"), Login = "", Password = "" }, storage);
            var revs = client.GetRevisions(new RevisionId { Time = new DateTime(2010, 1, 1) }, new RevisionId { Time = new DateTime(2020, 1, 1) }).ToDictionary(r => r.Id);

            Assert.AreEqual(3, revs.Count);

            Assert.AreEqual("Kruglik Alexey", revs["efab82a90bcd2b4e7403329b338d9d9c32b277e1"].Author);
            Assert.AreEqual("kruglik@targetprocess.com", revs["efab82a90bcd2b4e7403329b338d9d9c32b277e1"].Email);
            Assert.AreEqual(new DateTime(2017, 10, 2, 10, 27, 20), revs["efab82a90bcd2b4e7403329b338d9d9c32b277e1"].Time);
            Assert.AreEqual(new DateTime(2017, 10, 2, 10, 27, 20), revs["efab82a90bcd2b4e7403329b338d9d9c32b277e1"].TimeCreated);

            Assert.AreEqual("Vasya Pupkin", revs["a66881e70e377b9f81815169789c4b700221c8eb"].Author);
            Assert.AreEqual("vasya@example.com", revs["a66881e70e377b9f81815169789c4b700221c8eb"].Email);
            Assert.AreEqual(new DateTime(2017, 10, 2, 11, 36, 43), revs["a66881e70e377b9f81815169789c4b700221c8eb"].Time);
            Assert.AreEqual(new DateTime(2017, 10, 2, 11, 34, 52), revs["a66881e70e377b9f81815169789c4b700221c8eb"].TimeCreated);

            Assert.AreEqual("Vasya Pupkin", revs["a52f600576750e300d2fa0170bdf76bb21929d98"].Author);
            Assert.AreEqual("vasya@example.com", revs["a52f600576750e300d2fa0170bdf76bb21929d98"].Email);
            Assert.AreEqual(new DateTime(2017, 10, 2, 11, 34, 52), revs["a52f600576750e300d2fa0170bdf76bb21929d98"].Time);
            Assert.AreEqual(new DateTime(2017, 10, 2, 11, 34, 52), revs["a52f600576750e300d2fa0170bdf76bb21929d98"].TimeCreated);

            client.RetrieveAuthors(new DateRange(new DateTime(2010, 1, 1), new DateTime(2020, 1, 1))).Should(Be.EquivalentTo(new []{ "Kruglik Alexey", "Vasya Pupkin" }), "");
        }

        private static string ExtractTestRepository(string name)
        {
            var executingPath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            var zip = Path.Combine(executingPath, "TestRepos", $"{name}.zip");
            var dest = Path.Combine(executingPath, "TestRepos", Guid.NewGuid().ToString());
            var fastZip = new FastZip { CreateEmptyDirectories = true };
            fastZip.ExtractZip(zip, dest, string.Empty);
            return dest;
        }

        [Test]
        [Explicit]
        // This test checks libgit implementation against ngit on a slice of real world TP repository
        // Make change to GitRepositoryFolder.GetAbsolutePath so it always return path to your working copy root. Repository uri do not matter.
        public void NGitVsLibgitTest()
        {
            var storage = new Storage();
            var profile = new GitPluginProfile { Uri = "c:\\foo", Login = "", Password = "" };
            storage.Add(new GitRepositoryFolder { RepoUri = profile.Uri });
            var ngit = new NGitClient(profile, storage);
            var libgit = new LibgitClient(profile, storage);

            var ngitRevs = ngit.GetRevisions(new RevisionId { Time = new DateTime(2017, 01, 01) }, new RevisionId { Time = new DateTime(2017, 10, 01) }).OrderBy(r => r.Id.Value).ToArray();
            var libgitRevs = libgit.GetRevisions(new RevisionId { Time = new DateTime(2017, 01, 01) }, new RevisionId { Time = new DateTime(2017, 10, 01) }).OrderBy(r => r.Id.Value).ToArray();

            Assert.AreEqual(ngitRevs.Length, libgitRevs.Length);
            for (int i = 0; i < ngitRevs.Length; i++)
            {
                Assert.AreEqual(ngitRevs[i].Id.Value, libgitRevs[i].Id.Value);
                Assert.AreEqual(ngitRevs[i].Time, libgitRevs[i].Time);
                Assert.AreEqual(ngitRevs[i].Id.Time, libgitRevs[i].Id.Time);
                Assert.AreEqual(ngitRevs[i].Author, libgitRevs[i].Author);
                Assert.AreEqual(ngitRevs[i].Comment, libgitRevs[i].Comment);
                Assert.AreEqual(ngitRevs[i].Email, libgitRevs[i].Email);
                Assert.AreEqual(ngitRevs[i].TimeCreated, libgitRevs[i].TimeCreated);

                var ngitEntries = ngitRevs[i].Entries.OrderBy(x => x.Path).ToArray();
                var libGitEntries = libgitRevs[i].Entries.OrderBy(x => x.Path).ToArray();

                var libgitRenames = libGitEntries.Where(e => e.Action == FileActionEnum.Rename).ToArray();
                var ngitRenames = ngitEntries.Where(e => e.Action == FileActionEnum.Rename).ToDictionary(x => x.Path);
                var ngitAdds = ngitEntries.Where(e => e.Action == FileActionEnum.Add).ToDictionary(x => x.Path);
                var ngitDeletes = ngitEntries.Where(e => e.Action == FileActionEnum.Delete).ToDictionary(x => x.Path);

                var processedNgit = new List<RevisionEntryInfo>();

                // libgit diff algorithm detects more renames than ngit one
                libgitRenames.ForEach(r =>
                {
                    if (!ngitRenames.ContainsKey(r.Path))
                    {
                        var parts = r.Path.Split(new[] { " -> " }, StringSplitOptions.None);
                        if (!ngitAdds.ContainsKey(parts[1]) && !ngitDeletes.ContainsKey(parts[0]))
                        {
                            Assert.Fail();
                        }
                        processedNgit.Add(ngitAdds[parts[1]]);
                        processedNgit.Add(ngitDeletes[parts[0]]);
                    }
                    else
                    {
                        processedNgit.Add(ngitRenames[r.Path]);
                    }
                });

                ngitEntries = ngitEntries.Except(processedNgit).ToArray();
                libGitEntries = libGitEntries.Except(libgitRenames).ToArray();

                Assert.AreEqual(ngitEntries.Length, libGitEntries.Length);
                for (int j = 0; j < ngitEntries.Length; j++)
                {
                    Assert.AreEqual(ngitEntries[j].Path, libGitEntries[j].Path);
                    Assert.AreEqual(ngitEntries[j].Action, libGitEntries[j].Action);
                }
            }
        }
    }    
}
