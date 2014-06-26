// 
// Copyright (c) 2005-2014 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using log4net;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap;
using Tp.Integration.Messages;
using Tp.Integration.Plugin.Common.Logging;
using Tp.Integration.Plugin.Common.Tests.Common;
using Tp.MashupManager.MashupStorage;
using Tp.Testing.Common.NUnit;

namespace Tp.MashupManager.Tests.MashupStorage
{
	[TestFixture]
	public class MashupScriptStorageTests : MashupManagerTestBase
	{
		private string _directory;
		private MashupScriptStorage _storage;
		private IMashupLocalFolder _mashupLocalFolder;
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
			logManager.Stub(lm => lm.GetLogger(Arg<Type>.Is.Same(typeof(MashupScriptStorage)))).Return(LogManager.GetLogger(typeof(MashupScriptStorage)));
			_mashupLocalFolder = ObjectFactory.GetInstance<IMashupLocalFolder>();
			_mashupFolderPath = Path.Combine(_mashupLocalFolder.Path, MashupName);
			_storage = new MashupScriptStorage(new PluginContextMock { AccountName = new AccountName() }, _mashupLocalFolder,
				logManager, new MashupLoader());
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
		public void ShouldSaveMashupWithSubFolder()
		{
			_storage.SaveMashup(
				new Mashup(new List<MashupFile>
				{
					new MashupFile {FileName = FileName, Content = string.Empty},
					new MashupFile {FileName = FileName2, Content = string.Empty}
				}) {Name = MashupName});

			File.Exists(Path.Combine(_mashupFolderPath, FileName)).Should(Be.True);
			File.Exists(Path.Combine(_mashupFolderPath, FileName2)).Should(Be.True);
		}
	}
}