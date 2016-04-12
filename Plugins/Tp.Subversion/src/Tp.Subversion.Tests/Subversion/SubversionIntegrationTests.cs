// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using StructureMap;
using Tp.Core;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Testing.Common;
using Tp.SourceControl.Commands;
using Tp.SourceControl.Diff;
using Tp.SourceControl.Settings;
using Tp.SourceControl.VersionControlSystem;
using Tp.Subversion.StructureMap;
using Tp.Testing.Common.NUnit;
using log4net.Config;

namespace Tp.Subversion.Subversion
{
	[TestFixture]
    [Category("PartPlugins1")]
	public class SubversionIntegrationTests
	{
		public const string LocalRepositoryPath = "TestRepository";
		public const string LocalEmptyRepositoryPath = "EmptyRepository";

		static SubversionIntegrationTests()
		{
			BasicConfigurator.Configure();
		}

		[SetUp]
		public void Init()
		{
			ObjectFactory.Initialize(x => x.AddRegistry<VcsEnvironmentRegistry>());
			ObjectFactory.Configure(x => x.For<TransportMock>().Use(TransportMock.CreateWithoutStructureMapClear(typeof (SubversionPluginProfile).Assembly)));
		}

		private static ConnectionSettings GetLocalRepositorySettings(string pathFolder)
		{
			return LocalRepositorySettings.Create(LocalRepositoryPath, pathFolder);
		}

		private static ConnectionSettings GetLocalRepositorySettings()
		{
			return LocalRepositorySettings.Create(LocalRepositoryPath);
		}

		private static ConnectionSettings GetEmptyRepositorySettings()
		{
			return LocalRepositorySettings.Create(LocalEmptyRepositoryPath);
		}

		[Test]
		[ExpectedException(typeof (VersionControlException))]
		public void CannotConnectToUnknownHost()
		{
			var settings = GetLocalRepositorySettings();
			settings.Uri = new Uri("http://unknowhost:8080/trunk").ToString();
			new Subversion(settings, ObjectFactory.GetInstance<ICheckConnectionErrorResolver>(), ObjectFactory.GetInstance<IActivityLogger>(), ObjectFactory.GetInstance<IDiffProcessor>());
		}

		[Test]
		[ExpectedException(typeof (VersionControlException))]
		public void CannotConnectToUnknownRepository()
		{
			var settings = GetLocalRepositorySettings();
			settings.Uri = new Uri("http://localhost:8080/unknown").ToString();
			new Subversion(settings, ObjectFactory.GetInstance<ICheckConnectionErrorResolver>(), ObjectFactory.GetInstance<IActivityLogger>(), ObjectFactory.GetInstance<IDiffProcessor>());
		}

		[Test, Explicit("temp test to reproduce a bug")]
		public void ShouldRetrieve()
		{
			var settings = GetLocalRepositorySettings();
			settings.Uri = new Uri("http://svn.apache.org/repos/asf/spamassassin/trunk").ToString();
			var vcs = new Subversion(settings, ObjectFactory.GetInstance<ICheckConnectionErrorResolver>(), ObjectFactory.GetInstance<IActivityLogger>(), ObjectFactory.GetInstance<IDiffProcessor>());
			var svnRevisionId = new SvnRevisionId(1000000);
			var endRevision = vcs.GetFromTillHead(svnRevisionId, 50);
			var result = vcs.GetRevisions(endRevision.Last());
			result.Should(Be.Not.Empty, "result.Should(Be.Not.Empty)");
		}

		[Test]
		public void GettingRevisionsFromDifferentSVNFodlers()
		{
			const string firstFolder = "first folder";
			const string secondFolder = "second folder";
			const string thridFolder = "second folder/subdir";
			TestSubversionRepository(firstFolder, 7, 9);
			TestSubversionRepository(secondFolder, 7, 8);
			TestSubversionRepository(thridFolder, 7, 8);
			TestSubversionRepository(string.Empty, 1, 2, 3, 4, 5, 6, 7, 8, 9);
		}

		[Test]
		public void ShouldRetrieveLastRevision()
		{
			using (var subversion = new Subversion(GetLocalRepositorySettings(), ObjectFactory.GetInstance<ICheckConnectionErrorResolver>(), ObjectFactory.GetInstance<IActivityLogger>(), ObjectFactory.GetInstance<IDiffProcessor>()))
			{
				var revisions = subversion.GetAfterTillHead(new SvnRevisionId(8), 50).ToArray();
				revisions.Single().FromChangeset.Value.Should(Be.EqualTo(9.ToString()), "revisions.Single().FromChangeset.Value.Should(Be.EqualTo(9.ToString()))");
				revisions.Single().ToChangeset.Value.Should(Be.EqualTo(9.ToString()), "revisions.Single().ToChangeset.Value.Should(Be.EqualTo(9.ToString()))");
			}
		}

