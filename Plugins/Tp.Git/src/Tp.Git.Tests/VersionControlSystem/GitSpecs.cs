// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NGit.Diff;
using NGit.Treewalk;
using NUnit.Framework;
using StructureMap;
using Tp.Core;
using Tp.Git.StructureMap;
using Tp.Git.VersionControlSystem;
using Tp.Integration.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Mapping;
using Tp.Integration.Plugin.Common.Validation;
using Tp.Integration.Testing.Common;
using Tp.SourceControl.Commands;
using Tp.SourceControl.Comments;
using Tp.SourceControl.Settings;
using Tp.SourceControl.Testing.Repository.Git;
using Tp.SourceControl.VersionControlSystem;
using Tp.Testing.Common.NUnit;

namespace Tp.Git.Tests.VersionControlSystem
{
	[TestFixture]
	public class GitSpecs : ISourceControlConnectionSettingsSource
	{
		private GitTestRepository _testRepository;
		private string _gitRepoUri;

		#region SetUp/TearDown

		[SetUp]
		public void Init()
		{
			ObjectFactory.Initialize(x => x.AddRegistry<GitRegistry>());
			ObjectFactory.Configure(
				x =>
				x.For<TransportMock>().Use(TransportMock.CreateWithoutStructureMapClear(typeof (GitPluginProfile).Assembly, new List<Assembly> {typeof (Command).Assembly})));

			_testRepository = new GitTestRepository();

			ObjectFactory.GetInstance<TransportMock>().AddProfile("Profile");

			_gitRepoUri = _testRepository.Uri.ToString();
		}

		string ISourceControlConnectionSettingsSource.Uri
		{
			get { return _gitRepoUri; }
		}

		string ISourceControlConnectionSettingsSource.Login
		{
			get { return string.Empty; }
		}

		string ISourceControlConnectionSettingsSource.Password
		{
			get { return string.Empty; }
		}

		string ISourceControlConnectionSettingsSource.StartRevision
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		MappingContainer ISourceControlConnectionSettingsSource.UserMapping
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		#endregion
		[Test]
		public void ShouldRetrieveRevisionRangeFromBeginningTillHead()
		{
			using (var git = CreateGit())
			{
				var revisionRange = git.GetFromTillHead(GitRevisionId.MinValue, 100).Single();
				GitRevisionId fromChangeSet = revisionRange.FromChangeset;
				fromChangeSet.Value.Should(Be.EqualTo(DateTime.Parse("2011-11-02 1:57:19 PM")));

				GitRevisionId toChangeSet = revisionRange.ToChangeset;
				toChangeSet.Value.Should(Be.EqualTo(DateTime.Parse("2011-11-04 11:32:04 AM")));
			}
		}

		[Test]
		public void ShouldRetrieveRevisionRangeFromRevisionTillHead()
		{
			using (var git = CreateGit())
			{
				GitRevisionId startRevisionId = DateTime.Parse("2011-11-04 8:42:11 AM");
				var revisionRange = git.GetFromTillHead(startRevisionId, 100).Single();
				GitRevisionId fromChangeSet = revisionRange.FromChangeset;
				fromChangeSet.Value.Should(Be.EqualTo(startRevisionId.Value));

				GitRevisionId toChangeSet = revisionRange.ToChangeset;
				toChangeSet.Value.Should(Be.EqualTo(DateTime.Parse("2011-11-04 11:32:04 AM")));
			}
		}

		[Test]
		public void ShouldRetrieveRevisionRangeAfterSpecifiedTillHead()
		{
			using (var git = CreateGit())
			{
				var startRevisionId = (GitRevisionId) DateTime.Parse("2011-11-04 8:42:11");
				var revisionRange = git.GetAfterTillHead(startRevisionId, 100).Single();
				GitRevisionId fromChangeSet = revisionRange.FromChangeset;

				GitRevisionId fromExpected = DateTime.Parse("2011-11-04 11:30:19");
				fromChangeSet.Value.Should(Be.EqualTo(fromExpected.Value));

				GitRevisionId toChangeSet = revisionRange.ToChangeset;
				toChangeSet.Value.Should(Be.EqualTo(DateTime.Parse("2011-11-04 11:32:04")));
			}
		}

		[Test]
		public void ShouldRetrieveRevisionsFromAndBefore()
		{
			using (var git = CreateGit())
			{
				var revisionRange =
					git.GetFromAndBefore((GitRevisionId) DateTime.Parse("2011-11-02 1:57:19 PM"),
					                     (GitRevisionId) DateTime.Parse("2011-11-04 11:32:04 AM"), 100).Single();
				GitRevisionId fromChangeSet = revisionRange.FromChangeset;

				GitRevisionId fromExpected = DateTime.Parse("2011-11-04 8:41:11 AM");
				fromChangeSet.Value.Should(Be.EqualTo(fromExpected.Value));


				GitRevisionId toExpected = DateTime.Parse("2011-11-04 11:31:19 AM");
				GitRevisionId toChangeSet = revisionRange.ToChangeset;
				toChangeSet.Value.Should(Be.EqualTo(toExpected.Value));
			}
		}

		[Test]
		public void ShouldRetrieveAuthors()
		{
			using (var git = CreateGit())
			{
				var authors = git.RetrieveAuthors(new DateRange(GitRevisionId.UtcTimeMin, DateTime.UtcNow));

				authors.Should(Be.EquivalentTo(new[] {"Valentine Palazkov"}));
			}
		}

