// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using StructureMap;
using Tp.Core;
using Tp.Git.StructureMap;
using Tp.Git.VersionControlSystem;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Messages.TargetProcessLifecycle;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Mapping;
using Tp.Integration.Plugin.Common.Validation;
using Tp.Integration.Testing.Common;
using Tp.SourceControl.Commands;
using Tp.SourceControl.Comments;
using Tp.SourceControl.Diff;
using Tp.SourceControl.RevisionStorage;
using Tp.SourceControl.Settings;
using Tp.SourceControl.Testing.Repository.Git;
using Tp.SourceControl.VersionControlSystem;
using Tp.SourceControl.Workflow.Workflow;
using Tp.Testing.Common.NUnit;

namespace Tp.Git.Tests.VersionControlSystem
{
    [TestFixture]
    [Category("PartPlugins1")]
	public class GitSpecs : ISourceControlConnectionSettingsSource
	{
		private GitTestRepository _testRepository;
		private string _gitRepoUri;
		private IProfile _profile;

		#region SetUp/TearDown

		[SetUp]
		public void Init()
		{
			ObjectFactory.Initialize(x => x.AddRegistry<GitRegistry>());
			ObjectFactory.Configure(
				x =>
				x.For<TransportMock>().Use(TransportMock.CreateWithoutStructureMapClear(typeof (GitPluginProfile).Assembly,
				                                                                        new List<Assembly>
				                                                                        	{typeof (Command).Assembly})));

			_testRepository = new GitTestRepository();
			_gitRepoUri = _testRepository.Uri.ToString();

			_profile = ObjectFactory.GetInstance<TransportMock>().AddProfile("Profile", new GitPluginProfile
			                                                                            	{
			                                                                            		Uri = _gitRepoUri,
			                                                                            		Login = _testRepository.Login,
			                                                                            		Password = _testRepository.Password,
			                                                                            		StartRevision = "1/1/1980"
			                                                                            	});
			SetStartRevision("1/1/1980");
		}

		private void SetStartRevision(string startRevision)
		{
			((ISourceControlConnectionSettingsSource)this).StartRevision = startRevision;
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
			get; set;
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
				var revisionRange = git.GetFromTillHead(CreateGitRevisionId(GitRevisionId.UtcTimeMin), 100).Single();
				GitRevisionId fromChangeSet = revisionRange.FromChangeset;
				fromChangeSet.Time.Should(Be.EqualTo(DateTime.Parse("2011-11-02 1:57:19 PM")));

				GitRevisionId toChangeSet = revisionRange.ToChangeset;
				toChangeSet.Time.Should(Be.EqualTo(DateTime.Parse("2011-11-04 11:32:04 AM")));
			}
		}

		[Test]
		public void ShouldHandleConnectError()
		{
			_gitRepoUri = "//bla-bla";
			try
			{
				using (CreateGit())
				{
				}
				Assert.Fail("invalid uri did not cause any exceptions");
			}
			catch (Exception ex)
			{
				ex.Message.Should(Be.EqualTo("invalid uri or insufficient access rights"));
			}
		}

		[Test]
		public void ShouldRetrieveRevisionRangeFromRevisionTillHead()
		{
			using (var git = CreateGit())
			{
				const string startRevision = "2011-11-04 8:42:11 AM";
				
				SetStartRevision(startRevision);

				var startRevisionId = CreateGitRevisionId(DateTime.Parse(startRevision));
				var revisionRange = git.GetFromTillHead(startRevisionId, 100).Single();
				
				GitRevisionId fromChangeSet = revisionRange.FromChangeset;
				fromChangeSet.Time.Should(Be.EqualTo(startRevisionId.Time));

				GitRevisionId toChangeSet = revisionRange.ToChangeset;
				toChangeSet.Time.Should(Be.EqualTo(DateTime.Parse("2011-11-04 11:32:04 AM")));
			}
		}

		[Test]
		public void ShouldRetrieveRevisionRangeAfterSpecifiedTillHead()
		{
			using (var git = CreateGit())
			{
				const string startRevision = "2011-11-04 8:42:11";
	
				SetStartRevision(startRevision);

				var startRevisionId = CreateGitRevisionId(DateTime.Parse(startRevision));
				var revisionRange = git.GetAfterTillHead(startRevisionId, 100).Single();
				GitRevisionId fromChangeSet = revisionRange.FromChangeset;

				GitRevisionId fromExpected = CreateGitRevisionId(DateTime.Parse("2011-11-04 11:30:19"));
				fromChangeSet.Time.Should(Be.EqualTo(fromExpected.Time));

				GitRevisionId toChangeSet = revisionRange.ToChangeset;
				toChangeSet.Time.Should(Be.EqualTo(DateTime.Parse("2011-11-04 11:32:04")));
			}
		}

