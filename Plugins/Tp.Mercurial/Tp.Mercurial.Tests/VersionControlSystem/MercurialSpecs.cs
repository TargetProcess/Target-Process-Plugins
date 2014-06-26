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
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Messages.TargetProcessLifecycle;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Mapping;
using Tp.Integration.Plugin.Common.Validation;
using Tp.Integration.Testing.Common;
using Tp.Mercurial.StructureMap;
using Tp.Mercurial.VersionControlSystem;
using Tp.SourceControl.Commands;
using Tp.SourceControl.Comments;
using Tp.SourceControl.Diff;
using Tp.SourceControl.RevisionStorage;
using Tp.SourceControl.Settings;
using Tp.SourceControl.Testing.Repository.Mercurial;
using Tp.SourceControl.VersionControlSystem;
using Tp.SourceControl.Workflow.Workflow;
using Tp.Testing.Common.NUnit;
namespace Tp.Mercurial.Tests.VersionControlSystem
{
	[TestFixture]
    [Category("PartPlugins1")]
	public class MercurialSpecs : ISourceControlConnectionSettingsSource
	{
		private MercurialTestRepository _testRepository;
		private string _mercurialRepoUri;
		private IProfile _profile;

		#region SetUp/TearDown

		[SetUp]
		public void Init()
		{
			ObjectFactory.Initialize(x => x.AddRegistry<MercurialRegistry>());
			ObjectFactory.Configure(
				x =>
                x.For<TransportMock>().Use(TransportMock.CreateWithoutStructureMapClear(
                    typeof(MercurialPluginProfile).Assembly,
				    new List<Assembly> {typeof (Command).Assembly})));

            _testRepository = new MercurialTestRepository();
            _mercurialRepoUri = _testRepository.Uri.ToString();

			_profile = ObjectFactory.GetInstance<TransportMock>().AddProfile(
                "Profile", 
                new MercurialPluginProfile
			    {
                    Uri = _mercurialRepoUri,
			        Login = _testRepository.Login,
			        Password = _testRepository.Password,
			        StartRevision = "1/1/1980"
			    });
		}

		string ISourceControlConnectionSettingsSource.Uri
		{
            get { return _mercurialRepoUri; }
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
            using (var mercurial = CreateMercurial())
			{
                var revisionRange = mercurial.GetFromTillHead(CreateMercurialRevisionId(MercurialRevisionId.UtcTimeMin), 100).Single();
                MercurialRevisionId fromChangeSet = revisionRange.FromChangeset;
                fromChangeSet.Time.Should(Be.EqualTo(DateTime.Parse("2012-04-09 11:43:18 AM")));

                MercurialRevisionId toChangeSet = revisionRange.ToChangeset;
                toChangeSet.Time.Should(Be.EqualTo(DateTime.Parse("2012-04-09 15:08:01 PM")));
			}
		}

		[Test]
		public void ShouldHandleConnectError()
		{
            _mercurialRepoUri = "//bla-bla";
			try
			{
                using (CreateMercurial())
				{
				}
				Assert.Fail("invalid uri did not cause any exceptions");
			}
			catch (Exception ex)
			{
				ex.Message.Should(Be.EqualTo("abort: repository //bla-bla not found!\n"));
			}
		}

		[Test]
		public void ShouldRetrieveRevisionRangeFromRevisionTillHead()
		{
            using (var mercurial = CreateMercurial())
			{
                MercurialRevisionId startRevisionId = CreateMercurialRevisionId(DateTime.Parse(("2012-04-09 11:43:18 AM")));
                var revisionRange = mercurial.GetFromTillHead(startRevisionId, 100).Single();
                MercurialRevisionId fromChangeSet = revisionRange.FromChangeset;
				fromChangeSet.Time.Should(Be.EqualTo(startRevisionId.Time));

                MercurialRevisionId toChangeSet = revisionRange.ToChangeset;
                toChangeSet.Time.Should(Be.EqualTo(DateTime.Parse("2012-04-09 15:08:01 PM")));
			}
		}

		[Test]
		public void ShouldRetrieveRevisionRangeAfterSpecifiedTillHead()
		{
            using (var mercurial = CreateMercurial())
            {
	            var startRevisionId = new RevisionId() { Value = "94001070fc6a" };
                var revisionRange = mercurial.GetAfterTillHead(startRevisionId, 100).Single();
                MercurialRevisionId fromChangeSet = revisionRange.FromChangeset;

				MercurialRevisionId fromExpected = CreateMercurialRevisionId(DateTime.Parse("2012-04-09 11:44:23 AM"));
				fromChangeSet.Time.Should(Be.EqualTo(fromExpected.Time));

                MercurialRevisionId toChangeSet = revisionRange.ToChangeset;
                toChangeSet.Time.Should(Be.EqualTo(DateTime.Parse("2012-04-09 15:08:01 PM")));
			}
		}