		[Test]
		public void ShouldRetrieveAuthors()
		{
			using (var subversion = new Subversion(GetLocalRepositorySettings(), ObjectFactory.GetInstance<ICheckConnectionErrorResolver>(), ObjectFactory.GetInstance<IActivityLogger>(), ObjectFactory.GetInstance<IDiffProcessor>()))
			{
				var revisions = subversion.GetRevisions(subversion.GetFromTillHead(new SvnRevisionId(0), 100).Single()).OrderBy(x => x.Time);

				var authors = subversion.RetrieveAuthors(new DateRange(revisions.First().Time, revisions.Last().Time));
				authors.Should(Be.EquivalentTo(revisions.Select(x => x.Author).Distinct().ToArray()), "authors.Should(Be.EquivalentTo(revisions.Select(x => x.Author).Distinct().ToArray()))");
			}
		}

		[Test]
		public void GetRevisionRanges()
		{
			using (var subversion = new Subversion(GetLocalRepositorySettings(), ObjectFactory.GetInstance<ICheckConnectionErrorResolver>(), ObjectFactory.GetInstance<IActivityLogger>(), ObjectFactory.GetInstance<IDiffProcessor>()))
			{
				var revisionRanges = subversion.GetFromTillHead(new SvnRevisionId(2), 3);
				revisionRanges.Count().Should(Be.EqualTo(3), "revisionRanges.Count().Should(Be.EqualTo(3))");
				revisionRanges[0].FromChangeset.Value.Should(Be.EqualTo(2.ToString()), "revisionRanges[0].FromChangeset.Value.Should(Be.EqualTo(2.ToString()))");
				revisionRanges[0].ToChangeset.Value.Should(Be.EqualTo(4.ToString()), "revisionRanges[0].ToChangeset.Value.Should(Be.EqualTo(4.ToString()))");

				revisionRanges[1].FromChangeset.Value.Should(Be.EqualTo(5.ToString()), "revisionRanges[1].FromChangeset.Value.Should(Be.EqualTo(5.ToString()))");
				revisionRanges[1].ToChangeset.Value.Should(Be.EqualTo(7.ToString()), "revisionRanges[1].ToChangeset.Value.Should(Be.EqualTo(7.ToString()))");

				revisionRanges[2].FromChangeset.Value.Should(Be.EqualTo(8.ToString()), "revisionRanges[2].FromChangeset.Value.Should(Be.EqualTo(8.ToString()))");
				revisionRanges[2].ToChangeset.Value.Should(Be.EqualTo(9.ToString()), "revisionRanges[2].ToChangeset.Value.Should(Be.EqualTo(9.ToString()))");
			}
		}

		private static void TestSubversionRepository(string firstFolder, params long[] revisionIds)
		{
			using (var sourceControlService = new Subversion(GetLocalRepositorySettings(firstFolder), ObjectFactory.GetInstance<ICheckConnectionErrorResolver>(), ObjectFactory.GetInstance<IActivityLogger>(), ObjectFactory.GetInstance<IDiffProcessor>()))
			{
				var range = sourceControlService.GetFromTillHead(0.ToString(), 100).Single();
				var revisionsFromRepo = sourceControlService.GetRevisions(range).Select(x => long.Parse(x.Id.Value)).ToArray();
				revisionsFromRepo.Should(Be.EquivalentTo(revisionIds), "revisionsFromRepo.Should(Be.EquivalentTo(revisionIds))");
			}
		}

		[Test]
		public void NoNeedToAuthenticateOnThirdPartyRepo()
		{
			var settings = GetLocalRepositorySettings();
			settings.Login = string.Empty;
			settings.Password = string.Empty;
			new Subversion(settings, ObjectFactory.GetInstance<ICheckConnectionErrorResolver>(), ObjectFactory.GetInstance<IActivityLogger>(), ObjectFactory.GetInstance<IDiffProcessor>());
		}

		[Test, Ignore("Turned off as we dont need to test SharpSvn functionality")]
		public void AuthenticationOnThirdPartyRepoSucceededWithTemporaryCertificate()
		{
			var settings = GetLocalRepositorySettings();
			settings.Uri = new Uri("https://srv2.office.targetprocess.com:8443/svn/func_test").ToString();
			settings.Login = "office\\testuser";
			settings.Password = "testuser";
			new Subversion(settings, ObjectFactory.GetInstance<ICheckConnectionErrorResolver>(), ObjectFactory.GetInstance<IActivityLogger>(), ObjectFactory.GetInstance<IDiffProcessor>());
		}

		[Test]
		public void ConnectToLocalRepository()
		{
			using (var s = new Subversion(GetLocalRepositorySettings(), ObjectFactory.GetInstance<ICheckConnectionErrorResolver>(), ObjectFactory.GetInstance<IActivityLogger>(), ObjectFactory.GetInstance<IDiffProcessor>()))
			{
				var range = s.GetFromTillHead(0.ToString(), 100).Single();
				s.GetRevisions(range).Should(Be.Not.Empty, "s.GetRevisions(range).Should(Be.Not.Empty)");
			}
		}

