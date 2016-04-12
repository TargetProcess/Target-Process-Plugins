using System.Linq;
using System.Reflection;
using NUnit.Framework;
using StructureMap;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Messages.EntityLifecycle.Queries;
using Tp.Integration.Testing.Common;
using Tp.Search.Bus;
using Tp.Search.Bus.Data;
using Tp.Search.Model.Query;
using Tp.Testing.Common.NUnit;

namespace Tp.Search.Tests
{
	[TestFixture]
	[Category("PartPlugins1")]
	class AssignableSquadChangedSagaTest : SearchTestBase
	{
		private TransportMock _transport;
		private AssignableDTO[] _assignables;
		private AssignableSquadDTO[] _assignableSquads;
		private CommentDTO[] _comments;

		protected override void OnSetup()
		{
			base.OnSetup();
			_transport = TransportMock.CreateWithoutStructureMapClear(typeof(SearcherProfile).Assembly, new[] { typeof(SearcherProfile).Assembly }, new Assembly[] { });
			_assignables = new[]
			{
				new AssignableDTO {ID = 2, Name = "Second general", EntityTypeID = QueryEntityTypeProvider.USERSTORY_TYPE_ID, ProjectID = 1, SquadID = 1, },
				new AssignableDTO {ID = 3, Name = "Third general", EntityTypeID = QueryEntityTypeProvider.BUG_TYPE_ID, ProjectID = 1, SquadID = 1},
			};
			var generals = _assignables.Select(a => new GeneralDTO {ID = a.ID, Name = a.Name, EntityTypeID = a.EntityTypeID, ParentProjectID = a.ProjectID}).ToArray();

			_comments = new[]
			{
				new CommentDTO {GeneralID = 2, CommentID = 1, Description = "FirstDescription"},
			};

			_assignableSquads = new[]
			{
				new AssignableSquadDTO { AssignableSquadID = 1, AssignableID = 2, SquadID = 1}
			};

			_transport.On<GeneralQuery>().Reply(x => ReplyOnEntityQuery<GeneralQuery, GeneralDTO, GeneralQueryResult>(x, generals));
			_transport.On<AssignableQuery>().Reply(x => ReplyOnEntityQuery<AssignableQuery, AssignableDTO, AssignableQueryResult>(x, _assignables));
			_transport.On<ImpedimentQuery>().Reply(x => new ImpedimentQueryResult { Dtos = new ImpedimentDTO[] { }, QueryResultCount = 0, TotalQueryResultCount = 0, FailedDtosCount = 0 });
			_transport.On<ReleaseProjectQuery>().Reply(x => new ReleaseProjectQueryResult { Dtos = new ReleaseProjectDTO[] { }, QueryResultCount = 0, TotalQueryResultCount = 0, FailedDtosCount = 0 });
			_transport.On<RetrieveAllAssignableSquadsQuery>().Reply(x => ReplyOnEntityQuery<RetrieveAllAssignableSquadsQuery, AssignableSquadDTO, AssignableSquadQueryResult>(x, _assignableSquads));
			_transport.On<CommentQuery>().Reply(x => ReplyOnEntityQuery<CommentQuery, CommentDTO, CommentQueryResult>(x, _comments));
			_transport.On<TestStepQuery>().Reply(x => ReplyOnEntityQuery<TestStepQuery, TestStepDTO, TestStepQueryResult>(x, new TestStepDTO[0]));
		}

		[Test]
		public void ShouldUpdateCommentTeamIndexWhenAddAssignableSquad()
		{
			var asssignable = _assignables.First();
			var oldTeamId = asssignable.SquadID.Value;
			const int newTeamId = 2;

			var profile = _transport.AddProfile("Test", new SearcherProfile());
			var queryRunner = ObjectFactory.GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "Description",
				EntityTypeId = QueryEntityTypeProvider.COMMENT_TYPE_ID,
				TeamIds = new[] { oldTeamId }
			});
			result.Total.Should(Be.EqualTo(1), "result.Total.Should(Be.EqualTo(1))");

			_transport.HandleMessageFromTp(profile, new AssignableSquadCreatedMessage
			{
				Dto = new AssignableSquadDTO
				{
					ID = 1,
					AssignableID = asssignable.AssignableID,
					SquadID = newTeamId
				}
			});

