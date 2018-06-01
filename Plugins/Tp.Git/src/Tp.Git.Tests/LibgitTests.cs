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
    [Category("PartPlugins1")]
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
        public void TestLibgitBinaries()
        {
            // LibgitSharp-SSH uses old version of libgit2 which can't connect to Gitlab via ssh.
            // To fix this we build our own version of libgit2 using libgit2 commit 429bb3575474a3d25ee1c9814612d8d01b3378e8 and libssh2 commit 616fd4d1b3e4a55de67c48819fefca83132126b5
            // In special build task we delete all libgit2 binaries copied to build directory by LibgitSharp-SSH. Then we copy our binary in lib\win32\x64\ directory
            // using file name expected by LibgitSharp-SSH.
            // See ReplaceLibgitBinaries.props file

            var basePath = Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath), "lib",
                "win32");
            var x64Path = Path.Combine(basePath, "x64");
            var x32Path = Path.Combine(basePath, "x86");

            Assert.IsFalse(Directory.Exists(x32Path), "There should be no 32 bit libgit2 binaries");
            Directory.EnumerateFiles(x64Path).Select(Path.GetFileName).Should(Be.EquivalentTo("git2-ssh-baa87df.dll".Yield()),
                "There should be single libgit2 binary in 64 bit directory");
        }

        [Test]
        public void ShouldCloneOnConstruction()
        {
            var storage = new Storage();
            var client = new LibgitClient(new GitPluginProfile { Uri = ExtractTestRepository("Clone"), Login = "", Password = "" },
                storage);
            var ranges = client.GetFromTillHead(new DateTime(2010, 1, 1), 10);
            Assert.AreEqual(1, storage.Count);
            Assert.IsNotEmpty(ranges);
        }

        [Test]
        public void ShouldNotReturnMergeCommits()
        {
            var storage = new Storage();
            var client = new LibgitClient(new GitPluginProfile { Uri = ExtractTestRepository("MergeCommits"), Login = "", Password = "" },
                storage);
            Assert.AreEqual(4, client.GetFromTillHead(new DateTime(2010, 1, 1), 1).Count());
            Assert.AreEqual(4, client.GetAfterTillHead(new RevisionId { Time = new DateTime(2010, 1, 1) }, 1).Count());
            Assert.AreEqual(4,
                client.GetFromAndBefore(new RevisionId { Time = new DateTime(2010, 1, 1) },
                    new RevisionId { Time = new DateTime(2020, 1, 1) }, 1).Count());

            client.GetRevisions(new RevisionId { Time = new DateTime(2010, 1, 1) }, new RevisionId { Time = new DateTime(2020, 1, 1) })
                .Select(r => r.Comment).Should(Be.EquivalentTo(new[] { "Commit1", "Commit2", "Commit3", "Commit4" }), "");
        }

        [Test]
        public void ShouldUseAuthorAndCommitterRightly()
        {
            var storage = new Storage();
            var client = new LibgitClient(
                new GitPluginProfile { Uri = ExtractTestRepository("AuthorVsCommitter"), Login = "", Password = "" }, storage);
            var revs = client
                .GetRevisions(new RevisionId { Time = new DateTime(2010, 1, 1) }, new RevisionId { Time = new DateTime(2020, 1, 1) })
                .ToDictionary(r => r.Id);

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

            client.RetrieveAuthors(new DateRange(new DateTime(2010, 1, 1), new DateTime(2020, 1, 1)))
                .Should(Be.EquivalentTo(new[] { "Kruglik Alexey", "Vasya Pupkin" }), "");
        }

        [Test]
        [TestCase("git@gitlab.com:tpgitplugintest/tpgitplugintest.git")]
        [TestCase("git@github.com:tpgitplugintest/tpgitplugintest.git")]
        // Both github and gitlab test accounts has login tpgitplugintest@gmail.com and password q4mVjtjdUMhGV2w4StWst3ujzW87Fw
        public void ShouldCloneViaSsh(string url)
        {
            const string privateKey =
                @"-----BEGIN RSA PRIVATE KEY-----
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
-----END RSA PRIVATE KEY-----
";

            const string publicKey =
                @"ssh-rsa AAAAB3NzaC1yc2EAAAADAQABAAACAQDN4riruRKR6KenyKkXk7E5E/Uu/uKkDwsR1NAla63kyvid0CH9dXZwyaKUCvQp/Zjt/gGpHb4byX8Ml5XejSNUnucFS5WvEAmqLvXf6usQMW99yr09K3EF/KbcOBWwBsg5uGxBiMKr/Lish9VrUq0O2mps9lX6GH7CWe+1HBfgxlLxiInWYqTt/IpYfpy4/k5SpVwXg6UATK6naijpHi21U4v2wYDpK/J8iyB2Dx9aZj34fK5LDc4AQbp5LNfURWJAg7XEPPqgZ/t74J30I8f4es7uqjISNw5OAgyu21kzWZepSV1vLPfbcwMmLn3uGVmUvvHGHXJrMPFRksL48FlAsimWtwBB6oWiqw1oXpSxYPkrzK09Y4oZlEZUwwGWKiFQ1WlAF0gdUYzvQKdgFlh3H2CKOgMQCT8AoyBZG1pjmNwfydrCfuQci9prYpDC0LqsPOYaXMFwKevFW6ZWs20/RovtaBuIHmxHEembXx0kJvi37Qwe8QZE8lBPH/IAd/wpfJmzfLCkkcmeKlWQpvYtnmM72lKD3Nti2MzO6st0WW77JTTVju2cOiKmvyxSkJL7hxsYiZ/TifT5lURLTi/2/AN4lSSGlUOPiu/wadicbT9Zosl/Yp8gEM/dMGSCOui3YXy3yMq6Ei94guu8E4MqeSLAZk8jtHaIn9/h9dF65w== kruglik@A-KRUGLIK
"; // new line at the key end is crucial!

            var storage = new Storage();
            var client = new LibgitClient(
                new GitPluginProfile
                {
                    Uri = url,
                    UseSsh = true,
                    SshPrivateKey = privateKey,
                    SshPublicKey = publicKey
                }, storage);

            Assert.DoesNotThrow(() => client.Fetch());
        }

        [Test]
        [TestCase("https://gitlab.com/tpgitplugintest/tpgitplugintest.git")]
        [TestCase("https://github.com/tpgitplugintest/tpgitplugintest.git")]
        // Both github and gitlab test accounts has login tpgitplugintest@gmail.com and password q4mVjtjdUMhGV2w4StWst3ujzW87Fw
        public void ShouldCloneViaHttp(string url)
        {
            var storage = new Storage();
            var client = new LibgitClient(
                new GitPluginProfile
                {
                    Uri = url,
                    UseSsh = false,
                    Login = "tpgitplugintest@gmail.com",
                    Password = "q4mVjtjdUMhGV2w4StWst3ujzW87Fw"
                }, storage);

            Assert.DoesNotThrow(() => client.Fetch());
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

            var ngitRevs = ngit
                .GetRevisions(new RevisionId { Time = new DateTime(2017, 01, 01) }, new RevisionId { Time = new DateTime(2017, 10, 01) })
                .OrderBy(r => r.Id.Value).ToArray();
            var libgitRevs = libgit
                .GetRevisions(new RevisionId { Time = new DateTime(2017, 01, 01) }, new RevisionId { Time = new DateTime(2017, 10, 01) })
                .OrderBy(r => r.Id.Value).ToArray();

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