		[Test]
		public void GetLastRevisionInfo()
		{
			using (var s = new Subversion(GetLocalRepositorySettings(), ObjectFactory.GetInstance<ICheckConnectionErrorResolver>(), ObjectFactory.GetInstance<IActivityLogger>(), ObjectFactory.GetInstance<IDiffProcessor>()))
			{
				var range = s.GetFromTillHead(0.ToString(), 100).Single();
				s.GetRevisions(range).Should(Be.Not.Empty, "s.GetRevisions(range).Should(Be.Not.Empty)");
			}
		}

		[Test]
		public void GetLastRevisionInfoOnEmptyLocalRepository()
		{
			using (var s = new Subversion(GetEmptyRepositorySettings(), ObjectFactory.GetInstance<ICheckConnectionErrorResolver>(), ObjectFactory.GetInstance<IActivityLogger>(), ObjectFactory.GetInstance<IDiffProcessor>()))
			{
				var range = s.GetFromTillHead(0.ToString(), 100).Single();
				s.GetRevisions(range).Should(Be.Empty, "s.GetRevisions(range).Should(Be.Empty)");
			}
		}

		[Test]
		public void GetRevisionInfos()
		{
			using (var s = new Subversion(GetLocalRepositorySettings(), ObjectFactory.GetInstance<ICheckConnectionErrorResolver>(), ObjectFactory.GetInstance<IActivityLogger>(), ObjectFactory.GetInstance<IDiffProcessor>()))
			{
				IEnumerable<RevisionInfo> revisionInfos = s.GetRevisions(new RevisionRange(new SvnRevisionId(8), new SvnRevisionId(8)));

				Assert.IsNotNull(revisionInfos);

				foreach (RevisionInfo info in revisionInfos)
				{
					Assert.AreEqual("#112 state:invalid  time :0.25 comment: Linich, Maksim", info.Comment);
					// Haha, mistaken!!!
				}
			}
		}

		[Test]
		public void GetRevisionInfosForPath()
		{
			using (var s = new Subversion(GetLocalRepositorySettings(), ObjectFactory.GetInstance<ICheckConnectionErrorResolver>(), ObjectFactory.GetInstance<IActivityLogger>(), ObjectFactory.GetInstance<IDiffProcessor>()))
			{
				IEnumerable<RevisionInfo> revisionInfos = s.GetRevisions(new RevisionRange(new SvnRevisionId(1), new SvnRevisionId(5)), "readme.txt");

				Assert.IsNotNull(revisionInfos);
			}
		}

		[Test]
		public void GetTextFileContent()
		{
			using (var s = new Subversion(GetLocalRepositorySettings(), ObjectFactory.GetInstance<ICheckConnectionErrorResolver>(), ObjectFactory.GetInstance<IActivityLogger>(), ObjectFactory.GetInstance<IDiffProcessor>()))
			{
				string content = s.GetTextFileContent(3.ToString(), "/readme.txt");

				Assert.AreEqual("Test repository to verify subversion integration.", content);
				// there is extra newline char at the end of file
			}
		}

		[Test]
		[ExpectedException(typeof (VersionControlException))]
		public void GetTextFileContentUnknownPath()
		{
			using (var s = new Subversion(GetLocalRepositorySettings(), ObjectFactory.GetInstance<ICheckConnectionErrorResolver>(), ObjectFactory.GetInstance<IActivityLogger>(), ObjectFactory.GetInstance<IDiffProcessor>()))
			{
				s.GetTextFileContent(6066.ToString(), "/thisfiledoesnotexist.txt");
			}
		}

		[Test]
		[ExpectedException(typeof (VersionControlException))]
		public void GetTextFileContentUnknownRevision()
		{
			using (var s = new Subversion(GetLocalRepositorySettings(), ObjectFactory.GetInstance<ICheckConnectionErrorResolver>(), ObjectFactory.GetInstance<IActivityLogger>(), ObjectFactory.GetInstance<IDiffProcessor>()))
			{
				s.GetTextFileContent(99999.ToString(), "/TempSubversionTest/readme.txt");
			}
		}

		[Test]
		public void GetBinaryFileContent()
		{
			using (var s = new Subversion(GetLocalRepositorySettings(), ObjectFactory.GetInstance<ICheckConnectionErrorResolver>(), ObjectFactory.GetInstance<IActivityLogger>(), ObjectFactory.GetInstance<IDiffProcessor>()))
			{
				byte[] content = s.GetBinaryFileContent(3.ToString(), "readme.txt");

				Assert.AreEqual(Encoding.UTF8.GetBytes("Test repository to verify subversion integration."), content);
			}
		}
	}
}