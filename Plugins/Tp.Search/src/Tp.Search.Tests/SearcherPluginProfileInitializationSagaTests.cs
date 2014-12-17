using System.Reflection;
using NUnit.Framework;
using StructureMap;
using Tp.Integration.Common;
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
	public class SearcherPluginProfileInitializationSagaTests : SearchTestBase
	{
		private TransportMock _transport;

		private GeneralDTO[] _generals;
		private CommentDTO[] _comments;
		private AssignableDTO[] _assignables;
		private TestCaseDTO[] _testCases;

		protected override void OnSetup()
		{
			base.OnSetup();
			_transport = TransportMock.CreateWithoutStructureMapClear(typeof(SearcherProfile).Assembly, new[] { typeof(SearcherProfile).Assembly }, new Assembly[] { });
			_generals = new[]
				{
					new GeneralDTO {ID = 1, Name = "First general", EntityTypeID = 4, ParentProjectID = 1}, 
					new GeneralDTO {ID = 2, Name = "Second general", EntityTypeID = QueryEntityTypeProvider.USERSTORY_TYPE_ID, ParentProjectID = 1},
					new GeneralDTO {ID = 3, Name = "Third general", EntityTypeID = QueryEntityTypeProvider.BUG_TYPE_ID, ParentProjectID = 1},
					new GeneralDTO {ID = 4, Name = "Fourth general", EntityTypeID = QueryEntityTypeProvider.TESTCASE_TYPE_ID, ParentProjectID = 1}
				};

			_comments = new[]
				{
					new CommentDTO {GeneralID = 1, CommentID = 1, Description = "FirstDescription"},
					new CommentDTO {GeneralID = 2, CommentID = 2, Description = "SecondDescription"}
				};

			_assignables = new[]
				{
					new AssignableDTO {ID = 2, EntityStateID = 11, SquadID = 10, ProjectID = 1, EntityTypeID = QueryEntityTypeProvider.USERSTORY_TYPE_ID},
					new AssignableDTO {ID = 3, EntityStateID = 12, SquadID = 11, ProjectID = 1, EntityTypeID = QueryEntityTypeProvider.BUG_TYPE_ID}
				};
			_testCases = new[] { new TestCaseDTO { ID = 4, Description = "Test Case Description", ProjectID = 1, EntityTypeID=QueryEntityTypeProvider.TESTCASE_TYPE_ID } };

			_transport.On<GeneralQuery>().Reply(x => ReplyOnEntityQuery<GeneralQuery, GeneralDTO, GeneralQueryResult>(x, _generals));
			_transport.On<CommentQuery>().Reply(x => ReplyOnEntityQuery<CommentQuery, CommentDTO, CommentQueryResult>(x, _comments));
			_transport.On<TestCaseQuery>().Reply(x => ReplyOnEntityQuery<TestCaseQuery, TestCaseDTO, TestCaseQueryResult>(x, _testCases));
			_transport.On<ImpedimentQuery>().Reply(x => new ImpedimentQueryResult { Dtos = new ImpedimentDTO[] { }, QueryResultCount = 0, TotalQueryResultCount = 0, FailedDtosCount = 0 });
			_transport.On<AssignableQuery>().Reply(x => ReplyOnAssignableQuery(x, _assignables));
		}

		[Test]
		public void ShouldBuildIndexesCorrectlyOnProfileCreation()
		{
			ObjectFactory.GetInstance<TransportMock>().AddProfile("Test", new SearcherProfile());

			CheckOnlyOneEntity("First general");
			CheckOnlyOneEntity("Second general");
			CheckOnlyOneEntity("Third general");

			CheckOnlyOneEntity("FirstDescription");
			CheckOnlyOneEntity("SecondDescription");
			
			CheckOnlyOneEntity("Test Case Description");

			CheckNothing("Nothing");

			CheckByState("general", 11, new[] { 1 });
			CheckByState("general", 12, new[] { 1 });
			CheckByState("general", 13, new[] { 1 }, resultCount:0);

			CheckBySquad("general", 10);
			CheckBySquad("general", 11);
			CheckBySquad("general", 12, resultCount:0);
		}

		private void CheckNothing(string queryString)
		{
			var queryRunner = ObjectFactory.GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = queryString,
				ProjectIds = new[] { 1 }
			});
			result.Total.Should(Be.EqualTo(0));
		}

		private static void CheckOnlyOneEntity(string queryString)
		{
			var queryRunner = ObjectFactory.GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
				{
					Query = queryString,
					ProjectIds = new[] {1}
				});
			result.Total.Should(Be.EqualTo(1));
		}

		private static void CheckBySquad(string queryString, int squadId, int resultCount = 1)
		{
			var queryRunner = ObjectFactory.GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = queryString,
				TeamIds = new[] { squadId},
				ProjectIds = new[] { 1 }
			});
			result.Total.Should(Be.EqualTo(resultCount));
		}
	}
}
