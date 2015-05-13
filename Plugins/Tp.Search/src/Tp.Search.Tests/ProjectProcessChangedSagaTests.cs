using System.Reflection;
using NUnit.Framework;
using StructureMap;
using Tp.Integration.Common;
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
	class ProjectProcessChangedSagaTests: SearchTestBase
	{
		private TransportMock _transport;
		private AssignableDTO[] _assignables;
		private GeneralDTO[] _generals;

		protected override void OnSetup()
		{
			base.OnSetup();
			_transport = TransportMock.CreateWithoutStructureMapClear(typeof(SearcherProfile).Assembly, new[] { typeof(SearcherProfile).Assembly }, new Assembly[] { });
			_generals = new[]
				{
					new GeneralDTO {ID = 2, Name = "Second general", EntityTypeID = QueryEntityTypeProvider.USERSTORY_TYPE_ID, ParentProjectID = 1},
					new GeneralDTO {ID = 3, Name = "Third general", EntityTypeID = QueryEntityTypeProvider.BUG_TYPE_ID, ParentProjectID = 1},
					new GeneralDTO {ID = 4, Name = "Fourth general", EntityTypeID = QueryEntityTypeProvider.BUG_TYPE_ID, ParentProjectID = 2},
				};

			_assignables = new[]
				{
					new AssignableDTO {ID = 2, EntityStateID = 11, SquadID = 10, ProjectID = 1, EntityTypeID = QueryEntityTypeProvider.USERSTORY_TYPE_ID},
					new AssignableDTO {ID = 3, EntityStateID = 12, SquadID = 11, ProjectID = 1, EntityTypeID = QueryEntityTypeProvider.BUG_TYPE_ID},
					new AssignableDTO {ID = 4, EntityStateID = 12, SquadID = 11, ProjectID = 2, EntityTypeID = QueryEntityTypeProvider.BUG_TYPE_ID}
				};

			_transport.On<GeneralQuery>().Reply(x => ReplyOnEntityQuery<GeneralQuery, GeneralDTO, GeneralQueryResult>(x, _generals));

			_transport.On<AssignableQuery>().Reply(x => ReplyOnAssignableQuery(x, _assignables));
			
			_transport.On<CommentQuery>().Reply(x => new CommentQueryResult{Dtos = new CommentDTO[]{}, QueryResultCount = 0, TotalQueryResultCount = 0, FailedDtosCount = 0});

			_transport.On<TestStepQuery>().Reply(x => new TestStepQueryResult { Dtos = new TestStepDTO[] { }, QueryResultCount = 0, TotalQueryResultCount = 0, FailedDtosCount = 0 });

		}

		[Test]
		public void ShouldBuildIndexesCorrectlyOnProfileCreation()
		{
			var profile = _transport.AddProfile("Test", new SearcherProfile());
			CheckByState("general", 11, new[] { 1, 2 });
			CheckByState("general", 12, new[] { 1, 2 }, resultCount:2);

			_assignables = new[]
				{
					new AssignableDTO {ID = 2, EntityStateID = 14, SquadID = 10, ProjectID = 1, EntityTypeID = QueryEntityTypeProvider.USERSTORY_TYPE_ID},
					new AssignableDTO {ID = 3, EntityStateID = 15, SquadID = 11, ProjectID = 1, EntityTypeID = QueryEntityTypeProvider.BUG_TYPE_ID}
				};

			_transport.HandleLocalMessage(profile, new ProjectProcessChangedLocalMessage{ProjectId = 1});

			CheckByState("general", 14, new[] { 1, 2 });
			CheckByState("general", 15, new[] { 1, 2 });
			CheckByState("general", 12, new[] { 1, 2 }, resultCount:1);

			var queryRunner = ObjectFactory.GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "general",
				EntityStateIds = new[] { 11 },
				ProjectIds = new[] { 1 }
			});
			result.Total.Should(Be.EqualTo(0));
		}
	}
}
