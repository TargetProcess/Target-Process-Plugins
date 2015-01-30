using System.Linq;
using System.Reflection;
using NUnit.Framework;
using StructureMap;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Commands;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Messages.EntityLifecycle.Queries;
using Tp.Integration.Testing.Common;
using Tp.Search.Bus;
using Tp.Search.Bus.Data;
using Tp.Search.Messages;
using Tp.Search.Model.Query;
using Tp.Testing.Common.NUnit;

namespace Tp.Search.Tests
{
	[TestFixture]
	[Category("PartPlugins1")]
	class GeneralProjectChangedSagaTest : SearchTestBase
	{
		private TransportMock _transport;
		private GeneralDTO[] _generals;
		private CommentDTO[] _comments;

		protected override void OnSetup()
		{
			base.OnSetup();
			_transport = TransportMock.CreateWithoutStructureMapClear(typeof(SearcherProfile).Assembly, new[] { typeof(SearcherProfile).Assembly }, new Assembly[] { });
			_generals = new[]
				{
					new GeneralDTO {ID = 2, Name = "Second general", EntityTypeID = QueryEntityTypeProvider.USERSTORY_TYPE_ID, ParentProjectID = 1},
					new GeneralDTO {ID = 3, Name = "Third general", EntityTypeID = QueryEntityTypeProvider.BUG_TYPE_ID, ParentProjectID = 1},
				};

			_comments = new[]
				{
					new CommentDTO {GeneralID = 2, CommentID = 1, Description = "FirstDescription"},
				};

			_transport.On<GeneralQuery>().Reply(x => ReplyOnEntityQuery<GeneralQuery, GeneralDTO, GeneralQueryResult>(x, _generals));
			_transport.On<AssignableQuery>().Reply(x => ReplyOnAssignableQuery(x, new AssignableDTO[]{}));
			_transport.On<TestCaseQuery>().Reply(x => new TestCaseQueryResult { Dtos = new TestCaseDTO[] { }, QueryResultCount = 0, TotalQueryResultCount = 0, FailedDtosCount = 0 });
			_transport.On<ImpedimentQuery>().Reply(x => new ImpedimentQueryResult { Dtos = new ImpedimentDTO[] { }, QueryResultCount = 0, TotalQueryResultCount = 0, FailedDtosCount = 0 });
			_transport.On<ReleaseProjectQuery>().Reply(x => new ReleaseProjectQueryResult { Dtos = new ReleaseProjectDTO[] { }, QueryResultCount = 0, TotalQueryResultCount = 0, FailedDtosCount = 0 });
			_transport.On<CommentQuery>().Reply(x => ReplyOnEntityQuery<CommentQuery, CommentDTO, CommentQueryResult>(x, _comments));
		}

		[Test]
		public void ShouldBuildIndexesCorrectlyOnProfileCreation()
		{
			var profile = _transport.AddProfile("Test", new SearcherProfile());
			var queryRunner = ObjectFactory.GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "Description",
				EntityTypeId = QueryEntityTypeProvider.COMMENT_TYPE_ID,
				ProjectIds = new[] { 1 }
			});
			result.Total.Should(Be.EqualTo(1));
			
			var generalDto = _generals.First();
			var newProjectId = 2;
			_transport.HandleMessageFromTp(profile, new UserStoryUpdatedMessage
			{
				Dto = new UserStoryDTO
				{
					ID = generalDto.ID,
					EntityTypeID = generalDto.EntityTypeID,
					ProjectID = newProjectId
				},
				ChangedFields = new[] { UserStoryField.ProjectID}
			});

			_transport.HandleLocalMessage(profile, new GeneralProjectChangedLocalMessage { ProjectId = newProjectId, GeneralId = generalDto.ID.Value });

			queryRunner = ObjectFactory.GetInstance<QueryRunner>();
			queryRunner.Run(new QueryData
			{
				Query = "Description",
				EntityTypeId = QueryEntityTypeProvider.COMMENT_TYPE_ID,
				ProjectIds = new[] { 1 }
			}).Total.Should(Be.EqualTo(0));

			queryRunner = ObjectFactory.GetInstance<QueryRunner>();
			queryRunner.Run(new QueryData
			{
				Query = "Description",
				EntityTypeId = QueryEntityTypeProvider.COMMENT_TYPE_ID,
				ProjectIds = new[] { 2 }
			}).Total.Should(Be.EqualTo(1));
		}
	}
}