			queryRunner = ObjectFactory.GetInstance<QueryRunner>();
			var oldTeamResult = queryRunner.Run(new QueryData
			{
				Query = "Description",
				EntityTypeId = QueryEntityTypeProvider.COMMENT_TYPE_ID,
				TeamIds = new[] { oldTeamId }
			});
			oldTeamResult.Total.Should(Be.EqualTo(1), "oldTeamResult.Total.Should(Be.EqualTo(1))");

			queryRunner = ObjectFactory.GetInstance<QueryRunner>();
			var updatedTeam = queryRunner.Run(new QueryData
			{
				Query = "Description",
				EntityTypeId = QueryEntityTypeProvider.COMMENT_TYPE_ID,
				TeamIds = new[] { newTeamId }
			});
			updatedTeam.Total.Should(Be.EqualTo(1), "updatedTeam.Total.Should(Be.EqualTo(1))");
		}

		[Test]
		public void ShouldUpdateCommentTeamIndexWhenUpdateAssignableSquad()
		{
			var asssignable = _assignables.First();
			var oldTeamId = asssignable.SquadID.Value;
			const int newTeamId = 2;

			var profile = _transport.AddProfile("Test", new SearcherProfile());
			var queryRunner = ObjectFactory.GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "Description",
				EntityTypeId = QueryEntityTypeProvider.COMMENT_TYPE_ID,
				TeamIds = new[] { oldTeamId }
			});
			result.Total.Should(Be.EqualTo(1), "result.Total.Should(Be.EqualTo(1))");

			var assignableSquad = _assignableSquads.First();
			_transport.HandleMessageFromTp(profile, new AssignableSquadUpdatedMessage
			{
				Dto = new AssignableSquadDTO
				{
					ID = assignableSquad.AssignableSquadID,
					AssignableID = assignableSquad.AssignableID,
					SquadID = newTeamId
				},
				ChangedFields = new[] { AssignableSquadField.SquadID },
				OriginalDto = assignableSquad
			});

			queryRunner = ObjectFactory.GetInstance<QueryRunner>();
			var oldTeamResult = queryRunner.Run(new QueryData
			{
				Query = "Description",
				EntityTypeId = QueryEntityTypeProvider.COMMENT_TYPE_ID,
				TeamIds = new[] { oldTeamId }
			});
			oldTeamResult.Total.Should(Be.EqualTo(0), "oldTeamResult.Total.Should(Be.EqualTo(0))");

			queryRunner = ObjectFactory.GetInstance<QueryRunner>();
			var updatedTeam = queryRunner.Run(new QueryData
			{
				Query = "Description",
				EntityTypeId = QueryEntityTypeProvider.COMMENT_TYPE_ID,
				TeamIds = new[] { newTeamId }
			});
			updatedTeam.Total.Should(Be.EqualTo(1), "updatedTeam.Total.Should(Be.EqualTo(1))");
		}

		[Test]
		public void ShouldUpdateCommentTeamIndexWhenRemoveAssignableSquad()
		{
			var asssignable = _assignables.First();

			var profile = _transport.AddProfile("Test", new SearcherProfile());
			var queryRunner = ObjectFactory.GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "Description",
				EntityTypeId = QueryEntityTypeProvider.COMMENT_TYPE_ID,
				TeamIds = new[] { asssignable.SquadID.Value }
			});
			result.Total.Should(Be.EqualTo(1), "result.Total.Should(Be.EqualTo(1))");

			_transport.HandleMessageFromTp(profile, new AssignableSquadDeletedMessage
			{
				Dto = new AssignableSquadDTO
				{
					ID = 1,
					AssignableID = asssignable.AssignableID,
					SquadID = asssignable.SquadID.Value
				}
			});

			queryRunner = ObjectFactory.GetInstance<QueryRunner>();
			var oldTeamResult = queryRunner.Run(new QueryData
			{
				Query = "Description",
				EntityTypeId = QueryEntityTypeProvider.COMMENT_TYPE_ID,
				TeamIds = new[] { asssignable.SquadID.Value }
			});
			oldTeamResult.Total.Should(Be.EqualTo(0), "oldTeamResult.Total.Should(Be.EqualTo(0))");
		}
		
	}
}