		private static MercurialRevisionId CreateMercurialRevisionId(DateTime dateTime)
		{
            return new MercurialRevisionId { Time = dateTime };
		}

		private static MercurialRevisionId CreateMercurialRevisionId(string value)
		{
			return new MercurialRevisionId { Value = value};
		}

		[Test]
		public void ShouldRetrieveRevisionsFromAndBefore()
		{
            using (var mercurial = CreateMercurial())
			{
				var revisionRange = mercurial.GetFromAndBefore(CreateMercurialRevisionId("0f07bc12a328"), CreateMercurialRevisionId("d95d67882060"), 100).Single();
                MercurialRevisionId fromChangeSet = revisionRange.FromChangeset;

				MercurialRevisionId fromExpected = CreateMercurialRevisionId(DateTime.Parse("2012-04-09 11:44:23 AM"));
				fromChangeSet.Time.Should(Be.EqualTo(fromExpected.Time));


				MercurialRevisionId toExpected = CreateMercurialRevisionId(DateTime.Parse("2012-04-09 15:06:47 PM"));
                MercurialRevisionId toChangeSet = revisionRange.ToChangeset;
				toChangeSet.Time.Should(Be.EqualTo(toExpected.Time));
			}
		}

		[Test]
		public void ShouldRetrieveAuthors()
		{
            using (var mercurial = CreateMercurial())
			{
                var authors = mercurial.RetrieveAuthors(new DateRange(MercurialRevisionId.UtcTimeMin, DateTime.Now));

				authors.Should(Be.EquivalentTo(new[] {"msuhinin"}));
			}
		}

		[Test]
		public void ShouldGetRevisions()
		{
			AssertCommits("* files created", "* second commit", "commit to first branch",
                "commit to second branch", "second commit to first branch", "second commit to second branch");
		}

		[Test]
		public void ShouldRetrieveRevisionsWithModifiedFiles()
		{
            using (var mercurial = CreateMercurial())
			{
                MercurialRevisionId startRevisionId = CreateMercurialRevisionId(DateTime.Parse("2012-04-09 15:03:02 PM"));
                var revisionRange = mercurial.GetFromTillHead(startRevisionId, 100).Single();

                var revision = mercurial.GetRevisions(revisionRange).OrderBy(x => x.Time).First();

				AssertEqual(revision.Entries, new[]
				                              	{
                                                    new RevisionEntryInfo {Path = "firstFile.txt", Action = FileActionEnum.Modify},
				                              		new RevisionEntryInfo
				                              			{Path = @"firstFolder/firstFolderFile.txt", Action = FileActionEnum.Modify},
				                              		new RevisionEntryInfo {Path = "secondFile.txt", Action = FileActionEnum.Modify},
				                              		new RevisionEntryInfo
				                              			{Path = @"secondFolder/secondFolderFile.txt", Action = FileActionEnum.Modify}
				                              	});
			}
		}


		[Test]
		public void ShouldRetrieveRevisionsWithAddedFiles()
		{
            using (var mercurial = CreateMercurial())
			{
                MercurialRevisionId startRevisionId = CreateMercurialRevisionId(DateTime.Parse("2012-04-09 11:43:18 AM"));
                var revisionRange = mercurial.GetFromTillHead(startRevisionId, 100).Single();

                var revision = mercurial.GetRevisions(revisionRange).OrderBy(x => x.Time).First();

				AssertEqual(revision.Entries, new[]
				                              	{
                                                    new RevisionEntryInfo {Path = ".hgignore", Action = FileActionEnum.Add}, // mercurial auxiliary file
				                              		new RevisionEntryInfo {Path = "firstFile.txt", Action = FileActionEnum.Add},
				                              		new RevisionEntryInfo
				                              			{Path = @"firstFolder/firstFolderFile.txt", Action = FileActionEnum.Add},
				                              		new RevisionEntryInfo {Path = "secondFile.txt", Action = FileActionEnum.Add},
				                              		new RevisionEntryInfo
				                              			{Path = @"secondFolder/secondFolderFile.txt", Action = FileActionEnum.Add}
				                              	});
			}
		}

