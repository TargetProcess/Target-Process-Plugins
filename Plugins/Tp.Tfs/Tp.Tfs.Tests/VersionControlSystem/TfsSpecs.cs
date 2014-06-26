// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Globalization;
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
using Tp.SourceControl.Commands;
using Tp.SourceControl.Comments;
using Tp.SourceControl.Diff;
using Tp.SourceControl.RevisionStorage;
using Tp.SourceControl.Settings;
using Tp.SourceControl.VersionControlSystem;
using Tp.SourceControl.Workflow.Workflow;
using Tp.Testing.Common.NUnit;
using Tp.Tfs.StructureMap;
using Tp.Tfs.VersionControlSystem;
using Tp.SourceControl.Testing.Repository.Tfs;

namespace Tp.Tfs.Tests.VersionControlSystem
{
	[TestFixture]
    [Ignore]
	[Category("PartPlugins1")]
	public class TfsSpecs : ISourceControlConnectionSettingsSource
	{
		private TfsTestRepository _testRepository;
		private string _tfsRepoUri;
		private IProfile _profile;

		#region SetUp/TearDown

		[SetUp]
		public void Init()
		{
			ObjectFactory.Initialize(x => x.AddRegistry<TfsRegistry>());
			ObjectFactory.Configure(
				x =>
								x.For<TransportMock>().Use(TransportMock.CreateWithoutStructureMapClear(
										typeof(TfsPluginProfile).Assembly,
						new List<Assembly> { typeof(Command).Assembly })));

			_testRepository = new TfsTestRepository();
			_tfsRepoUri = _testRepository.Uri.ToString();

			_profile = ObjectFactory.GetInstance<TransportMock>().AddProfile(
								"Profile",
								new TfsPluginProfile
					{
						Uri = _tfsRepoUri,
						Login = _testRepository.Login,
						Password = _testRepository.Password,
						StartRevision = "1"
					});
		}

		[TearDown]
		public void Dispose()
		{
			_testRepository.Dispose();
		}

		string ISourceControlConnectionSettingsSource.Uri
		{
			get { return _tfsRepoUri; }
		}

		string ISourceControlConnectionSettingsSource.Login
		{
			get { return _testRepository.Login; }
		}

		string ISourceControlConnectionSettingsSource.Password
		{
			get { return _testRepository.Password; }
		}

		string ISourceControlConnectionSettingsSource.StartRevision
		{
			get { return "1"; }
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
			using (var tfs = CreateTfs())
			{
				int revisionValue = GetValidRevision(_testRepository);

				var revisionRange = tfs.GetFromTillHead(
						CreateTfsRevisionId(revisionValue.ToString()), ConfigHelper.Instance.PageSize).First();

				TfsRevisionId fromChangeSet = revisionRange.FromChangeset;
				fromChangeSet.Value.Should(Be.EqualTo(revisionValue.ToString()));

				string maxChengesetId = _testRepository.GetLatestRevision();

				TfsRevisionId toChangeSet = revisionRange.ToChangeset;
				toChangeSet.Value.Should(Be.EqualTo(maxChengesetId));
			}
		}

		[Test]
		public void ShouldHandleConnectError()
		{
			_tfsRepoUri = "//bla-bla";
			try
			{
				using (CreateTfs())
				{
				}
				Assert.Fail("invalid uri did not cause any exceptions");
			}
			catch (Exception ex)
			{
				//pass
			}
		}

		[Test]
		public void ShouldRetrieveRevisionRangeFromRevisionTillHead()
		{
			using (var tfs = CreateTfs())
			{
				int revisionValue = GetValidRevision(_testRepository);
				TfsRevisionId startRevisionId = CreateTfsRevisionId(revisionValue.ToString(CultureInfo.InvariantCulture));
				var revisionRange = tfs.GetFromTillHead(startRevisionId, ConfigHelper.Instance.PageSize).First();

				TfsRevisionId fromChangeSet = revisionRange.FromChangeset;
				fromChangeSet.Value.Should(Be.EqualTo(startRevisionId.Value));

				string maxChangesetId = _testRepository.GetLatestRevision();

				TfsRevisionId toChangeSet = revisionRange.ToChangeset;
				toChangeSet.Value.Should(Be.EqualTo(maxChangesetId));
			}
		}

		private int GetValidRevision(TfsTestRepository repository)
		{
			string maxChangeSetId = _testRepository.GetLatestRevision();

			int revisionValue;
			if (int.Parse(maxChangeSetId) >= ConfigHelper.Instance.PageSize)
				revisionValue = int.Parse(maxChangeSetId) - ConfigHelper.Instance.PageSize / 2;
			else
				revisionValue = int.Parse(maxChangeSetId) / 2;

			return revisionValue;
		}

