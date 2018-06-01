using System;
using System.IO;
using System.Linq;
using System.Net;
using NGit.Api;
using NUnit.Framework;
using StructureMap;
using Tp.Core;
using Tp.Integration.Plugin.Common;
using Tp.MashupManager.MashupLibrary;
using Tp.MashupManager.MashupLibrary.Package;
using Tp.MashupManager.MashupLibrary.Repository;
using Tp.MashupManager.MashupLibrary.Repository.Config;
using Tp.MashupManager.MashupLibrary.Repository.Synchronizer;
using Tp.Testing.Common.NUnit;

namespace Tp.MashupManager.Tests.MashupLibrary.Repository
{
    [TestFixture]
    [Category("PartPlugins1")]
    public class LibraryRepositoryTests : MashupManagerTestBase
    {
        private string _testMashupFullName;
        private string _remoteRepository;

        public override void SetUp()
        {
            base.SetUp();

            ClearLibrary();
            ObjectFactory.Configure(x => x.For<ILibraryLocalFolder>().Use<LibraryLocalFolder>());
            _remoteRepository = CreateRemoteRepository();
            _testMashupFullName = CreateRemoteMashup(_remoteRepository, TestMashupName);
            CreateRemoteMashupFile(_testMashupFullName, TestMashupJsFileName);
            CreateRemoteMashupFile(_testMashupFullName, TestMashupMkdFileName, TestMashupMkdFileContent);
            CreateRemoteMashupFile(_testMashupFullName, TestMashupBaseInfoFileName, TestMashupBaseInfoFileContent);
            InitRemoteRepository(_remoteRepository);
            CommitRemoteRepository(_remoteRepository);
        }

        public override void TearDown()
        {
            ClearLibrary();

            base.TearDown();
        }

        private const string Tp3RepositoryName = "Tp3";
        private const string TestMashupName = "LibraryTestMashup";
        private const string TestMashupJsFileName = "LibraryTestMashup.js";
        private const string TestMashupCfgFileName = "LibraryTestMashup.cfg";
        private const string TestMashupMkdFileName = "readme.mkd";
        private const string TestMashupMkdFileContent = "Test mashup description";
        private const string TestMashupBaseInfoFileName = "LibraryTestMashup.baseinfo.json";

        private const string TestMashupBaseInfoFileContentTemplate =
            "{{\"ShortDescription\": \"{0}\", \"CompatibleTpVersion\": {{\"Minimum\": \"{1}\"}}}}";

        private const string TestMashupBaseInfoDescription = "test description";
        private const string TestMashupBaseInfoCompatibleTpVersionMinimum = "3.0.0";

        private static readonly string TestMashupBaseInfoFileContent =
            TestMashupBaseInfoFileContentTemplate.Fmt(TestMashupBaseInfoDescription, TestMashupBaseInfoCompatibleTpVersionMinimum);

        private const string TestMashupBaseInfoUpdatedDescription = "test description updated";
        private const string TestMashupBaseInfoUpdatedCompatibleTpVersionMinimum = "3.0.1";

        private static readonly string TestMashupBaseInfoUpdatedFileContent =
            "{{\"ShortDescription\": \"{0}\", \"CompatibleTpVersion\": {{\"Minimum\": \"{1}\"}}}}".Fmt(
                TestMashupBaseInfoUpdatedDescription, TestMashupBaseInfoUpdatedCompatibleTpVersionMinimum);

        private void ClearLibrary()
        {
            if (Directory.Exists(LibraryPath))
            {
                RemoveReadOnly(new DirectoryInfo(LibraryPath));
                Directory.Delete(LibraryPath, true);
            }
        }

        private void RemoveReadOnly(DirectoryInfo parentDirectory)
        {
            if (parentDirectory != null)
            {
                parentDirectory.Attributes = FileAttributes.Normal;
                foreach (FileInfo fi in parentDirectory.GetFiles())
                {
                    fi.Attributes = FileAttributes.Normal;
                }
                foreach (DirectoryInfo di in parentDirectory.GetDirectories())
                {
                    RemoveReadOnly(di);
                }
            }
        }

        private string LibraryPath
        {
            get { return new LibraryLocalFolder(new PluginDataFolder()).Path; }
        }

        private string RemoteRepositoryPath
        {
            get { return Path.Combine(LibraryPath, "RemoteRepository"); }
        }

        private string CreateRemoteRepository()
        {
            Directory.CreateDirectory(RemoteRepositoryPath);
            return RemoteRepositoryPath;
        }

        private string CreateRemoteMashup(string remoteRepository, string mashupName)
        {
            string mashupPath = Path.Combine(remoteRepository, mashupName);
            Directory.CreateDirectory(mashupPath);
            return mashupPath;
        }

        private void CreateRemoteMashupFile(string mashup, string fileName, string fileContent = "")
        {
            using (StreamWriter streamWriter = File.CreateText(Path.Combine(mashup, fileName)))
            {
                streamWriter.Write(fileContent);
            }
        }

        private void CommitRemoteRepository(string remoteRepository)
        {
            InvokeInSecurityProtocolScope(() =>
            {
                Git git = Git.Open(remoteRepository);
                try
                {
                    git.Add().AddFilepattern(".").Call();
                    git.Commit().SetMessage("Commit message").Call();
                }
                finally
                {
                    git.GetRepository().Close();
                }
            });
        }

        private void InitRemoteRepository(string remoteRepository)
        {
            InvokeInSecurityProtocolScope(() =>
            {
                Git git = Git.Init().SetDirectory(remoteRepository).Call();
                git.GetRepository().Close();
            });
        }