		[Test]
		public void ShouldRetrieveRevisionsWithRemovedFiles()
		{
			var testRepo = new MercurialTestRepositoryWithFileDeleted();
            _mercurialRepoUri = testRepo.Uri.ToString();
            using (var mercurial = CreateMercurial())
			{
                MercurialRevisionId startRevisionId = CreateMercurialRevisionId(MercurialRevisionId.UtcTimeMin);
                var revisionRange = mercurial.GetFromTillHead(startRevisionId, 100).Single();

                var revision = mercurial.GetRevisions(revisionRange).OrderByDescending(x => x.Time).First();

				AssertEqual(revision.Entries, new[] {new RevisionEntryInfo {Path = "deletedFile.txt", Action = FileActionEnum.Delete}});
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
            using (var mercurial = CreateMercurial())
			{
                MercurialRevisionId startRevisionId = CreateMercurialRevisionId(DateTime.Parse("2011-11-02 1:57:19 PM"));
                var revisionRange = mercurial.GetFromTillHead(startRevisionId, 100).Single();

                var revisions = mercurial.GetRevisions(revisionRange);
				revisions.Select(x => x.Comment).ToArray().Should(Be.EquivalentTo(commits));
				revisions.Select(x => x.Author).Distinct().ToArray().Should(Be.EquivalentTo(new[] {"msuhinin"}));
			}
		}

		[Test]
		public void ShouldCheckCorrectRevision()
		{
            using (var mercurial = CreateMercurial())
			{
                MercurialRevisionId correctRevisionId = CreateMercurialRevisionId(MercurialRevisionId.UtcTimeMin);
				var errors = new PluginProfileErrorCollection();
                mercurial.CheckRevision(correctRevisionId, errors);
				errors.Should(Be.Empty);
			}
		}

		[Test]
		public void ShouldCheckIncorrectRevision()
		{
            using (var mercurial = CreateMercurial())
			{
                MercurialRevisionId correctRevisionId = CreateMercurialRevisionId(MercurialRevisionId.UtcTimeMax.AddYears(1));
				var errors = new PluginProfileErrorCollection();
                mercurial.CheckRevision(correctRevisionId, errors);

			    string localizedMessage = string.Format(
			        "Revision: should be between {0} and {1}",
			        MercurialRevisionId.UtcTimeMin.ToShortDateString(),
			        MercurialRevisionId.UtcTimeMax.ToShortDateString());

				errors.Single().ToString().Should(Be.EqualTo(localizedMessage));
			}
		}

		[Test]
		public void ShouldReCreateRepositoryWhenUriChanged()
		{
            using (CreateMercurial())
			{
			}

			var testRepo = new MercurialTestRepositoryWithMergeCommit();
            _mercurialRepoUri = testRepo.Uri.ToString();

            AssertCommits("* files created", "* second commit", "commit to first branch",
                "commit to second branch", "second commit to first branch", "second commit to second branch", "changed in clone");
		}

		[Test]
		public void ShouldNotImportDuplicateCommits()
		{
			var repo = new MercurialTestRepositoryWithCherryPickedCommit();
			var transportMock = ObjectFactory.GetInstance<TransportMock>();
            var mercurialPluginProfile = new MercurialPluginProfile
			                       	{
			                       		Uri = repo.Uri.ToString(),
			                       		Login = repo.Login,
			                       		Password = repo.Password,
			                       		StartRevision = "1/1/1980"
			                       	};
            var profile = transportMock.AddProfile("CherryPick", mercurialPluginProfile);

            using (var mercurial = CreateMercurial(mercurialPluginProfile))
			{
                var startRevisionId = CreateMercurialRevisionId(MercurialRevisionId.UtcTimeMin);
                var revisionRange = mercurial.GetFromTillHead(startRevisionId, 100).Single();

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

            using (var mercurial = CreateMercurial())
			{
                var startRevisionId = CreateMercurialRevisionId(MercurialRevisionId.UtcTimeMin);
                var revisionRange = mercurial.GetFromTillHead(startRevisionId, 100).Single();

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

            using (var mercurial = CreateMercurial())
			{
                var startRevisionId = CreateMercurialRevisionId(MercurialRevisionId.UtcTimeMin);
                var revisionRange = mercurial.GetFromTillHead(startRevisionId, 100).Single();

				transportMock.HandleLocalMessage(_profile, new NewRevisionRangeDetectedLocalMessage { Range = revisionRange });

				ObjectFactory.GetInstance<IStorageRepository>().Get<bool>().FirstOrDefault().Should(Be.False);
			}
		}

        #region Helpers

		private MercurialVersionControlSystem CreateMercurial()
		{
            return CreateMercurial(this);
		}

        private MercurialVersionControlSystem CreateMercurial(ISourceControlConnectionSettingsSource settings)
		{
            return new MercurialVersionControlSystem(
                settings, 
                ObjectFactory.GetInstance<ICheckConnectionErrorResolver>(),
                ObjectFactory.GetInstance<IActivityLogger>(), 
                ObjectFactory.GetInstance<IDiffProcessor>(),
                ObjectFactory.GetInstance<IStorageRepository>());
		}

		#endregion
	}
}