		[Test]
		public void ShouldRetrieveRevisionRangeAfterSpecifiedTillHead()
		{
			using (var tfs = CreateTfs())
			{
				string maxChangeSetId = _testRepository.GetLatestRevision();

				int revisionValue = GetValidRevision(_testRepository);
				var startRevisionId = CreateTfsRevisionId(revisionValue.ToString());
				var revisionRange = tfs.GetAfterTillHead(startRevisionId, ConfigHelper.Instance.PageSize).First();
				TfsRevisionId fromChangeSet = revisionRange.FromChangeset;

				TfsRevisionId fromExpected = CreateTfsRevisionId((revisionValue + 1).ToString(CultureInfo.InvariantCulture));
				fromChangeSet.Value.Should(Be.EqualTo(fromExpected.Value));

				TfsRevisionId toChangeSet = revisionRange.ToChangeset;
				toChangeSet.Value.Should(Be.EqualTo(maxChangeSetId));
			}
		}

		private static TfsRevisionId CreateTfsRevisionId(string value)
		{
			return new TfsRevisionId { Value = value, Time = DateTime.MinValue.ToUniversalTime() };
		}

		[Test]
		public void ShouldRetrieveRevisionsFromAndBefore()
		{
			using (var tfs = CreateTfs())
			{
				var revisionRange =
										tfs.GetFromAndBefore(CreateTfsRevisionId("12"), CreateTfsRevisionId("18"), ConfigHelper.Instance.PageSize).Single();
				TfsRevisionId fromChangeSet = revisionRange.FromChangeset;

				TfsRevisionId fromExpected = CreateTfsRevisionId("12");
				fromChangeSet.Value.Should(Be.EqualTo(fromExpected.Value));

				TfsRevisionId toExpected = CreateTfsRevisionId("18");
				TfsRevisionId toChangeSet = revisionRange.ToChangeset;
				toChangeSet.Value.Should(Be.EqualTo(toExpected.Value));
			}
		}

		[Test]
		public void ShouldRetrieveAuthors()
		{
			using (var tfs = CreateTfs())
			{
				var authors = tfs.RetrieveAuthors(new DateRange(TfsRevisionId.UtcTimeMin, DateTime.Now));

				//string login;
				//if (string.IsNullOrEmpty(ConfigHelper.Instance.Domen))
				//    login = ConfigHelper.Instance.Login;
				//else
				//    login = string.Concat(ConfigHelper.Instance.Domen, "\\", ConfigHelper.Instance.Login);

				authors.FirstOrDefault(x => string.Equals(x, "artgroup\\msuhinin", StringComparison.OrdinalIgnoreCase)).Should(Be.Not.Null);
			}
		}

		[Test]
		public void ShouldGetRevisions()
		{
			AssertCommits(
								new[] { "first commit", "", "first commit", "second commit", "first commit to branch 1", 
                    "first commit to branch 2", "second commit to branch 1", "second commit to branch 2" },
								"12", "19");
		}

		[Test]
		public void ShouldRetrieveRevisionsWithModifiedFiles()
		{
			using (var tfs = CreateTfs())
			{
				TfsRevisionId startRevisionId = CreateTfsRevisionId("14");
				var revisionRange = tfs.GetFromTillHead(startRevisionId, ConfigHelper.Instance.PageSize).First();

				var revision = tfs.GetRevisions(revisionRange).OrderBy(x => int.Parse(x.Id.Value)).First();

				AssertEqual(revision.Entries, new[] { new RevisionEntryInfo { Path = "$/TeamProject1/TestProject/Class1.cs", Action = FileActionEnum.Modify } });
			}
		}

		[Test]
		public void ShouldRetrieveRevisionsWithAddedFiles()
		{
			using (var tfs = CreateTfs())
			{
				TfsRevisionId startRevisionId = CreateTfsRevisionId("12");
				var revisionRange = tfs.GetFromTillHead(startRevisionId, ConfigHelper.Instance.PageSize).First();

				var revision = tfs.GetRevisions(revisionRange).OrderBy(x => int.Parse(x.Id.Value)).First();

				AssertEqual(revision.Entries, new[]
				{
                    new RevisionEntryInfo {Path = "$/TeamProject1/TestProject", Action = FileActionEnum.Add}, // tfs auxiliary file
				    new RevisionEntryInfo {Path = "$/TeamProject1/TestProject/Class1.cs", Action = FileActionEnum.Add},
				    new RevisionEntryInfo {Path = "$/TeamProject1/TestProject/Properties", Action = FileActionEnum.Add},
				    new RevisionEntryInfo {Path = "$/TeamProject1/TestProject/TestProject.csproj", Action = FileActionEnum.Add},
				    new RevisionEntryInfo {Path = "$/TeamProject1/TestProject/TestProject.sln", Action = FileActionEnum.Add},
                    new RevisionEntryInfo {Path = "$/TeamProject1/TestProject/Properties/AssemblyInfo.cs", Action = FileActionEnum.Add}
				});
			}
		}