		[Test]
		public void ShouldGetRevisions()
		{
			AssertCommits("first commit", "second commit to master", "Second Branch", "second commit to second branch", "First Branch Commit", "second commit");
		}

		[Test]
		public void ShouldRetrieveRevisionsWithModifiedFiles()
		{
			using (var git = CreateGit())
			{
				GitRevisionId startRevisionId = DateTime.Parse("2011-11-04 11:30:19 AM");
				var revisionRange = git.GetFromTillHead(startRevisionId, 100).Single();

				var revision = git.GetRevisions(revisionRange).OrderBy(x => x.Time).First();

				AssertEqual(revision.Entries, new[]
				{
					new RevisionEntryInfo{Path = @"FirstFolder/firstFolderFile.txt", Action = FileActionEnum.Modify}, 
					new RevisionEntryInfo{Path = @"SecondFolder/secondFolderFile.txt", Action = FileActionEnum.Modify}, 
					new RevisionEntryInfo{Path = "firstFile.txt", Action = FileActionEnum.Modify}, 
					new RevisionEntryInfo{Path = "secondFile.txt", Action = FileActionEnum.Modify}, 
				});
			}
		}


		[Test]
		public void ShouldRetrieveRevisionsWithAddedFiles()
		{
			using (var git = CreateGit())
			{
				GitRevisionId startRevisionId = DateTime.Parse("2011-11-02 1:57:19 PM");
				var revisionRange = git.GetFromTillHead(startRevisionId, 100).Single();

				var revision = git.GetRevisions(revisionRange).OrderBy(x => x.Time).First();

				AssertEqual(revision.Entries, new[]
				{
					new RevisionEntryInfo{Path = @"FirstFolder/firstFolderFile.txt", Action = FileActionEnum.Add}, 
					new RevisionEntryInfo{Path = @"SecondFolder/secondFolderFile.txt", Action = FileActionEnum.Add}, 
					new RevisionEntryInfo{Path = "firstFile.txt", Action = FileActionEnum.Add}, 
					new RevisionEntryInfo{Path = "secondFile.txt", Action = FileActionEnum.Add}, 
				});
			}
		}

		[Test]
		public void ShouldRetrieveRevisionsWithRemovedFiles()
		{
			var testRepo = new GitTestRepositoryWithFileDeleted();
			_gitRepoUri = testRepo.Uri.ToString();
			using (var git = CreateGit())
			{
				GitRevisionId startRevisionId = GitRevisionId.MinValue;
				var revisionRange = git.GetFromTillHead(startRevisionId, 100).Single();

				var revision = git.GetRevisions(revisionRange).OrderByDescending(x => x.Time).First();

				AssertEqual(revision.Entries, new []{new RevisionEntryInfo{Path = "firstFile.txt", Action = FileActionEnum.Delete}});
			}
		}


		private static void AssertEqual(IList<RevisionEntryInfo> actual, IList<RevisionEntryInfo> expected)
		{
			actual.Select(x => x.Action).ToArray().Should(Be.EquivalentTo(expected.Select(x => x.Action).ToArray()));
			actual.Select(x => x.Path).ToArray().Should(Be.EquivalentTo(expected.Select(x => x.Path).ToArray()));

			for (int i = 0; i < actual.Count; i++)
			{
				actual[i].Action.Should(Be.EqualTo(expected[i].Action));
				actual[i].Path.Should(Be.EqualTo(expected[i].Path));
			}
		}


		private void AssertCommits(params string[] commits)
		{
			using (var git = CreateGit())
			{
				GitRevisionId startRevisionId = DateTime.Parse("2011-11-02 1:57:19 PM");
				var revisionRange = git.GetFromTillHead(startRevisionId, 100).Single();

				var revisions = git.GetRevisions(revisionRange);
				revisions.Select(x => x.Comment).ToArray().Should(Be.EquivalentTo(commits));
				revisions.Select(x => x.Author).Distinct().ToArray().Should(Be.EquivalentTo(new[] {"Valentine Palazkov"}));
			}
		}

		[Test]
		public void ShouldCheckCorrectRevision()
		{
			using (var git = CreateGit())
			{
				GitRevisionId correctRevisionId = GitRevisionId.MinValue;
				var errors = new PluginProfileErrorCollection();
				git.CheckRevision(correctRevisionId, errors);
				errors.Should(Be.Empty);
			}
		}

		[Test]
		public void ShouldCheckIncorrectRevision()
		{
			using (var git = CreateGit())
			{
				GitRevisionId correctRevisionId = GitRevisionId.MaxValue.Value.AddYears(1);
				var errors = new PluginProfileErrorCollection();
				git.CheckRevision(correctRevisionId, errors);
				errors.Single().ToString().Should(Be.EqualTo("Revision: should be between 1/1/1970 and 1/19/2038"));
			}
		}

		[Test]
		public void ShouldReCreateRepositoryWhenUriChanged()
		{
			using (CreateGit()) {}

			var testRepo = new GitTestRepositoryWithMergeCommit();
			_gitRepoUri = testRepo.Uri.ToString();

			AssertCommits("first commit", "second commit to master", "Second Branch", "second commit to second branch", "First Branch Commit", "second commit",
			              "thirdth commit to second branch");
		}

		#region Helpers

		private GitVersionControlSystem CreateGit()
		{
			return new GitVersionControlSystem(this, ObjectFactory.GetInstance<ICheckConnectionErrorResolver>(), ObjectFactory.GetInstance<IActivityLogger>());
		}

		#endregion
	}
}