        private ILibraryRepository LibraryRepository
        {
            get
            {
                return new LibraryRepository(new LibraryRepositoryConfig
                {
                    Name = Tp3RepositoryName,
                    Uri = new Uri(RemoteRepositoryPath).AbsoluteUri
                }, new LibraryLocalFolder(new PluginDataFolder()), new LibraryRepositorySynchronizer(), new MashupLoader());
            }
        }

        private void CheckRepositoryFilesExistence(params string[] fileNames)
        {
            string tp3RepositoryPath = Path.Combine(LibraryPath, Tp3RepositoryName);
            string testMashupPath = Path.Combine(tp3RepositoryPath, TestMashupName);

            Directory.Exists(tp3RepositoryPath).Should(Is.True, "Directory.Exists(tp3RepositoryPath).Should(Is.True)");
            Directory.Exists(testMashupPath).Should(Is.True, "Directory.Exists(testMashupPath).Should(Is.True)");
            foreach (string fileName in fileNames)
            {
                File.Exists(Path.Combine(testMashupPath, fileName))
                    .Should(Is.True, "File.Exists(Path.Combine(testMashupPath, fileName)).Should(Is.True)");
            }
        }

        [Test]
        public void CloneRepository()
        {
            LibraryRepository.Refresh();
            CheckRepositoryFilesExistence(TestMashupJsFileName, TestMashupMkdFileName);
        }

        [Test]
        public void GetPackages()
        {
            LibraryRepository.Refresh();
            TestDefaultBaseInfo(GetTestMashupPackage().BaseInfo);
        }

        [Test]
        public void TestDescriptionUpdate()
        {
            LibraryRepository.Refresh();
            TestDefaultBaseInfo(GetTestMashupPackage().BaseInfo);
            CreateRemoteMashupFile(_testMashupFullName, TestMashupBaseInfoFileName, TestMashupBaseInfoUpdatedFileContent);
            InitRemoteRepository(_remoteRepository);
            CommitRemoteRepository(_remoteRepository);
            LibraryRepository.Refresh();
            GetTestMashupPackage()
                .BaseInfo.ShortDescription.Should(Be.EqualTo(TestMashupBaseInfoUpdatedDescription),
                    "GetTestMashupPackage().BaseInfo.ShortDescription.Should(Be.EqualTo(TestMashupBaseInfoUpdatedDescription))");
            GetTestMashupPackage()
                .BaseInfo.CompatibleTpVersion.Minimum.Should(Be.EqualTo(TestMashupBaseInfoUpdatedCompatibleTpVersionMinimum),
                    "GetTestMashupPackage().BaseInfo.CompatibleTpVersion.Minimum.Should(Be.EqualTo(TestMashupBaseInfoUpdatedCompatibleTpVersionMinimum))");
        }

        [Test]
        public void GetPackagesWhenRepositoryNotCloned()
        {
            TestDefaultBaseInfo(GetTestMashupPackage().BaseInfo);
        }

        [Test]
        public void PullRepository()
        {
            LibraryRepository.Refresh();
            CreateRemoteMashupFile(Path.Combine(RemoteRepositoryPath, TestMashupName), TestMashupCfgFileName);
            CommitRemoteRepository(RemoteRepositoryPath);
            LibraryRepository.Refresh();
            CheckRepositoryFilesExistence(TestMashupJsFileName, TestMashupMkdFileName, TestMashupCfgFileName);
        }

        [Test]
        public void GetMashup()
        {
            CreateRemoteMashupFile(Path.Combine(RemoteRepositoryPath, TestMashupName), TestMashupCfgFileName,
                "Placeholders:" + DefaultPlaceholders);
            CommitRemoteRepository(RemoteRepositoryPath);
            LibraryRepository.Refresh();
            string name = GetTestMashupPackage().Name;
            var mashup = LibraryRepository.GetPackageMashup(name);
            mashup.Name.Should(Be.EqualTo(name), "mashup.Name.Should(Be.EqualTo(name))");
            mashup.Placeholders.ToLower()
                .Should(Be.EqualTo(DefaultPlaceholders.ToLower()),
                    "mashup.Placeholders.ToLower().Should(Be.EqualTo(DefaultPlaceholders.ToLower()))");
        }

        [Test]
        public void GetPackageDetailed()
        {
            LibraryRepository.Refresh();
            LibraryRepository.GetPackageDetailed(TestMashupName)
                .ReadmeMarkdown.Should(Is.EqualTo(TestMashupMkdFileContent),
                    "LibraryRepository.GetPackageDetailed(TestMashupName).ReadmeMarkdown.Should(Is.EqualTo(TestMashupMkdFileContent))");
        }

        private LibraryPackage GetTestMashupPackage()
        {
            return LibraryRepository.GetPackages().First(p => p.Name == TestMashupName);
        }

        private void TestDefaultBaseInfo(LibraryPackageBaseInfo baseInfo)
        {
            baseInfo.ShortDescription.Should(Is.EqualTo(TestMashupBaseInfoDescription),
                "baseInfo.ShortDescription.Should(Is.EqualTo(TestMashupBaseInfoDescription))");
            baseInfo.CompatibleTpVersion.Minimum.Should(Is.EqualTo(TestMashupBaseInfoCompatibleTpVersionMinimum),
                "baseInfo.CompatibleTpVersion.Minimum.Should(Is.EqualTo(TestMashupBaseInfoCompatibleTpVersionMinimum))");
        }

        private void InvokeInSecurityProtocolScope(Action action)
        {
            using (new SecurityProtocolTypeScope(SecurityProtocolType.Tls12))
            {
                action();
            }
        }
    }
}