		private static GitRevisionId CreateGitRevisionId(DateTime dateTime)
		{
			return new GitRevisionId {Time = dateTime};
		}

		[Test]
		public void ShouldRetrieveRevisionsFromAndBefore()
		{
			using (var git = CreateGit())
			{
				var revisionRange =
					git.GetFromAndBefore(CreateGitRevisionId(DateTime.Parse("2011-11-02 1:57:19 PM")),
					                     CreateGitRevisionId(DateTime.Parse("2011-11-04 11:32:04 AM")), 100).Single();
				GitRevisionId fromChangeSet = revisionRange.FromChangeset;

				GitRevisionId fromExpected = CreateGitRevisionId(DateTime.Parse("2011-11-04 8:41:11 AM"));
				fromChangeSet.Time.Should(Be.EqualTo(fromExpected.Time));


				GitRevisionId toExpected = CreateGitRevisionId(DateTime.Parse("2011-11-04 11:31:19 AM"));
				GitRevisionId toChangeSet = revisionRange.ToChangeset;
				toChangeSet.Time.Should(Be.EqualTo(toExpected.Time));
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
			AssertCommits("first commit", "second commit to master", "Second Branch", "second commit to second branch",
			              "First Branch Commit", "second commit");
		}

		[Test]
		public void ShouldRetrieveRevisionsWithModifiedFiles()
		{
			using (var git = CreateGit())
			{
				const string startRevision = "2011-11-04 11:30:19 AM";

				SetStartRevision(startRevision);

				var startRevisionId = CreateGitRevisionId(DateTime.Parse(startRevision));
				var revisionRange = git.GetFromTillHead(startRevisionId, 100).Single();
				var revision = git.GetRevisions(revisionRange).OrderBy(x => x.Time).First();

				AssertEqual(revision.Entries, new[]
				                              	{
				                              		new RevisionEntryInfo
				                              			{Path = @"FirstFolder/firstFolderFile.txt", Action = FileActionEnum.Modify},
				                              		new RevisionEntryInfo
				                              			{Path = @"SecondFolder/secondFolderFile.txt", Action = FileActionEnum.Modify},
				                              		new RevisionEntryInfo {Path = "firstFile.txt", Action = FileActionEnum.Modify},
				                              		new RevisionEntryInfo {Path = "secondFile.txt", Action = FileActionEnum.Modify}
				                              	});
			}
		}


		[Test]
		public void ShouldRetrieveRevisionsWithAddedFiles()
		{
			using (var git = CreateGit())
			{
				GitRevisionId startRevisionId = CreateGitRevisionId(DateTime.Parse("2011-11-02 1:57:19 PM"));
				var revisionRange = git.GetFromTillHead(startRevisionId, 100).Single();

				var revision = git.GetRevisions(revisionRange).OrderBy(x => x.Time).First();

				AssertEqual(revision.Entries, new[]
				                              	{
				                              		new RevisionEntryInfo
				                              			{Path = @"FirstFolder/firstFolderFile.txt", Action = FileActionEnum.Add},
				                              		new RevisionEntryInfo
				                              			{Path = @"SecondFolder/secondFolderFile.txt", Action = FileActionEnum.Add},
				                              		new RevisionEntryInfo {Path = "firstFile.txt", Action = FileActionEnum.Add},
				                              		new RevisionEntryInfo {Path = "secondFile.txt", Action = FileActionEnum.Add}
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
				GitRevisionId startRevisionId = CreateGitRevisionId(GitRevisionId.UtcTimeMin);
				var revisionRange = git.GetFromTillHead(startRevisionId, 100).Single();

				var revision = git.GetRevisions(revisionRange).OrderByDescending(x => x.Time).First();

				AssertEqual(revision.Entries, new[] {new RevisionEntryInfo {Path = "firstFile.txt", Action = FileActionEnum.Delete}});
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
				GitRevisionId startRevisionId = CreateGitRevisionId(DateTime.Parse("2011-11-02 1:57:19 PM"));
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
				GitRevisionId correctRevisionId = CreateGitRevisionId(GitRevisionId.UtcTimeMin);
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
				GitRevisionId correctRevisionId = CreateGitRevisionId(GitRevisionId.UtcTimeMax.AddYears(1));
				var errors = new PluginProfileErrorCollection();
				git.CheckRevision(correctRevisionId, errors);
				errors.Single().ToString().Should(Be.EqualTo("Revision: should be between 1/1/1970 and 1/19/2038"));
			}
		}

		[Test]
		public void ShouldReCreateRepositoryWhenUriChanged()
		{
			using (CreateGit())
			{
			}

			var testRepo = new GitTestRepositoryWithMergeCommit();
			_gitRepoUri = testRepo.Uri.ToString();

			AssertCommits("first commit", "second commit to master", "Second Branch", "second commit to second branch",
			              "First Branch Commit", "second commit",
			              "thirdth commit to second branch");
		}

		[Test]
		public void ShouldNotImportDuplicateCommits()
		{
			var repo = new GitTestRepositoryWithCherryPickedCommit();
			var transportMock = ObjectFactory.GetInstance<TransportMock>();
			var gitPluginProfile = new GitPluginProfile
			                       	{
			                       		Uri = repo.Uri.ToString(),
			                       		Login = repo.Login,
			                       		Password = repo.Password,
			                       		StartRevision = "1/1/1980"
			                       	};
			var profile = transportMock.AddProfile("CherryPick", gitPluginProfile);

			using (var git = CreateGit(gitPluginProfile))
			{
				var startRevisionId = CreateGitRevisionId(GitRevisionId.UtcTimeMin);
				var revisionRange = git.GetFromTillHead(startRevisionId, 100).Single();

				transportMock.HandleLocalMessage(profile, new NewRevisionRangeDetectedLocalMessage {Range = revisionRange});

				transportMock.TpQueue.GetMessages<CreateCommand>().Count(x => x.Dto is RevisionDTO).Should(Be.EqualTo(1));
			}
		}

		[Test]
		public void ShouldSaveRevisionIdTpIdRelation()
		{
			var revisionId = _testRepository.Commit("third.txt", "Sample content", "#124");
			var transportMock = ObjectFactory.GetInstance<TransportMock>();
			const int tpId = 200;
			transportMock.On<CreateCommand>(x => x.Dto is RevisionDTO).Reply(x => new RevisionCreatedMessage
			{
				Dto = new RevisionDTO
				{
					SourceControlID = revisionId,
					ID = tpId
				}
			});

			using (var git = CreateGit())
			{
				var startRevisionId = CreateGitRevisionId(GitRevisionId.UtcTimeMin);
				var revisionRange = git.GetFromTillHead(startRevisionId, 100).Single();

				transportMock.HandleLocalMessage(_profile, new NewRevisionRangeDetectedLocalMessage { Range = revisionRange });

				ObjectFactory.GetInstance<IRevisionStorageRepository>().GetRevisionId(tpId).Should(Be.Not.Null);
			}
		}

		[Test]
		public void ShouldRemoveRevisionInfoIfRevisionCreationFailed()
		{
			var transportMock = ObjectFactory.GetInstance<TransportMock>();

			transportMock.On<CreateCommand>(x => x.Dto is RevisionDTO).Reply(x => new TargetProcessExceptionThrownMessage
			{
				ExceptionString = "Error"
			});

			using (var git = CreateGit())
			{
				var startRevisionId = CreateGitRevisionId(GitRevisionId.UtcTimeMin);
				var revisionRange = git.GetFromTillHead(startRevisionId, 100).Single();

				transportMock.HandleLocalMessage(_profile, new NewRevisionRangeDetectedLocalMessage { Range = revisionRange });

				ObjectFactory.GetInstance<IStorageRepository>().Get<bool>().FirstOrDefault().Should(Be.False);
			}
		}

		#region Helpers

		private GitVersionControlSystem CreateGit()
		{
			return CreateGit(this);
		}

		private GitVersionControlSystem CreateGit(ISourceControlConnectionSettingsSource settings)
		{
			return new GitVersionControlSystem(settings, ObjectFactory.GetInstance<ICheckConnectionErrorResolver>(),
											   ObjectFactory.GetInstance<IActivityLogger>(), ObjectFactory.GetInstance<IDiffProcessor>(), ObjectFactory.GetInstance<IStorageRepository>(), ObjectFactory.GetInstance<IRevisionIdComparer>());
		}

		#endregion
	}
}