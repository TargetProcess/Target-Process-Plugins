using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using log4net;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap;
using Tp.Core;
using Tp.Integration.Messages;
using Tp.Integration.Plugin.Common.Logging;
using Tp.Integration.Plugin.Common.Tests.Common;
using Tp.MashupManager.MashupStorage;
using Tp.Testing.Common.NUnit;

namespace Tp.MashupManager.Tests.MashupStorage
{
    [TestFixture]
    [Category("PartPlugins1")]
    public class MashupScriptStorageTests : MashupManagerTestBase
    {
        private string _directory;
        private MashupScriptStorage _storage;
        private string _mashupFolderPath;

        private const string MashupName = "TestMashup";
        private const string FileName = "script1.js";
        private const string FileName2 = "SubFolder1\\SubFolder2\\script2.js";

        [SetUp]
        public void TestSetUp()
        {
            _directory = Path.Combine(Path.GetTempPath(), DateTime.UtcNow.Ticks.ToString(CultureInfo.InvariantCulture));
            Directory.CreateDirectory(_directory);
            var logManager = MockRepository.GenerateStub<ILogManager>();
            logManager.Stub(lm => lm.GetLogger(Arg<Type>.Is.Same(typeof(MashupScriptStorage))))
                .Return(LogManager.GetLogger(typeof(MashupScriptStorage)));

            var mashupLocalFolder = ObjectFactory.GetInstance<IMashupLocalFolder>();
            _mashupFolderPath = Path.Combine(mashupLocalFolder.Path, MashupName);

            _storage = new MashupScriptStorage(new PluginContextMock { AccountName = new AccountName() }, mashupLocalFolder,
                logManager, new MashupLoader(Maybe.Nothing));
        }

        [TearDown]
        public void TestTearDown()
        {
            if (Directory.Exists(_directory))
            {
                Directory.Delete(_directory, true);
            }
        }

        [Test]
        public void ShouldSaveMashupWithFileInSubFolder()
        {
            _storage.SaveMashup(
                new Mashup(new List<MashupFile>
                {
                    EmptyMashupFile(FileName),
                    EmptyMashupFile(FileName2)
                }) { Name = MashupName });

            File.Exists(Path.Combine(_mashupFolderPath, FileName)).Should(Be.True, $"File {FileName} should exist");
            File.Exists(Path.Combine(_mashupFolderPath, FileName2)).Should(Be.True, $"File {FileName2} should exist");
        }

        [Test]
        public void ShouldSaveMashupConfiguration()
        {
            _storage.SaveMashup(
                new Mashup(new List<MashupFile>
                {
                    EmptyMashupFile(FileName)
                }) { Name = MashupName, Placeholders = "footer" });

            var configPath = Path.Combine(_mashupFolderPath, "placeholders.cfg");
            File.Exists(configPath).Should(Be.True, "Configuration file placeholders.cfg should exist");

            var expectedConfig = new[] { "Placeholders:footer", "IsEnabled:true" };
            File.ReadAllLines(configPath).Should(Be.EquivalentTo(expectedConfig),
                "Configuration file should have content as expected");
        }

        [Test, ExpectedException(typeof(BadMashupFileNameException))]
        public void ShouldRaiseBadMashupFileNameErrorForAbsolutePath()
        {
            _storage.SaveMashup(
                new Mashup(new List<MashupFile>
                {
                    EmptyMashupFile(@"C:\1.txt")
                }) { Name = MashupName });
        }

        [Test, ExpectedException(typeof(BadMashupFileNameException))]
        public void ShouldRaiseBadMashupFileNameErrorForRelativePath()
        {
            _storage.SaveMashup(
                new Mashup(new List<MashupFile>
                {
                    EmptyMashupFile(@"..\" + FileName)
                }) { Name = MashupName });
        }

        [Test, ExpectedException(typeof(BadMashupNameException))]
        public void ShouldRaiseBadMashupNameErrorForBadMashupNameAndGoodFileName()
        {
            _storage.SaveMashup(
                new Mashup(new List<MashupFile>
                {
                    EmptyMashupFile(FileName)
                }) { Name = @"\..\..\zagzag" });
        }

        [Test, ExpectedException(typeof(BadMashupNameException))]
        public void ShouldRaiseBadMashupNameErrorForBadMashupNameAndBadFileName()
        {
            _storage.SaveMashup(
                new Mashup(new List<MashupFile>
                {
                    EmptyMashupFile(@"C:\1.txt")
                }) { Name = @"\..\zagzag" });
        }

        [Test]
        public void ShouldNotThrowWhenDeletingNonExistingMashup()
        {
            Assert.DoesNotThrow(() => _storage.DeleteMashup("NonExistingMashup"));
        }

        private static MashupFile EmptyMashupFile(string fileName)
        {
            return new MashupFile { FileName = fileName, Content = string.Empty };
        }
    }
}
