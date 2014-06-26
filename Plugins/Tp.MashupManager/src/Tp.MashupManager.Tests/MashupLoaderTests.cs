// 
// Copyright (c) 2005-2014 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Tp.MashupManager.Tests
{
	[TestFixture]
    [Category("PartPlugins0")]
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
			_loader = new MashupLoader();
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
		public void ShouldReturnNullIfDirectoryNotExists()
		{
			Directory.Delete(_directory, true);

			Assert.Null(_loader.Load(_directory, "name"));
		}

		[Test]
		public void ShouldLoadMashupWithoutPlaceholdersAndAccounts()
		{
			var mashup = _loader.Load(_directory, MashupName);

			Assert.AreEqual(MashupName, mashup.Name);
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
			Assert.AreEqual(PlaceholderFileContentStripped, mashup.Placeholders);
			Assert.AreEqual(1, mashup.Files.Count);
			Assert.AreEqual(ScriptFileName, mashup.Files.First().FileName);
			Assert.AreEqual(ScriptFileContent, mashup.Files.First().Content);
		}

		[Test]
		public void ShouldLoadMashupWithCustomPlaceholdersFileAndAccounts()
		{
			const string customPlaceholderFileName = "foo.cfg";
			WriteMashupContent(customPlaceholderFileName, PlaceholderFileContent);
			WriteMashupContent(AccountsFileName, AccountsFileContent);

			var mashup = _loader.Load(_directory, MashupName);

			Assert.AreEqual(MashupName, mashup.Name);
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
			Assert.AreEqual(PlaceholderFileContentStripped, mashup.Placeholders);
			Assert.AreEqual(2, mashup.Files.Count);
			Assert.NotNull(scriptFile);
			Assert.AreEqual(ScriptFileContent, scriptFile.Content);
			Assert.NotNull(subFolderScriptFile);
			Assert.AreEqual(ScriptFileContent, subFolderScriptFile.Content);
		}
	}
}
