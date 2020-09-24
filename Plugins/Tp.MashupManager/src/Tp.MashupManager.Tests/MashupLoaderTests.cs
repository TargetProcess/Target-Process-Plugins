using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Tp.Core;

namespace Tp.MashupManager.Tests
{
    [TestFixture]
    [Category("PartPlugins1")]
    public class MashupLoaderTests
    {
        private const string MashupName = "mashup";
        private const string ScriptFileName = "foo.js";
        private const string ScriptSubFolderName = "tau";
        private const string ScriptFileContent = "function(){}";
        private const string PlaceholderFileName = "placeholders.cfg";
        private const string PlaceholderFileContent = "Placeholders:footer";
        private const string PlaceholderFileContentStripped = "footer";
        private const string AccountsFileName = "account.cfg";
        private const string AccountsFileContent = "accounts:foo";

        private string _directory;
        private MashupLoader _loader;

        private void WriteMashupContent(string fileName, string fileContent)
        {
            File.WriteAllText(Path.Combine(_directory, fileName), fileContent);
        }

        [SetUp]
        public void SetUp()
        {
            _directory = Path.Combine(Path.GetTempPath(), DateTime.UtcNow.Ticks.ToString(CultureInfo.InvariantCulture));
            Directory.CreateDirectory(_directory);
            _loader = new MashupLoader(Maybe.Just<IReadOnlyCollection<string>>(new []{".js"}));
            WriteMashupContent(ScriptFileName, ScriptFileContent);
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_directory))
            {
                Directory.Delete(_directory, true);
            }
        }

        [Test]
        public void ShouldReturnNullIfDirectoryDoesNotExist()
        {
            Directory.Delete(_directory, true);

            Assert.Null(_loader.Load(_directory, "name"));
        }

        [Test]
        public void ShouldLoadMashupWithoutPlaceholdersAndAccounts()
        {
            var mashup = _loader.Load(_directory, MashupName);

            Assert.AreEqual(MashupName, mashup.Name);
            AssertMashupIsEnabled(mashup);
            Assert.AreEqual("", mashup.Placeholders);
            Assert.AreEqual(1, mashup.Files.Count);
            Assert.AreEqual(ScriptFileName, mashup.Files.First().FileName);
            Assert.AreEqual(ScriptFileContent, mashup.Files.First().Content);
        }

        [Test]
        public void ShouldLoadMashupWithPlaceholdersAndAccounts()
        {
            WriteMashupContent(PlaceholderFileName, PlaceholderFileContent);
            WriteMashupContent(AccountsFileName, AccountsFileContent);

            var mashup = _loader.Load(_directory, MashupName);

            Assert.AreEqual(MashupName, mashup.Name);
            AssertMashupIsEnabled(mashup);
            Assert.AreEqual(PlaceholderFileContentStripped, mashup.Placeholders);
            Assert.AreEqual(1, mashup.Files.Count);
            Assert.AreEqual(ScriptFileName, mashup.Files.First().FileName);
            Assert.AreEqual(ScriptFileContent, mashup.Files.First().Content);
        }

        [Test]
        public void ShouldLoadEnabledMashupWithPlaceholdersAndAccounts()
        {
            WriteMashupContent(PlaceholderFileName, "Placeholders:footer\r\nIsEnabled:true");
            WriteMashupContent(AccountsFileName, AccountsFileContent);

            var mashup = _loader.Load(_directory, MashupName);

            Assert.AreEqual(MashupName, mashup.Name);
            AssertMashupIsEnabled(mashup);
            Assert.AreEqual(PlaceholderFileContentStripped, mashup.Placeholders);
        }

        [Test]
        public void ShouldLoadDisabledMashupWithPlaceholdersAndAccounts()
        {
            WriteMashupContent(PlaceholderFileName, "Placeholders:footer\r\nIsEnabled:false");
            WriteMashupContent(AccountsFileName, AccountsFileContent);

            var mashup = _loader.Load(_directory, MashupName);

            Assert.AreEqual(MashupName, mashup.Name);
            AssertMashupIsDisabled(mashup);
            Assert.AreEqual(PlaceholderFileContentStripped, mashup.Placeholders);
        }

        [Test]
        public void ShouldLoadMashupWithUnixLineEndingsInConfig()
        {
            WriteMashupContent(PlaceholderFileName, "Placeholders:footer\nIsEnabled:false");
            WriteMashupContent(AccountsFileName, AccountsFileContent);

            var mashup = _loader.Load(_directory, MashupName);

            Assert.AreEqual(MashupName, mashup.Name);
            AssertMashupIsDisabled(mashup);
            Assert.AreEqual(PlaceholderFileContentStripped, mashup.Placeholders);
        }

        [Test]
        public void ShouldLoadEnabledMashupWithMetaInfo()
        {
            var config = new List<string>
            {
                "Placeholders:footer",
                "IsEnabled:true",
                "PackageName:Card Focus",
                "CreationDate:1485336191964",
                "CreatedBy:#1 Admin Admin"
            };
            WriteMashupContent(PlaceholderFileName, config.ToString("\r\n"));
            WriteMashupContent(AccountsFileName, AccountsFileContent);

            var mashup = _loader.Load(_directory, MashupName);

            Assert.AreEqual(MashupName, mashup.Name);
            Assert.AreEqual(PlaceholderFileContentStripped, mashup.Placeholders);
            AssertMashupIsEnabled(mashup);

            Assert.AreEqual("Card Focus", mashup.MashupMetaInfo.PackageName);

            Assert.AreEqual(1485336191964, mashup.MashupMetaInfo.CreationDate);
            Assert.IsNotNull(mashup.MashupMetaInfo.CreatedBy);
            Assert.AreEqual(1, mashup.MashupMetaInfo.CreatedBy.Id);
            Assert.AreEqual("Admin Admin", mashup.MashupMetaInfo.CreatedBy.Name);

            Assert.AreEqual(0, mashup.MashupMetaInfo.LastModificationDate);
            Assert.IsNull(mashup.MashupMetaInfo.LastModifiedBy);
        }

        [Test]
        public void ShouldLoadMashupWithCustomPlaceholdersFileAndAccounts()
        {
            const string customPlaceholderFileName = "foo.cfg";
            WriteMashupContent(customPlaceholderFileName, PlaceholderFileContent);
            WriteMashupContent(AccountsFileName, AccountsFileContent);

            var mashup = _loader.Load(_directory, MashupName);

            Assert.AreEqual(MashupName, mashup.Name);
            AssertMashupIsEnabled(mashup);
            Assert.AreEqual(PlaceholderFileContentStripped, mashup.Placeholders);
            Assert.AreEqual(1, mashup.Files.Count);
            Assert.AreEqual(ScriptFileName, mashup.Files.First().FileName);
            Assert.AreEqual(ScriptFileContent, mashup.Files.First().Content);
        }

        [Test]
        public void ShouldLoadMashupWithSeveralConfigFilesAndAccounts()
        {
            WriteMashupContent("config1.cfg", "Placeholders:config1");
            WriteMashupContent("config2.cfg", "Placeholders:config2");
            WriteMashupContent(AccountsFileName, AccountsFileContent);

            var mashup = _loader.Load(_directory, MashupName);
            var scriptFile = mashup.Files.FirstOrDefault(f => f.FileName == ScriptFileName);

            Assert.AreEqual(MashupName, mashup.Name);
            AssertMashupIsEnabled(mashup);
            Assert.AreEqual("config1,config2", mashup.Placeholders);
            Assert.AreEqual(1, mashup.Files.Count);
            Assert.NotNull(scriptFile);
            Assert.AreEqual(ScriptFileContent, scriptFile.Content);
        }

        [Test]
        public void ShouldLoadMashupWithSubFolder()
        {
            var scriptFileRelativePath = Path.Combine(ScriptSubFolderName, ScriptFileName);
            Directory.CreateDirectory(Path.Combine(_directory, ScriptSubFolderName));
            WriteMashupContent(scriptFileRelativePath, ScriptFileContent);
            WriteMashupContent(PlaceholderFileName, PlaceholderFileContent);

            var mashup = _loader.Load(_directory, MashupName);
            var scriptFile = mashup.Files.FirstOrDefault(f => f.FileName == ScriptFileName);
            var subFolderScriptFile = mashup.Files.FirstOrDefault(f => f.FileName == scriptFileRelativePath);

            Assert.AreEqual(MashupName, mashup.Name);
            AssertMashupIsEnabled(mashup);
            Assert.AreEqual(PlaceholderFileContentStripped, mashup.Placeholders);
            Assert.AreEqual(2, mashup.Files.Count);
            Assert.NotNull(scriptFile);
            Assert.AreEqual(ScriptFileContent, scriptFile.Content);
            Assert.NotNull(subFolderScriptFile);
            Assert.AreEqual(ScriptFileContent, subFolderScriptFile.Content);
        }

        [Test]
        public void ShouldNotLoadFilesNotInWhitelist()
        {
            WriteMashupContent("ignore.me", "please");

            var mashup = _loader.Load(_directory, MashupName);

            Assert.AreEqual(1, mashup.Files.Count);
            Assert.AreEqual(ScriptFileName, mashup.Files[0].FileName);
        }

        // ReSharper disable once UnusedParameter.Local
        private static void AssertMashupIsEnabled(Mashup mashup)
        {
            Assert.IsTrue(mashup.MashupMetaInfo.IsEnabled, "Mashup should be enabled");
        }

        // ReSharper disable once UnusedParameter.Local
        private static void AssertMashupIsDisabled(Mashup mashup)
        {
            Assert.IsFalse(mashup.MashupMetaInfo.IsEnabled, "Mashup should be disabled");
        }
    }
}