		[Test]
		public void ShouldRetrieveRevisionsWithRemovedFiles()
		{
			var testRepo = new TfsTestRepositoryWithFileDeleted();
			_tfsRepoUri = testRepo.Uri.ToString();
			using (var tfs = CreateTfs())
			{
				TfsRevisionId startRevisionId = CreateTfsRevisionId("11");
				var revisionRange = tfs.GetFromTillHead(startRevisionId, ConfigHelper.Instance.PageSize).Single();

				var revision = tfs.GetRevisions(revisionRange).OrderByDescending(x => int.Parse(x.Id.Value)).First();

				AssertEqual(revision.Entries,
										new[]
                        {
                            new RevisionEntryInfo {Path = "$/TestRepositoryWithFileDeleted/ProjectWithDeletedFile-branch1/Class2.cs", Action = FileActionEnum.Delete},
                            new RevisionEntryInfo {Path = "$/TestRepositoryWithFileDeleted/ProjectWithDeletedFile-branch1/ProjectWithDeletedFile.csproj", Action = FileActionEnum.Modify},
                        });
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

		private void AssertCommits(string[] commits, string startRevision = "1", string endRevision = "19")
		{
			using (var tfs = CreateTfs())
			{
				TfsRevisionId startRevisionId = CreateTfsRevisionId(startRevision);
				TfsRevisionId endRevisionId = CreateTfsRevisionId(endRevision);
				var revisionRange = tfs.GetFromAndBefore(startRevisionId, endRevision, ConfigHelper.Instance.PageSize).Single();

				var revisions = tfs.GetRevisions(revisionRange);
				revisions.Select(x => x.Comment).ToArray().Should(Be.EquivalentTo(commits));
				revisions.Select(x => x.Author).Distinct().ToArray().Should(Be.EquivalentTo(new[] { "ARTGROUP\\msuhinin" }));
			}
		}

		[Test]
		public void ShouldCheckCorrectRevision()
		{
			using (var tfs = CreateTfs())
			{
				TfsRevisionId correctRevisionId = CreateTfsRevisionId("12");
				var errors = new PluginProfileErrorCollection();
				tfs.CheckRevision(correctRevisionId, errors);
				errors.Should(Be.Empty);
			}
		}

		[Test]
		public void ShouldCheckIncorrectRevision()
		{
			using (var tfs = CreateTfs())
			{
				TfsRevisionId correctRevisionId = CreateTfsRevisionId("-1");
				var errors = new PluginProfileErrorCollection();
				tfs.CheckRevision(correctRevisionId, errors);

				string localizedMessage = "Revision: Specify a start revision number in the range of 1 - 2147483647.";
				errors.Single().ToString().Should(Be.EqualTo(localizedMessage));
			}
		}

		[Test]
		public void ShouldReCreateRepositoryWhenUriChanged()
		{
			using (CreateTfs())
			{
			}

			var testRepo = new TfsTestRepositoryWithMergeCommit();
			_tfsRepoUri = testRepo.Uri.ToString();

			AssertCommits(new[] { "first commit", "second commit", "third commit" }, "5", "7");
		}

		[Test]
		public void ShouldSaveRevisionIdTpIdRelation()
		{
			var revisionId = _testRepository.Commit(@"$/TeamProject1/TestProject/Class1.cs", "///////////", "#124");
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

			using (var tfs = CreateTfs())
			{
				int revisionValue = GetValidRevision(_testRepository);
				var startRevisionId = CreateTfsRevisionId(revisionValue.ToString());
				var revisionRange = tfs.GetFromTillHead(startRevisionId, ConfigHelper.Instance.PageSize).First();

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

			using (var tfs = CreateTfs())
			{
				var startRevisionId = CreateTfsRevisionId("1");
				var revisionRange = tfs.GetFromTillHead(startRevisionId, ConfigHelper.Instance.PageSize).First();

				transportMock.HandleLocalMessage(_profile, new NewRevisionRangeDetectedLocalMessage { Range = revisionRange });

				ObjectFactory.GetInstance<IStorageRepository>().Get<bool>().FirstOrDefault().Should(Be.False);
			}
		}

		#region Helpers

		private TfsVersionControlSystem CreateTfs()
		{
			return CreateTfs(this);
		}

		private TfsVersionControlSystem CreateTfs(ISourceControlConnectionSettingsSource settings)
		{
			return new TfsVersionControlSystem(
					settings,
					ObjectFactory.GetInstance<ICheckConnectionErrorResolver>(),
					ObjectFactory.GetInstance<IActivityLogger>(),
					ObjectFactory.GetInstance<IDiffProcessor>());
		}

		#endregion
	}
}