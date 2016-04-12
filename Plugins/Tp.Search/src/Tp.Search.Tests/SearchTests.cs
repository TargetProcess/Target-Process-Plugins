using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using NUnit.Framework;
using Tp.Core;
using Tp.Integration.Common;
using Tp.Integration.Messages.Entities;
using Tp.Search.Bus.Data;
using Tp.Search.Model.Entity;
using Tp.Search.Model.Query;
using Tp.Testing.Common.NUnit;

namespace Tp.Search.Tests
{
	[TestFixture]
	[Category("PartPlugins1")]
	public class SearchTests : SearchTestBase
	{
		[Test]
		public void SearchUserStoryByNameAndDescription()
		{
			var indexer = GetInstance<IEntityIndexer>();
			var id = 1;
			indexer.AddGeneralIndex(new GeneralDTO
				{
					ID = id,
					Name = "zagzag",
					Description = "bla bla bla",
					EntityTypeID = 4,
					ParentProjectID = 1
				});
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
				{
					Query = "+zagzag +bla",
					ProjectIds = new[] { 1 }
				});
			result.Total.Should(Be.EqualTo(1), "result.Total.Should(Be.EqualTo(1))");
			result.AssignableIds.Should(Be.EquivalentTo(new[]{id.ToString()}), "result.AssignableIds.Should(Be.EquivalentTo(new[]{id.ToString()}))");
		}

		[Test]
		public void SearchUserStoryByCustomField()
		{
			var indexer = GetInstance<IEntityIndexer>();
			var id = 1;
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = id,
				EntityTypeID = 4,
				ParentProjectID = 1,
				CustomFieldsMetaInfo = new[]
					{
						new Field {FieldType = FieldTypeEnum.Text, Value = "CFTextValue1 and CFTextValue2"},
						new Field {FieldType = FieldTypeEnum.RichText, Value = "CFRichTextValue"},
						new Field {FieldType = FieldTypeEnum.DropDown, Value = "CFDropDownValue"},
						new Field {FieldType = FieldTypeEnum.MultipleSelectionList, Value = "MutipleValue1"},
						new Field {FieldType = FieldTypeEnum.URL, Value = "http://localhost/tp"}
					}
			});

			CheckSearch(id, "CFTextValue1");
			CheckSearch(id, "CFTextValue2");
			CheckSearch(id, "CFRichTextValue");
			CheckSearch(id, "MutipleValue1");

			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "http://localhost/tp",
				ProjectIds = new[] { 1 }
			});

			result.Total.Should(Be.EqualTo(0), "result.Total.Should(Be.EqualTo(0))");

			indexer.UpdateGeneralIndex(new GeneralDTO
				{
					ID = id,
					EntityTypeID = 4,
					EntityTypeName = "General",
					ParentProjectID = 1,
					CustomFieldsMetaInfo = new[]
						{
							new Field {FieldType = FieldTypeEnum.Text, Value = "CFTextNewValue"}
						}
				}, new[] { GeneralField.CustomField60 });

			CheckSearch(id, "CFTextNewValue");
		}

		private void CheckSearch(int id, string value)
		{
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
				{
					Query = value,
					ProjectIds = new[] {1}
				});
			result.Total.Should(Be.EqualTo(1), "result.Total.Should(Be.EqualTo(1))");
			result.AssignableIds.Should(Be.EquivalentTo(new[] {id.ToString()}), "result.AssignableIds.Should(Be.EquivalentTo(new[] {id.ToString()}))");
		}

		[Test]
		public void SearchUserStoryByNameWithQuotes()
		{
			var indexer = GetInstance<IEntityIndexer>();
			var id = 1;
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = id,
				Name = "zagzag",
				Description = "bla bla bla",
				EntityTypeID = 4,
				ParentProjectID = 1
			});
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "\"zagzag\"",
				ProjectIds = new[] { 1 }
			});
			result.Total.Should(Be.EqualTo(1), "result.Total.Should(Be.EqualTo(1))");
			result.AssignableIds.Should(Be.EquivalentTo(new[] { id.ToString() }), "result.AssignableIds.Should(Be.EquivalentTo(new[] { id.ToString() }))");
		}

		[Test]
		public void SearchUserStoryByNameWithSpacebarInQuotes()
		{
			var indexer = GetInstance<IEntityIndexer>();
			var id = 1;
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = id,
				Name = "zagzag",
				Description = "bla bla bla",
				EntityTypeID = 4,
				ParentProjectID = 1
			});
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "\" zagzag\"  ",
				ProjectIds = new[] { 1 }
			});
			result.Total.Should(Be.EqualTo(1), "result.Total.Should(Be.EqualTo(1))");
			result.AssignableIds.Should(Be.EquivalentTo(new[] { id.ToString() }), "result.AssignableIds.Should(Be.EquivalentTo(new[] { id.ToString() }))");
		}

		[Test]
		public void SearchUserStoryByNameWithSemicolumnInQuotes()
		{
			var indexer = GetInstance<IEntityIndexer>();
			var id = 2;
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = 1,
				Name = "zagzag",
				Description = "Could not continue scan with NOLOCK due to data movement",
				EntityTypeID = 4,
				ParentProjectID = 1
			});
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = id,
				Name = "zagzag",
				Description = "SqlClient.SqlException: Could not continue scan with NOLOCK due to data movement",
				EntityTypeID = 4,
				ParentProjectID = 1
			});
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "\"SqlClient. SqlException: Could not continue scan with NOLOCK due to data movement\"",
				ProjectIds = new[]{1}
			});
			result.Total.Should(Be.EqualTo(1), "result.Total.Should(Be.EqualTo(1))");
			result.AssignableIds.Should(Be.EquivalentTo(new[] { id.ToString() }), "result.AssignableIds.Should(Be.EquivalentTo(new[] { id.ToString() }))");
		}

		[Test]
		public void SearchUserStoryByNameWithSeveralSpacebarInQuotes()
		{
			var indexer = GetInstance<IEntityIndexer>();
			var id = 1;
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = id,
				Name = "zagzag",
				Description = "bla bla bla",
				EntityTypeID = 4,
				ParentProjectID = 1
			});
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "\" zagzag     a\"  ",
				ProjectIds = new[] { 1 }
			});
			result.Total.Should(Be.EqualTo(1), "result.Total.Should(Be.EqualTo(1))");
			result.AssignableIds.Should(Be.EquivalentTo(new[] { id.ToString() }), "result.AssignableIds.Should(Be.EquivalentTo(new[] { id.ToString() }))");
		}

		[Test]
		public void SearchUserStoryByNameWithSeveralSpacebarInQuotes2()
		{
			var indexer = GetInstance<IEntityIndexer>();
			var id = 1;
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = id,
				Name = "zagzag",
				Description = "local@time",
				EntityTypeID = 4,
				ParentProjectID = 1
			});
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = 2,
				Name = "zagzag",
				Description = "local",
				EntityTypeID = 4,
				ParentProjectID = 1
			});
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "\"local@time\"",
				ProjectIds = new[]{1}
			});
			result.Total.Should(Be.EqualTo(1), "result.Total.Should(Be.EqualTo(1))");
			result.AssignableIds.Should(Be.EquivalentTo(new[] { id.ToString() }), "result.AssignableIds.Should(Be.EquivalentTo(new[] { id.ToString() }))");
		}

		[Test]
		public void SearchUserStoryByNameWithSeveralSpacebarInQuotes4()
		{
			var indexer = GetInstance<IEntityIndexer>();
			var id = 1;
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = id,
				Name = "zag",
				Description = "local",
				EntityTypeID = 4,
				ParentProjectID = 1
			});
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = 2,
				Name = "zag",
				Description = "time",
				EntityTypeID = 4,
				ParentProjectID = 1
			});
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "+zag -time",
				ProjectIds = new[] { 1 }
			});
			result.Total.Should(Be.EqualTo(1), "result.Total.Should(Be.EqualTo(1))");
			result.AssignableIds.Should(Be.EquivalentTo(new[] { id.ToString() }), "result.AssignableIds.Should(Be.EquivalentTo(new[] { id.ToString() }))");
		}


		[Test]
		public void SearchUserStoryByNameWithSeveralSpacebarInQuotes3()
		{
			var indexer = GetInstance<IEntityIndexer>();
			var ids = new[]{1,2};
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = ids[0],
				Name = "zagzag",
				Description = "local@time",
				EntityTypeID = 4,
				ParentProjectID = 1
			});
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = ids[1],
				Name = "zagzag",
				Description = "zagzaglocal@time",
				EntityTypeID = 4,
				ParentProjectID = 1
			});
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "local@time",
				ProjectIds = new[] { 1 }
			});
			result.Total.Should(Be.EqualTo(2), "result.Total.Should(Be.EqualTo(2))");
			result.AssignableIds.Should(Be.EquivalentTo(ids.Select(x => x.ToString()).ToArray()), "result.AssignableIds.Should(Be.EquivalentTo(ids.Select(x => x.ToString()).ToArray()))");
		}

		[Test]
		public void SearchUserStoryByNameWithDotInTheEnd()
		{
			var indexer = GetInstance<IEntityIndexer>();
			var id = 1;
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = 1,
				Name = "zag zag.",
				Description = "bla bla bla",
				EntityTypeID = 4,
				ParentProjectID = 1
			});
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "\"zag zag.\"",
				ProjectIds = new[] { 1 }
			});
			result.Total.Should(Be.EqualTo(1), "result.Total.Should(Be.EqualTo(1))");
			result.AssignableIds.Should(Be.EquivalentTo(new[] { id.ToString() }), "result.AssignableIds.Should(Be.EquivalentTo(new[] { id.ToString() }))");
		}
		
		[Test]
		public void SearchUserStoryByNameWithPlus()
		{
			var indexer = GetInstance<IEntityIndexer>();
			var id = 1;
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = id,
				Name = "zag zag.",
				Description = "bla bla bla",
				EntityTypeID = 4,
				ParentProjectID = 1
			});
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = 2,
				Name = "zagzag",
				Description = "bla bla bla",
				EntityTypeID = 4,
				ParentProjectID = 1
			});
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "+bla.zag.",
				ProjectIds = new[] { 1 }
			});
			result.Total.Should(Be.EqualTo(1), "result.Total.Should(Be.EqualTo(1))");
			result.AssignableIds.Should(Be.EquivalentTo(new[] { id.ToString() }), "result.AssignableIds.Should(Be.EquivalentTo(new[] { id.ToString() }))");
		}

		[Test]
		public void SearchUserStoryByNameWithMinus()
		{
			var indexer = GetInstance<IEntityIndexer>();
			var id = 1;
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = id,
				Name = "zag zag.",
				Description = "bla bla bla",
				EntityTypeID = 4,
				ParentProjectID = 1
			});
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = 2,
				Name = "zag xxx",
				Description = "bla bla bla",
				EntityTypeID = 4,
				ParentProjectID = 1
			});
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "+zag -xxx",
				ProjectIds = new[] { 1 }
			});
			result.Total.Should(Be.EqualTo(1), "result.Total.Should(Be.EqualTo(1))");
			result.AssignableIds.Should(Be.EquivalentTo(new[] { id.ToString() }), "result.AssignableIds.Should(Be.EquivalentTo(new[] { id.ToString() }))");
		}

		[Test]
		public void SearchUserStoryByNameWithDigits()
		{
			var indexer = GetInstance<IEntityIndexer>();
			var id = 1;
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = id,
				Name = "zag zag",
				Description = "22 time",
				EntityTypeID = 4,
				ParentProjectID = 1
			});
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = 2,
				Name = "zag xxx",
				Description = "bla bla bla",
				EntityTypeID = 4,
				ParentProjectID = 1
			});
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "22 time",
				ProjectIds = new[] { 1 }
			});
			result.Total.Should(Be.EqualTo(1), "result.Total.Should(Be.EqualTo(1))");
			result.AssignableIds.Should(Be.EquivalentTo(new[] { id.ToString() }), "result.AssignableIds.Should(Be.EquivalentTo(new[] { id.ToString() }))");
		}

		[Test]
		public void SearchUserStoryByNameWithStar()
		{
			var indexer = GetInstance<IEntityIndexer>();
			var id = 1;
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = id,
				Name = "zag zag. xxx",
				Description = "bla bla bla",
				EntityTypeID = 4,
				ParentProjectID = 1
			});
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = 2,
				Name = "zagzag",
				Description = "bla bla bla",
				EntityTypeID = 4,
				ParentProjectID = 1
			});
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "*zag.xxx",
				ProjectIds = new[] { 1 }
			});
			result.Total.Should(Be.EqualTo(1), "result.Total.Should(Be.EqualTo(1))");
			result.AssignableIds.Should(Be.EquivalentTo(new[] { id.ToString() }), "result.AssignableIds.Should(Be.EquivalentTo(new[] { id.ToString() }))");
		}

		[Test]
		public void SearchUserStoryByProject()
		{
			var indexer = GetInstance<IEntityIndexer>();
			var ids = new[] { 1, 2, 3, 4, 5 };
			const int projectId = 918;
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = ids[0],
				Name = "zag zag. xxx",
				Description = "bla bla bla",
				EntityTypeID = 4,
				ProjectID = 1
			});
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = ids[1],
				Name = "zag zag. xxx",
				Description = "bla bla bla",
				EntityTypeID = 4,
				ProjectID = 2
			});
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = ids[2],
				Name = "zag zag. xxx",
				Description = "bla bla bla",
				EntityTypeID = 4,
				ProjectID = projectId
			});
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = ids[3],
				Name = "zag zag. xxx",
				Description = "bla bla bla",
				EntityTypeID = 4,
				ProjectID = 4
			});
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = ids[4],
				Name = "zag zag. xxx",
				Description = "bla bla bla",
				EntityTypeID = 4,
				ProjectID = 5
			});

			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "*zag.xxx",
				ProjectIds = new[] { projectId, 5 }
			});
			result.Total.Should(Be.EqualTo(2), "result.Total.Should(Be.EqualTo(2))");
			result.AssignableIds.Should(Be.EquivalentTo(new[] { ids[2].ToString(), ids[4].ToString() }), "result.AssignableIds.Should(Be.EquivalentTo(new[] { ids[2].ToString(), ids[4].ToString() }))");
		}

		[Test]
		public void SearchUserStoryByNameWithAtSymbolc()
		{
			var indexer = GetInstance<IEntityIndexer>();
			var ids = new[] { 1, 2 };
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = ids[0],
				Name = "test",
				Description = "",
				EntityTypeID = 4,
				ParentProjectID = 1
			});
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = ids[1],
				Name = "@test",
				Description = "",
				EntityTypeID = 4,
				ParentProjectID = 1
			});

			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "\"@test\"",
				ProjectIds = new[] { 1 }
			});
			result.Total.Should(Be.EqualTo(2), "result.Total.Should(Be.EqualTo(2))");
			result.AssignableIds.Should(Be.EquivalentTo(ids.Select(x => x.ToString()).ToArray()), "result.AssignableIds.Should(Be.EquivalentTo(ids.Select(x => x.ToString()).ToArray()))");
		}
		
		[Test]
		public void SearchUserStoryByEntityState()
		{
			var indexer = GetInstance<IEntityIndexer>();
			int userStoryTypeId = QueryEntityTypeProvider.USERSTORY_TYPE_ID;
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = 1,
				Name = "zagzag",
				Description = "bla bla bla",
				EntityTypeID = userStoryTypeId,
				ParentProjectID = 1
			});
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = 2,
				Name = "qqqqq",
				Description = "wwww",
				EntityTypeID = userStoryTypeId,
				ParentProjectID = 1
			});
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "+zagzag +bla",
				ProjectIds = new[] { 1 }
			});
			result.Total.Should(Be.EqualTo(1), "result.Total.Should(Be.EqualTo(1))");
		}

		[Test]
		public void SearchUserStoryByEntityStates()
		{
			var indexer = GetInstance<IEntityIndexer>();
			var targetEntityStateId = 1;
			var expectedAssignableIds = new[]{1,4};
			int userStoryTypeId = QueryEntityTypeProvider.USERSTORY_TYPE_ID;
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = expectedAssignableIds[0],
				Name = "zagzag",
				Description = "",
				EntityTypeID = userStoryTypeId,
				EntityStateID = targetEntityStateId,
				ProjectID = 1
			}); 
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = 2,
				Name = "zagzag",
				Description = "",
				EntityTypeID = userStoryTypeId,
				EntityStateID = 2,
				ProjectID = 1
			});
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = 3,
				Name = "qqqqq",
				Description = "wwww",
				EntityTypeID = userStoryTypeId,
				EntityStateID = targetEntityStateId,
				ProjectID = 1
			});
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = expectedAssignableIds[1],
				Name = "zagzag qwerty",
				Description = "",
				EntityTypeID = userStoryTypeId,
				EntityStateID = targetEntityStateId,
				ProjectID = 1
			}); 
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "zagzag",
				EntityStateIds = new[]{targetEntityStateId},
				ProjectIds = new[] { 1 }
			});
			result.Total.Should(Be.EqualTo(2), "result.Total.Should(Be.EqualTo(2))");
			result.AssignableIds.Should(Be.EquivalentTo(expectedAssignableIds.Select(s => s.ToString()).ToArray()), "result.AssignableIds.Should(Be.EquivalentTo(expectedAssignableIds.Select(s => s.ToString()).ToArray()))");
		}

		[Test]
		public void SearchUserStoryByTeam()
		{
			var indexer = GetInstance<IEntityIndexer>();
			const int targetTeamId = 1;
			var expectedAssignablesIds = new[] { 1, 4 };
			int userStoryTypeId = QueryEntityTypeProvider.USERSTORY_TYPE_ID;
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = expectedAssignablesIds[0],
				Name = "zagzag",
				Description = "",
				EntityTypeID = userStoryTypeId,
				SquadID = targetTeamId
			});
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = 2,
				Name = "zagzag",
				Description = "",
				EntityTypeID = userStoryTypeId,
				SquadID = 2
			});
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = 3,
				Name = "qqqqq",
				Description = "wwww",
				EntityTypeID = userStoryTypeId,
				SquadID = targetTeamId
			});
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = expectedAssignablesIds[1],
				Name = "zagzag qwerty",
				Description = "",
				EntityTypeID = userStoryTypeId,
				SquadID = targetTeamId
			});
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = 5,
				Name = "zagzag qwerty blabla",
				Description = "",
				EntityTypeID = userStoryTypeId,
			});
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "zagzag",
				TeamIds = new[] { targetTeamId }
			});
			result.Total.Should(Be.EqualTo(2), "result.Total.Should(Be.EqualTo(2))");
			result.AssignableIds.Should(Be.EquivalentTo(expectedAssignablesIds.Select(s => s.ToString()).ToArray()), "result.AssignableIds.Should(Be.EquivalentTo(expectedAssignablesIds.Select(s => s.ToString()).ToArray()))");
		}
		
		[Test]
		public void SearchUserStoryByTeamAfterTeamUpdate()
		{
			var indexer = GetInstance<IEntityIndexer>();
			const int targetTeamId = 1;
			const int newTargetTeamId = 10;
			var expectedAssignablesIds = new[] { 1, 4 };
			int userStoryTypeId = QueryEntityTypeProvider.USERSTORY_TYPE_ID;
			var target = new AssignableDTO
				{
					ID = expectedAssignablesIds[0],
					Name = "zagzag", 
					Description = "", 
					EntityTypeID = userStoryTypeId, 
					SquadID = targetTeamId
				};
			indexer.AddAssignableIndex(target);
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = 2,
				Name = "zagzag",
				Description = "",
				EntityTypeID = userStoryTypeId,
				SquadID = 2
			});
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = 3,
				Name = "qqqqq",
				Description = "wwww",
				EntityTypeID = userStoryTypeId,
				SquadID = targetTeamId
			});
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = expectedAssignablesIds[1],
				Name = "zagzag qwerty",
				Description = "",
				EntityTypeID = userStoryTypeId,
				SquadID = targetTeamId
			});
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = 5,
				Name = "zagzag qwerty blabla",
				Description = "",
				EntityTypeID = userStoryTypeId,
			});
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "zagzag",
				TeamIds = new[] { targetTeamId }
			});
			result.Total.Should(Be.EqualTo(2), "result.Total.Should(Be.EqualTo(2))");
			result.AssignableIds.Should(Be.EquivalentTo(expectedAssignablesIds.Select(s => s.ToString()).ToArray()), "result.AssignableIds.Should(Be.EquivalentTo(expectedAssignablesIds.Select(s => s.ToString()).ToArray()))");

			indexer.UpdateAssignableSquadIndex(new AssignableSquadDTO
			{
				AssignableSquadID = 1,
				AssignableID = target.AssignableID,
				SquadID = newTargetTeamId
			}, new AssignableSquadDTO
			{
				AssignableSquadID = 1,
				AssignableID = target.AssignableID,
				SquadID = targetTeamId
			}, new List<AssignableSquadField> {AssignableSquadField.SquadID});
			target.SquadID = newTargetTeamId;
			indexer.UpdateAssignableIndex(target, new[] {AssignableField.SquadID}, false);

			var newResult = queryRunner.Run(new QueryData
			{
				Query = "zagzag",
				TeamIds = new[] { targetTeamId }
			});
			newResult.Total.Should(Be.EqualTo(1), "newResult.Total.Should(Be.EqualTo(1))");
			newResult.AssignableIds.Should(Be.EquivalentTo(expectedAssignablesIds.Skip(1).Select(s => s.ToString()).ToArray()), "newResult.AssignableIds.Should(Be.EquivalentTo(expectedAssignablesIds.Skip(1).Select(s => s.ToString()).ToArray()))");

			var lastResult = queryRunner.Run(new QueryData
			{
				Query = "zagzag",
				TeamIds = new[] { newTargetTeamId }
			});
			lastResult.Total.Should(Be.EqualTo(1), "lastResult.Total.Should(Be.EqualTo(1))");
			lastResult.AssignableIds.Should(Be.EquivalentTo(expectedAssignablesIds.Take(1).Select(s => s.ToString()).ToArray()), "lastResult.AssignableIds.Should(Be.EquivalentTo(expectedAssignablesIds.Take(1).Select(s => s.ToString()).ToArray()))");
		}

		[Test]
		public void SearchUserStoryByTeamAfterTeamUpdateToNull()
		{
			var indexer = GetInstance<IEntityIndexer>();
			const int targetTeamId = 1;
			const int assignableIdWithNullSquad = 5;
			var expectedAssignablesIds = new[] { 1, 4 };
			int userStoryTypeId = QueryEntityTypeProvider.USERSTORY_TYPE_ID;
			var target = new AssignableDTO
			{
				ID = expectedAssignablesIds[0],
				Name = "zagzag",
				Description = "",
				EntityTypeID = userStoryTypeId,
				SquadID = targetTeamId
			};
			indexer.AddAssignableIndex(target);
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = 2,
				Name = "zagzag",
				Description = "",
				EntityTypeID = userStoryTypeId,
				SquadID = 2
			});
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = 3,
				Name = "qqqqq",
				Description = "wwww",
				EntityTypeID = userStoryTypeId,
				SquadID = targetTeamId
			});
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = expectedAssignablesIds[1],
				Name = "zagzag qwerty",
				Description = "",
				EntityTypeID = userStoryTypeId,
				SquadID = targetTeamId
			});
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = assignableIdWithNullSquad,
				Name = "zagzag qwerty blabla",
				Description = "",
				EntityTypeID = userStoryTypeId,
			});
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "zagzag",
				TeamIds = new[] { targetTeamId }
			});
			result.Total.Should(Be.EqualTo(2), "result.Total.Should(Be.EqualTo(2))");
			result.AssignableIds.Should(Be.EquivalentTo(expectedAssignablesIds.Select(s => s.ToString()).ToArray()), "result.AssignableIds.Should(Be.EquivalentTo(expectedAssignablesIds.Select(s => s.ToString()).ToArray()))");

			indexer.RemoveAssignableSquadIndex(new AssignableSquadDTO
			{
				AssignableSquadID = 1,
				AssignableID = target.AssignableID,
				SquadID = targetTeamId
			});
			target.SquadID = null;
			indexer.UpdateAssignableIndex(target, new[] { AssignableField.SquadID }, isIndexing: false);

			var newResult = queryRunner.Run(new QueryData
			{
				Query = "zagzag",
				TeamIds = new[] { targetTeamId }
			});
			newResult.Total.Should(Be.EqualTo(1), "newResult.Total.Should(Be.EqualTo(1))");
			newResult.AssignableIds.Should(Be.EquivalentTo(expectedAssignablesIds.Skip(1).Select(s => s.ToString()).ToArray()), "newResult.AssignableIds.Should(Be.EquivalentTo(expectedAssignablesIds.Skip(1).Select(s => s.ToString()).ToArray()))");

			var lastResult = queryRunner.Run(new QueryData
			{
				Query = "zagzag",
				TeamIds = null,
				IncludeNoTeam = true
			});
			lastResult.Total.Should(Be.EqualTo(2), "lastResult.Total.Should(Be.EqualTo(2))");
			lastResult.AssignableIds.Should(Be.EquivalentTo(expectedAssignablesIds.Take(1).Concat(new[]{assignableIdWithNullSquad}).Select(s => s.ToString()).ToArray()), "lastResult.AssignableIds.Should(Be.EquivalentTo(expectedAssignablesIds.Take(1).Concat(new[]{assignableIdWithNullSquad}).Select(s => s.ToString()).ToArray()))");
		}

		[Test]
		public void SearchUserStoryByTeamAfterAddAssignableSquad()
		{
			var indexer = GetInstance<IEntityIndexer>();
			const int targetTeamId = 1;
			const int anotherTargetTeamId = 2;
			const int expectedAssignableId = 1;
			const int userStoryTypeId = QueryEntityTypeProvider.USERSTORY_TYPE_ID;
			var target = new AssignableDTO
			{
				ID = expectedAssignableId,
				Name = "zagzag",
				Description = "",
				EntityTypeID = userStoryTypeId,
				SquadID = targetTeamId
			};
			indexer.AddAssignableIndex(target);
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = 2,
				Name = "zagzag",
				Description = "",
				EntityTypeID = userStoryTypeId,
				SquadID = targetTeamId
			});
			
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "zagzag",
				TeamIds = new[] { targetTeamId }
			});
			result.Total.Should(Be.EqualTo(2), "result.Total.Should(Be.EqualTo(2))");
			result.AssignableIds.Contains(expectedAssignableId.ToString()).Should(Be.True, "result.AssignableIds.Contains(expectedAssignableId.ToString()).Should(Be.True)");

			indexer.AddAssignableSquadIndex(new AssignableSquadDTO
			{
				AssignableSquadID = 1,
				AssignableID = expectedAssignableId,
				SquadID = anotherTargetTeamId
			});
						
			var newResult = queryRunner.Run(new QueryData
			{
				Query = "zagzag",
				TeamIds = new[] { targetTeamId }
			});
			newResult.Total.Should(Be.EqualTo(2), "newResult.Total.Should(Be.EqualTo(2))");
			newResult.AssignableIds.Contains(expectedAssignableId.ToString()).Should(Be.True, "newResult.AssignableIds.Contains(expectedAssignableId.ToString()).Should(Be.True)");

			var lastResult = queryRunner.Run(new QueryData
			{
				Query = "zagzag",
				TeamIds = new[] { anotherTargetTeamId }
			});
			lastResult.Total.Should(Be.EqualTo(1), "lastResult.Total.Should(Be.EqualTo(1))");
			lastResult.AssignableIds.Contains(expectedAssignableId.ToString()).Should(Be.True, "lastResult.AssignableIds.Contains(expectedAssignableId.ToString()).Should(Be.True)");
		}

		[Test]
		public void SearchUserStoryByTeamAfterUpdateAssignableSquad()
		{
			var indexer = GetInstance<IEntityIndexer>();
			const int targetTeamId = 1;
			const int anotherTargetTeamId = 2;
			const int expectedAssignableId = 1;
			const int expectedAssignableSquadId = 1;
			const int userStoryTypeId = QueryEntityTypeProvider.USERSTORY_TYPE_ID;
			var target = new AssignableDTO
			{
				ID = expectedAssignableId,
				Name = "zagzag",
				Description = "",
				EntityTypeID = userStoryTypeId,
				SquadID = targetTeamId
			};
			indexer.AddAssignableIndex(target);
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = 2,
				Name = "zagzag",
				Description = "",
				EntityTypeID = userStoryTypeId,
				SquadID = targetTeamId
			});
			var targetAssignableSquad = new AssignableSquadDTO
			{
				AssignableSquadID = expectedAssignableSquadId,
				AssignableID = expectedAssignableId,
				SquadID = targetTeamId
			};
			indexer.AddAssignableSquadIndex(targetAssignableSquad);

			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "zagzag",
				TeamIds = new[] {targetTeamId}
			});
			result.Total.Should(Be.EqualTo(2), "result.Total.Should(Be.EqualTo(2))");
			result.AssignableIds.Contains(expectedAssignableId.ToString()).Should(Be.True, "result.AssignableIds.Contains(expectedAssignableId.ToString()).Should(Be.True)");

			indexer.UpdateAssignableSquadIndex(new AssignableSquadDTO
			{
				AssignableSquadID = expectedAssignableSquadId,
				AssignableID = expectedAssignableId,
				SquadID = anotherTargetTeamId
			}, targetAssignableSquad, new List<AssignableSquadField> {AssignableSquadField.SquadID});

			var newResult = queryRunner.Run(new QueryData
			{
				Query = "zagzag",
				TeamIds = new[] {targetTeamId}
			});
			newResult.Total.Should(Be.EqualTo(1), "newResult.Total.Should(Be.EqualTo(1))");
			newResult.AssignableIds.Contains(expectedAssignableId.ToString()).Should(Be.False, "newResult.AssignableIds.Contains(expectedAssignableId.ToString()).Should(Be.False)");

			var lastResult = queryRunner.Run(new QueryData
			{
				Query = "zagzag",
				TeamIds = new[] {anotherTargetTeamId}
			});
			lastResult.Total.Should(Be.EqualTo(1), "lastResult.Total.Should(Be.EqualTo(1))");
			lastResult.AssignableIds.Contains(expectedAssignableId.ToString()).Should(Be.True, "lastResult.AssignableIds.Contains(expectedAssignableId.ToString()).Should(Be.True)");
		}

		[Test]
		public void SearchUserStoryByTeamAfterRemoveAssignableSquad()
		{
			var indexer = GetInstance<IEntityIndexer>();
			const int targetTeamId = 1;
			const int expectedAssignableId = 1;
			const int expectedAssignableSquadId = 1;
			const int userStoryTypeId = QueryEntityTypeProvider.USERSTORY_TYPE_ID;
			var target = new AssignableDTO
			{
				ID = expectedAssignableId,
				Name = "zagzag",
				Description = "",
				EntityTypeID = userStoryTypeId,
				SquadID = targetTeamId
			};
			indexer.AddAssignableIndex(target);
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = 2,
				Name = "zagzag",
				Description = "",
				EntityTypeID = userStoryTypeId,
				SquadID = targetTeamId
			});
			var targetAssignableSquad = new AssignableSquadDTO
			{
				AssignableSquadID = expectedAssignableSquadId,
				AssignableID = expectedAssignableId,
				SquadID = targetTeamId
			};
			indexer.AddAssignableSquadIndex(targetAssignableSquad);

			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "zagzag",
				TeamIds = new[] { targetTeamId }
			});
			result.Total.Should(Be.EqualTo(2), "result.Total.Should(Be.EqualTo(2))");
			result.AssignableIds.Contains(expectedAssignableId.ToString()).Should(Be.True, "result.AssignableIds.Contains(expectedAssignableId.ToString()).Should(Be.True)");

			indexer.RemoveAssignableSquadIndex(targetAssignableSquad);

			var newResult = queryRunner.Run(new QueryData
			{
				Query = "zagzag",
				TeamIds = new[] { targetTeamId }
			});
			newResult.Total.Should(Be.EqualTo(1), "newResult.Total.Should(Be.EqualTo(1))");
			newResult.AssignableIds.Contains(expectedAssignableId.ToString()).Should(Be.False, "newResult.AssignableIds.Contains(expectedAssignableId.ToString()).Should(Be.False)");
		}

		[Test]
		public void SearchUserStoryByMultipleTeamsAfterChange()
		{
			var indexer = GetInstance<IEntityIndexer>();
			const int targetTeamId = 1;
			const int anotherTargetTeamId = 2;
			const int updatedTeamId = 3;
			const int expectedAssignableId = 1;
			const int userStoryTypeId = QueryEntityTypeProvider.USERSTORY_TYPE_ID;
			var target = new AssignableDTO
			{
				ID = expectedAssignableId,
				Name = "zagzag",
				Description = "",
				EntityTypeID = userStoryTypeId,
				SquadID = targetTeamId
			};
			indexer.AddAssignableIndex(target);
			var targetAssignableSquad  = new AssignableSquadDTO
			{
				AssignableSquadID = 1,
				AssignableID = expectedAssignableId,
				SquadID = targetTeamId
			};
			indexer.AddAssignableSquadIndex(targetAssignableSquad);
			target.SquadID = targetTeamId;
			indexer.UpdateAssignableIndex(target, new[] { AssignableField.SquadID }, false);
			indexer.AddAssignableSquadIndex(new AssignableSquadDTO
			{
				AssignableSquadID = 2,
				AssignableID = expectedAssignableId,
				SquadID = anotherTargetTeamId
			});
			target.SquadID = anotherTargetTeamId;
			indexer.UpdateAssignableIndex(target, new[] { AssignableField.SquadID }, false);

			var queryRunner = GetInstance<QueryRunner>();
			var targetResult = queryRunner.Run(new QueryData
			{
				Query = "zagzag",
				TeamIds = new[] { targetTeamId }
			});
			targetResult.Total.Should(Be.EqualTo(1), "targetResult.Total.Should(Be.EqualTo(1))");
			targetResult.AssignableIds.Contains(expectedAssignableId.ToString()).Should(Be.True, "targetResult.AssignableIds.Contains(expectedAssignableId.ToString()).Should(Be.True)");

			var anotherResult = queryRunner.Run(new QueryData
			{
				Query = "zagzag",
				TeamIds = new[] { anotherTargetTeamId }
			});
			anotherResult.Total.Should(Be.EqualTo(1), "anotherResult.Total.Should(Be.EqualTo(1))");
			anotherResult.AssignableIds.Contains(expectedAssignableId.ToString()).Should(Be.True, "anotherResult.AssignableIds.Contains(expectedAssignableId.ToString()).Should(Be.True)");

			indexer.UpdateAssignableSquadIndex(new AssignableSquadDTO
			{
				AssignableSquadID = targetAssignableSquad.AssignableSquadID,
				AssignableID = targetAssignableSquad.AssignableID,
				SquadID = updatedTeamId
			}, targetAssignableSquad, new List<AssignableSquadField> { AssignableSquadField.SquadID });
			target.SquadID = updatedTeamId;
			indexer.UpdateAssignableIndex(target, new[] { AssignableField.SquadID }, isIndexing: false);

			var oldResult = queryRunner.Run(new QueryData
			{
				Query = "zagzag",
				TeamIds = new[] { targetTeamId }
			});
			oldResult.Total.Should(Be.EqualTo(0), "oldResult.Total.Should(Be.EqualTo(0))");
			oldResult.AssignableIds.Contains(expectedAssignableId.ToString()).Should(Be.False, "oldResult.AssignableIds.Contains(expectedAssignableId.ToString()).Should(Be.False)");
			var anotherStillResult = queryRunner.Run(new QueryData
			{
				Query = "zagzag",
				TeamIds = new[] { anotherTargetTeamId }
			});
			anotherStillResult.Total.Should(Be.EqualTo(1), "anotherStillResult.Total.Should(Be.EqualTo(1))");
			anotherStillResult.AssignableIds.Contains(expectedAssignableId.ToString()).Should(Be.True, "anotherStillResult.AssignableIds.Contains(expectedAssignableId.ToString()).Should(Be.True)");
			var updatedResult = queryRunner.Run(new QueryData
			{
				Query = "zagzag",
				TeamIds = new[] { updatedTeamId }
			});
			updatedResult.Total.Should(Be.EqualTo(1), "updatedResult.Total.Should(Be.EqualTo(1))");
			updatedResult.AssignableIds.Contains(expectedAssignableId.ToString()).Should(Be.True, "updatedResult.AssignableIds.Contains(expectedAssignableId.ToString()).Should(Be.True)");
		}

		[Test]
		public void SearchUserStoryByNoTeamAfterChangeAssignableSquad()
		{
			var indexer = GetInstance<IEntityIndexer>();
			const int targetTeamId = 1;
			const int anotherTargetTeamId = 2;
			const int expectedAssignableId = 1;
			const int userStoryTypeId = QueryEntityTypeProvider.USERSTORY_TYPE_ID;
			var target = new AssignableDTO
			{
				ID = expectedAssignableId,
				Name = "zagzag",
				Description = "",
				EntityTypeID = userStoryTypeId,
				SquadID = null
			};
			indexer.AddAssignableIndex(target);
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = 2,
				Name = "zagzag",
				Description = "",
				EntityTypeID = userStoryTypeId,
				SquadID = null
			});

			var queryRunner = GetInstance<QueryRunner>();

			var result = queryRunner.Run(new QueryData
			{
				Query = "zagzag",
				TeamIds = null,
				IncludeNoTeam = true
			});
			result.Total.Should(Be.EqualTo(2), "result.Total.Should(Be.EqualTo(2))");
			result.AssignableIds.Contains(expectedAssignableId.ToString()).Should(Be.True, "result.AssignableIds.Contains(expectedAssignableId.ToString()).Should(Be.True)");

			var targetAssignableSquad = new AssignableSquadDTO
			{
				AssignableSquadID = 1,
				AssignableID = expectedAssignableId,
				SquadID = targetTeamId
			};
			indexer.AddAssignableSquadIndex(targetAssignableSquad);

			var noTeamResult = queryRunner.Run(new QueryData
			{
				Query = "zagzag",
				TeamIds = null,
				IncludeNoTeam = true
			});
			noTeamResult.Total.Should(Be.EqualTo(1), "noTeamResult.Total.Should(Be.EqualTo(1))");
			noTeamResult.AssignableIds.Contains(expectedAssignableId.ToString()).Should(Be.False, "noTeamResult.AssignableIds.Contains(expectedAssignableId.ToString()).Should(Be.False)");

			var teamResult = queryRunner.Run(new QueryData
			{
				Query = "zagzag",
				TeamIds = new[] { targetTeamId },
				IncludeNoTeam = true
			});
			teamResult.Total.Should(Be.EqualTo(2), "teamResult.Total.Should(Be.EqualTo(2))");
			teamResult.AssignableIds.Contains(expectedAssignableId.ToString()).Should(Be.True, "teamResult.AssignableIds.Contains(expectedAssignableId.ToString()).Should(Be.True)");

			indexer.UpdateAssignableSquadIndex(new AssignableSquadDTO
			{
				AssignableSquadID = targetAssignableSquad.AssignableSquadID,
				AssignableID = targetAssignableSquad.AssignableID,
				SquadID = anotherTargetTeamId
			}, targetAssignableSquad, new List<AssignableSquadField> { AssignableSquadField.SquadID });

			var updatedNoTeamResult = queryRunner.Run(new QueryData
			{
				Query = "zagzag",
				TeamIds = null,
				IncludeNoTeam = true
			});
			updatedNoTeamResult.Total.Should(Be.EqualTo(1), "updatedNoTeamResult.Total.Should(Be.EqualTo(1))");
			updatedNoTeamResult.AssignableIds.Contains(expectedAssignableId.ToString()).Should(Be.False, "updatedNoTeamResult.AssignableIds.Contains(expectedAssignableId.ToString()).Should(Be.False)");

			var updatedTeamResult = queryRunner.Run(new QueryData
			{
				Query = "zagzag",
				TeamIds = new[] { targetTeamId }
			});
			updatedTeamResult.Total.Should(Be.EqualTo(0), "updatedTeamResult.Total.Should(Be.EqualTo(0))");
			updatedTeamResult.AssignableIds.Contains(expectedAssignableId.ToString()).Should(Be.False, "updatedTeamResult.AssignableIds.Contains(expectedAssignableId.ToString()).Should(Be.False)");

			var anotherTeamResult = queryRunner.Run(new QueryData
			{
				Query = "zagzag",
				TeamIds = new[] { anotherTargetTeamId },
				IncludeNoTeam = true
			});
			anotherTeamResult.Total.Should(Be.EqualTo(2), "anotherTeamResult.Total.Should(Be.EqualTo(2))");
			anotherTeamResult.AssignableIds.Contains(expectedAssignableId.ToString()).Should(Be.True, "anotherTeamResult.AssignableIds.Contains(expectedAssignableId.ToString()).Should(Be.True)");

			indexer.RemoveAssignableSquadIndex(new AssignableSquadDTO
			{
				AssignableSquadID = targetAssignableSquad.AssignableSquadID,
				AssignableID = targetAssignableSquad.AssignableID,
				SquadID = anotherTargetTeamId
			});

			var lastNoTeamResult = queryRunner.Run(new QueryData
			{
				Query = "zagzag",
				TeamIds = null,
				IncludeNoTeam = true
			});
			lastNoTeamResult.Total.Should(Be.EqualTo(2), "lastNoTeamResult.Total.Should(Be.EqualTo(2))");
			lastNoTeamResult.AssignableIds.Contains(expectedAssignableId.ToString()).Should(Be.True, "lastNoTeamResult.AssignableIds.Contains(expectedAssignableId.ToString()).Should(Be.True)");

			var lastTeamResult = queryRunner.Run(new QueryData
			{
				Query = "zagzag",
				TeamIds = new[] { anotherTargetTeamId }
			});
			lastTeamResult.Total.Should(Be.EqualTo(0), "lastTeamResult.Total.Should(Be.EqualTo(0))");
			lastTeamResult.AssignableIds.Contains(expectedAssignableId.ToString()).Should(Be.False, "lastTeamResult.AssignableIds.Contains(expectedAssignableId.ToString()).Should(Be.False)");
		}

		[Test]
		public void SearchUserStoryByTeamAfterProjectUpdateToNull()
		{
			var indexer = GetInstance<IEntityIndexer>();
			const int targetProjectId = 1;
			const int assignableIdWithNullProject = 5;
			var expectedAssignablesIds = new[] { 1, 4 };
			int userStoryTypeId = QueryEntityTypeProvider.USERSTORY_TYPE_ID;
			var target = new AssignableDTO
			{
				ID = expectedAssignablesIds[0],
				Name = "zagzag",
				Description = "",
				EntityTypeID = userStoryTypeId,
				ProjectID = targetProjectId
			};
			indexer.AddAssignableIndex(target);
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = 2,
				Name = "zagzag",
				Description = "",
				EntityTypeID = userStoryTypeId,
				ProjectID = 2
			});
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = 3,
				Name = "qqqqq",
				Description = "wwww",
				EntityTypeID = userStoryTypeId,
				ProjectID = targetProjectId
			});
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = expectedAssignablesIds[1],
				Name = "zagzag qwerty",
				Description = "",
				EntityTypeID = userStoryTypeId,
				ProjectID = targetProjectId
			});
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = assignableIdWithNullProject,
				Name = "zagzag qwerty blabla",
				Description = "",
				EntityTypeID = userStoryTypeId,
			});
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "zagzag",
				ProjectIds = new[] { targetProjectId }
			});
			result.Total.Should(Be.EqualTo(2), "result.Total.Should(Be.EqualTo(2))");
			result.AssignableIds.Should(Be.EquivalentTo(expectedAssignablesIds.Select(s => s.ToString()).ToArray()), "result.AssignableIds.Should(Be.EquivalentTo(expectedAssignablesIds.Select(s => s.ToString()).ToArray()))");
			target.ProjectID = null;
			indexer.UpdateAssignableIndex(target, new[] { AssignableField.ProjectID }, isIndexing: false);
			var newResult = queryRunner.Run(new QueryData
			{
				Query = "zagzag",
				ProjectIds = new[] { targetProjectId }
			});
			newResult.Total.Should(Be.EqualTo(1), "newResult.Total.Should(Be.EqualTo(1))");
			newResult.AssignableIds.Should(Be.EquivalentTo(expectedAssignablesIds.Skip(1).Select(s => s.ToString()).ToArray()), "newResult.AssignableIds.Should(Be.EquivalentTo(expectedAssignablesIds.Skip(1).Select(s => s.ToString()).ToArray()))");

			var lastResult = queryRunner.Run(new QueryData
			{
				Query = "zagzag",
				ProjectIds = null,
				IncludeNoProject = true
			});
			lastResult.Total.Should(Be.EqualTo(2), "lastResult.Total.Should(Be.EqualTo(2))");
			lastResult.AssignableIds.Should(Be.EquivalentTo(expectedAssignablesIds.Take(1).Concat(new[] { assignableIdWithNullProject }).Select(s => s.ToString()).ToArray()), "lastResult.AssignableIds.Should(Be.EquivalentTo(expectedAssignablesIds.Take(1).Concat(new[] { assignableIdWithNullProject }).Select(s => s.ToString()).ToArray()))");
		}

		[Test]
		public void SearchUserStoryByTeamAndProject()
		{
			var indexer = GetInstance<IEntityIndexer>();
			const int targetProjectId = 1;
			const int targetSquadId = 1;
			var expectedAssignablesIds = new[] { 1, 3, 4 };
			int userStoryTypeId = QueryEntityTypeProvider.USERSTORY_TYPE_ID;
			indexer.AddAssignableIndex(new AssignableDTO
				{
					ID = expectedAssignablesIds[0],
					Name = "zagzag",
					Description = "",
					EntityTypeID = userStoryTypeId,
					ProjectID = targetProjectId
				});
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = 2,
				Name = "zagzag",
				Description = "",
				EntityTypeID = userStoryTypeId,
				ProjectID = 2
			});
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = expectedAssignablesIds[1],
				Name = "qqqqq",
				Description = "zagzag",
				EntityTypeID = userStoryTypeId,
				SquadID = targetSquadId
			});
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = expectedAssignablesIds[2],
				Name = "zagzag qwerty",
				Description = "",
				EntityTypeID = userStoryTypeId,
				ProjectID = targetProjectId,
				SquadID = targetSquadId
			});
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "zagzag",
				ProjectIds = new[] { targetProjectId },
				TeamIds = new[]{targetSquadId}
			});
			result.Total.Should(Be.EqualTo(1), "result.Total.Should(Be.EqualTo(1))");
			result.AssignableIds.Should(Be.EquivalentTo(new[] { expectedAssignablesIds[2] }.Select(s => s.ToString()).ToArray()), "result.AssignableIds.Should(Be.EquivalentTo(new[] { expectedAssignablesIds[2] }.Select(s => s.ToString()).ToArray()))");
		}

		[Test]
		public void SearchCommentByTeamAndProject()
		{
			var indexer = GetInstance<IEntityIndexer>();
			const int targetProjectId = 1;
			const int targetSquadId = 1;
			var expectedAssignablesIds = new[] { 1, 3, 4 };
			var expectedCommentIds = new[] { 2, 4, 8 };
			int userStoryTypeId = QueryEntityTypeProvider.USERSTORY_TYPE_ID;
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = expectedAssignablesIds[0],
				Name = "zagzag",
				Description = "",
				EntityTypeID = userStoryTypeId,
				ProjectID = targetProjectId
			});
			indexer.AddCommentIndex(new CommentDTO
				{
					CommentID = expectedCommentIds[0],
					GeneralID = expectedAssignablesIds[0],
					Description = "qwerty ffff"
				});
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = 2,
				Name = "zagzag",
				Description = "",
				EntityTypeID = userStoryTypeId,
				ProjectID = 2
			});
			indexer.AddCommentIndex(new CommentDTO
			{
				CommentID = 2,
				GeneralID = 2,
				Description = "qwerty wert"
			});
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = expectedAssignablesIds[1],
				Name = "qqqqq",
				Description = "zagzag",
				EntityTypeID = userStoryTypeId,
				SquadID = targetSquadId
			});
			indexer.AddCommentIndex(new CommentDTO
			{
				CommentID = expectedCommentIds[1],
				GeneralID = expectedAssignablesIds[1],
				Description = "qwerty bla bla"
			});
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = expectedAssignablesIds[2],
				Name = "zagzag qwerty",
				Description = "",
				EntityTypeID = userStoryTypeId,
				ProjectID = targetProjectId,
				SquadID = targetSquadId
			});
			indexer.AddCommentIndex(new CommentDTO
			{
				CommentID = expectedCommentIds[2],
				GeneralID = expectedAssignablesIds[2],
				Description = "    qwerty"
			});
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "qwerty",
				ProjectIds = new[] { targetProjectId },
				TeamIds = new[] { targetSquadId },
			});
			result.Total.Should(Be.EqualTo(2), "result.Total.Should(Be.EqualTo(2))");
			result.AssignableIds.Should(Be.EquivalentTo(new[] { expectedAssignablesIds[2] }.Select(s => s.ToString()).ToArray()), "result.AssignableIds.Should(Be.EquivalentTo(new[] { expectedAssignablesIds[2] }.Select(s => s.ToString()).ToArray()))");
			result.CommentIds.Should(Be.EquivalentTo(new[] { expectedCommentIds[2] }.Select(s => s.ToString()).ToArray()), "result.CommentIds.Should(Be.EquivalentTo(new[] { expectedCommentIds[2] }.Select(s => s.ToString()).ToArray()))");
		}

		[Test]
		public void SearchCommentByMultipleTeams()
		{
			var indexer = GetInstance<IEntityIndexer>();
			const int targetSquadId = 1;
			const int anotherTargetSquadId = 2;
			const int expectedAssignableId = 1;
			const int anotherAssignableId = 2;
			const int expectedAssignableSquadId = 1;
			const int expectedCommentId = 1;
			const int anotherCommentId = 2;
			const int userStoryTypeId = QueryEntityTypeProvider.USERSTORY_TYPE_ID;
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = expectedAssignableId,
				Name = "zagzag",
				Description = "",
				EntityTypeID = userStoryTypeId,
				SquadID = targetSquadId
			});
			var targetAssignableSquad = new AssignableSquadDTO
			{
				AssignableSquadID = expectedAssignableSquadId,
				AssignableID = expectedAssignableId,
				SquadID = targetSquadId
			};
			indexer.AddAssignableSquadIndex(targetAssignableSquad);
			indexer.AddAssignableSquadIndex(new AssignableSquadDTO
			{
				AssignableSquadID = 2,
				AssignableID = expectedAssignableId,
				SquadID = anotherTargetSquadId
			});
			indexer.AddCommentIndex(new CommentDTO
			{
				CommentID = expectedCommentId,
				GeneralID = expectedAssignableId,
				Description = "qwerty ffff"
			});
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = anotherAssignableId,
				Name = "zagzag",
				Description = "",
				EntityTypeID = userStoryTypeId,
				SquadID = targetSquadId
			});
			indexer.AddCommentIndex(new CommentDTO
			{
				CommentID = anotherCommentId,
				GeneralID = anotherAssignableId,
				Description = "qwerty wert"
			});			

			var queryRunner = GetInstance<QueryRunner>();
			var targetResult = queryRunner.Run(new QueryData
			{
				Query = "qwerty",
				TeamIds = new[] { targetSquadId }
			});
			targetResult.Total.Should(Be.EqualTo(2), "targetResult.Total.Should(Be.EqualTo(2))");
			targetResult.CommentIds.Should(Be.EquivalentTo(new[] { expectedCommentId, anotherCommentId }.Select(s => s.ToString()).ToArray()), "targetResult.CommentIds.Should(Be.EquivalentTo(new[] { expectedCommentId, anotherCommentId }.Select(s => s.ToString()).ToArray()))");
			var anotherQueryResult = queryRunner.Run(new QueryData
			{
				Query = "qwerty",
				TeamIds = new[] { anotherTargetSquadId }
			});
			anotherQueryResult.Total.Should(Be.EqualTo(1), "anotherQueryResult.Total.Should(Be.EqualTo(1))");
			anotherQueryResult.CommentIds.Should(Be.EquivalentTo(new[] { expectedCommentId }.Select(s => s.ToString()).ToArray()), "anotherQueryResult.CommentIds.Should(Be.EquivalentTo(new[] { expectedCommentId }.Select(s => s.ToString()).ToArray()))");
		}

		[Test]
		public void ShouldNotSearchCommentIfEntityStateSelected()
		{
			var indexer = GetInstance<IEntityIndexer>();
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = 1,
				Name = "test",
				Description = "",
				EntityTypeID = 4,
				EntityStateID = 1,
				ProjectID = 1
			});
			indexer.AddCommentIndex(new CommentDTO
			{
				CommentID = 10,
				GeneralID = 1,
				Description = "test ffff"
			});

			var queryRunner = GetInstance<QueryRunner>();
			var entityWithComments = queryRunner.Run(new QueryData
			{
				Query = "test",
				ProjectIds = new[] { 1 }
			});
			entityWithComments.Total.Should(Be.EqualTo(2), "entityWithComments.Total.Should(Be.EqualTo(2))");
			entityWithComments.AssignableIds.Should(Be.EquivalentTo(new[]{1}.Select(x => x.ToString()).ToArray()), "entityWithComments.AssignableIds.Should(Be.EquivalentTo(new[]{{i}}.Select(x => x.ToString()).ToArray()))");
			entityWithComments.CommentIds.Should(Be.EquivalentTo(new[] { 10 }.Select(x => x.ToString()).ToArray()), "entityWithComments.CommentIds.Should(Be.EquivalentTo(new[] { 10 }.Select(x => x.ToString()).ToArray()))");
			var entityWithoutComments = queryRunner.Run(new QueryData
			{
				Query = "test",
				EntityStateIds = new[]{1},
				ProjectIds = new[] { 1 }
			});
			entityWithoutComments.Total.Should(Be.EqualTo(1), "entityWithoutComments.Total.Should(Be.EqualTo(1))");
			entityWithoutComments.AssignableIds.Should(Be.EquivalentTo(new[] { 1 }.Select(x => x.ToString()).ToArray()), "entityWithoutComments.AssignableIds.Should(Be.EquivalentTo(new[] { 1 }.Select(x => x.ToString()).ToArray()))");
			entityWithoutComments.CommentIds.Should(Be.Empty, "entityWithoutComments.CommentIds.Should(Be.Empty)");
		}

		[Test]
		public void SearchUserStoryByTwoWordsWithWildcards()
		{
			var indexer = GetInstance<IEntityIndexer>();
			int userStoryTypeId = QueryEntityTypeProvider.USERSTORY_TYPE_ID;
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = 1,
				Name = "qwerty asdfg",
				EntityTypeID = userStoryTypeId,
				ParentProjectID = 1
			});
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = 2,
				Name = "qwert asdf",
				EntityTypeID = userStoryTypeId,
				ParentProjectID = 1
			});
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = 3,
				Name = "qwert asd",
				EntityTypeID = userStoryTypeId,
				ParentProjectID = 1
			});
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "qwert asdf",
				ProjectIds = new[] { 1 }
			});
			result.Total.Should(Be.EqualTo(2), "result.Total.Should(Be.EqualTo(2))");
			result.AssignableIds.Should(Be.EquivalentTo(new[]{1,2}.Select(x => x.ToString())), "result.AssignableIds.Should(Be.EquivalentTo(new[]{1,2}.Select(x => x.ToString())))");
		}

		[Test]
		public void SearchUserStoryByTwoWordsWithWildcardsIfNotAllTargetWordsExistsInCommentIndex()
		{
			var indexer = GetInstance<IEntityIndexer>();
			int userStoryTypeId = QueryEntityTypeProvider.USERSTORY_TYPE_ID;
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = 1,
				Name = "qwerty asdfg",
				EntityTypeID = userStoryTypeId,
				ParentProjectID = 1
			});
			indexer.AddCommentIndex(new CommentDTO
			{
				CommentID = 1,
				GeneralID = 1,
				Description = "qwerty",
			});
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = 2,
				Name = "qwert asdf",
				EntityTypeID = userStoryTypeId,
				ParentProjectID = 1
			});
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = 3,
				Name = "qwert asd",
				EntityTypeID = userStoryTypeId,
				ParentProjectID = 1
			});
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "qwert asdf",
				ProjectIds = new[] { 1 }
			});
			result.Total.Should(Be.EqualTo(2), "result.Total.Should(Be.EqualTo(2))");
			result.AssignableIds.Should(Be.EquivalentTo(new[] { 1, 2 }.Select(x => x.ToString())), "result.AssignableIds.Should(Be.EquivalentTo(new[] { 1, 2 }.Select(x => x.ToString())))");
		}

		[Test]
		public void SearchTestCase()
		{
			var indexer = GetInstance<IEntityIndexer>();
			int testCaseTypeId = QueryEntityTypeProvider.TESTCASE_TYPE_ID;
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = 1,
				Name = "qwerty asdfg",
				EntityTypeID = testCaseTypeId,
				ParentProjectID = 1
			});
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "qwerty",
				ProjectIds = new[]{1},
				EntityTypeId = testCaseTypeId,
				IncludeNoTeam = true
			});
			result.Total.Should(Be.EqualTo(1), "result.Total.Should(Be.EqualTo(1))");
			result.GeneralIds.Should(Be.EquivalentTo(new[] { 1 }.Select(x => x.ToString())), "result.GeneralIds.Should(Be.EquivalentTo(new[] { 1 }.Select(x => x.ToString())))");
		}

		[Test]
		public void SearchCommentForTestCase()
		{
			var indexer = GetInstance<IEntityIndexer>();
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = 1,
				Name = "qwerty asdfg",
				EntityTypeID = QueryEntityTypeProvider.TESTCASE_TYPE_ID,
				ParentProjectID = 1
			});
			indexer.AddCommentIndex(new CommentDTO
			{
				CommentID = 1,
				GeneralID = 1,
				Description = "qwerty zagzag"
			});

			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "zagzag",
				ProjectIds = new[] { 1 },
				EntityTypeId = QueryEntityTypeProvider.COMMENT_TYPE_ID,
				IncludeNoTeam = true
			});
			result.Total.Should(Be.EqualTo(1), "result.Total.Should(Be.EqualTo(1))");
			result.CommentIds.Should(Be.EquivalentTo(new[] { 1 }.Select(x => x.ToString())), "result.CommentIds.Should(Be.EquivalentTo(new[] { 1 }.Select(x => x.ToString())))");
		}

		[Test]
		public void SearchCommentForTestCaseForProjectNotFromContext()
		{
			var indexer = GetInstance<IEntityIndexer>();
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = 1,
				Name = "qwerty asdfg",
				EntityTypeID = QueryEntityTypeProvider.TESTCASE_TYPE_ID,
				ParentProjectID = 1
			});
			indexer.AddCommentIndex(new CommentDTO
			{
				CommentID = 1,
				GeneralID = 1,
				Description = "qwerty zagzag"
			});

			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "zagzag",
				ProjectIds = new[] { 2 },
				EntityTypeId = QueryEntityTypeProvider.COMMENT_TYPE_ID,
				IncludeNoTeam = true
			});
			result.Total.Should(Be.EqualTo(0), "result.Total.Should(Be.EqualTo(0))");
		}

		[Test]
		public void SearchUserStory()
		{
			var indexer = GetInstance<IEntityIndexer>();
			int userStoryTypeId = QueryEntityTypeProvider.USERSTORY_TYPE_ID;
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = 1,
				Name = "qwerty asdfg",
				EntityTypeID = userStoryTypeId,
				ProjectID = 1
			});
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "qwerty",
				ProjectIds = new[] { 1 },
				EntityTypeId = userStoryTypeId,
				IncludeNoTeam = true
			});
			result.Total.Should(Be.EqualTo(1), "result.Total.Should(Be.EqualTo(1))");
			result.AssignableIds.Should(Be.EquivalentTo(new[] { 1 }.Select(x => x.ToString())), "result.AssignableIds.Should(Be.EquivalentTo(new[] { 1 }.Select(x => x.ToString())))");
		}

		[Test]
		public void SearchNoSquadAndSquadEntities()
		{
			var indexer = GetInstance<IEntityIndexer>();

			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = 1,
				Name = "qwerty",
				EntityTypeID = QueryEntityTypeProvider.RELEASE_TYPE_ID,
				ParentProjectID = 1
			});

			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = 2,
				Name = "qwerty",
				EntityTypeID = QueryEntityTypeProvider.USERSTORY_TYPE_ID,
				ProjectID = 1
			});

			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "qwerty",
				ProjectIds = new[]{1}
			});
			result.Total.Should(Be.EqualTo(2), "result.Total.Should(Be.EqualTo(2))");
			result.GeneralIds.Should(Be.EquivalentTo(new[] { 1 }.Select(x => x.ToString())), "result.GeneralIds.Should(Be.EquivalentTo(new[] { 1 }.Select(x => x.ToString())))");
			result.AssignableIds.Should(Be.EquivalentTo(new[] { 2 }.Select(x => x.ToString())), "result.AssignableIds.Should(Be.EquivalentTo(new[] { 2 }.Select(x => x.ToString())))");
		}

		[Test]
		public void SearchNoSquadAndSquadEntitiesByNoSquad()
		{
			var indexer = GetInstance<IEntityIndexer>();

			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = 1,
				Name = "qwerty",
				EntityTypeID = QueryEntityTypeProvider.RELEASE_TYPE_ID,
				ParentProjectID = 1
			});

			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = 2,
				Name = "qwerty",
				EntityTypeID = QueryEntityTypeProvider.USERSTORY_TYPE_ID,
				ProjectID = 1
			});

			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "qwerty",
				ProjectIds = new[]{1}
			});
			result.Total.Should(Be.EqualTo(2), "result.Total.Should(Be.EqualTo(2))");
			result.GeneralIds.Should(Be.EquivalentTo(new[] { 1 }.Select(x => x.ToString())), "result.GeneralIds.Should(Be.EquivalentTo(new[] { 1 }.Select(x => x.ToString())))");
			result.AssignableIds.Should(Be.EquivalentTo(new[] { 2 }.Select(x => x.ToString())), "result.AssignableIds.Should(Be.EquivalentTo(new[] { 2 }.Select(x => x.ToString())))");
		}

		[Test]
		public void SearchNothingIfNoContext()
		{
			var indexer = GetInstance<IEntityIndexer>();
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = 1,
				Name = "qwerty",
				EntityTypeID = QueryEntityTypeProvider.RELEASE_TYPE_ID,
				ParentProjectID = 1
			});

			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = 2,
				Name = "qwerty",
				EntityTypeID = QueryEntityTypeProvider.USERSTORY_TYPE_ID,
				ProjectID = 1
			});

			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "qwerty",
			});
			result.Total.Should(Be.EqualTo(0), "result.Total.Should(Be.EqualTo(0))");
		}

		[Test]
		public void SearchTestCasesWithoutProject()
		{
			var indexer = GetInstance<IEntityIndexer>();
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = 1,
				Name = "qwerty",
				EntityTypeID = QueryEntityTypeProvider.TESTCASE_TYPE_ID,
			});
			
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "qwerty",
				IncludeNoProject = true
			});
			result.Total.Should(Be.EqualTo(1), "result.Total.Should(Be.EqualTo(1))");
			result.GeneralIds.Should(Be.EquivalentTo(new[] { 1 }.Select(x => x.ToString())), "result.GeneralIds.Should(Be.EquivalentTo(new[] { 1 }.Select(x => x.ToString())))");
		}

		[Test]
		public void SearchUserStoriesInProjectsReachableThroughTeam()
		{
			var indexer = GetInstance<IEntityIndexer>();
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = 1,
				Name = "qwerty xxxx",
				EntityTypeID = QueryEntityTypeProvider.USERSTORY_TYPE_ID,
				ProjectID = 1,
				SquadID = 1
			});
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = 2,
				Name = "qwerty gggg",
				EntityTypeID = QueryEntityTypeProvider.USERSTORY_TYPE_ID,
				ProjectID = 1,
			});
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = 3,
				Name = "qwerty yyyy",
				EntityTypeID = QueryEntityTypeProvider.USERSTORY_TYPE_ID,
				ProjectID = 2,
				SquadID = 1
			});
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = 4,
				Name = "qwerty zzzz",
				EntityTypeID = QueryEntityTypeProvider.USERSTORY_TYPE_ID,
				ProjectID = 3,
				SquadID = 1
			});
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = 5,
				Name = "qwerty qqqq",
				EntityTypeID = QueryEntityTypeProvider.USERSTORY_TYPE_ID,
				ProjectID = 3,
			});
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "qwerty",
				ProjectIds = new[] { 1,2},
				TeamIds = new[]{1},
				TeamProjectRelations = new[]{new TeamProjectRelation
					{
						ProjectIds = new[]{3},
						TeamId = 1
					} },
				EntityTypeId = QueryEntityTypeProvider.USERSTORY_TYPE_ID,
				IncludeNoTeam = true
			});
			result.Total.Should(Be.EqualTo(4), "result.Total.Should(Be.EqualTo(4))");
			result.AssignableIds.Should(Be.EquivalentTo(new[] { 1,2,3,4 }.Select(x => x.ToString())), "result.AssignableIds.Should(Be.EquivalentTo(new[] { 1,2,3,4 }.Select(x => x.ToString())))");
		}

		[Test]
		public void SearchUserStoriesInProjectsReachableThroughTeamComplexCase()
		{
			var indexer = GetInstance<IEntityIndexer>();
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = 1,
				Name = "qwerty xxxx",
				EntityTypeID = QueryEntityTypeProvider.USERSTORY_TYPE_ID,
				ProjectID = 1,
				SquadID = 1
			});
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = 2,
				Name = "qwerty gggg",
				EntityTypeID = QueryEntityTypeProvider.USERSTORY_TYPE_ID,
				ProjectID = 1,
			});
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = 3,
				Name = "qwerty yyyy",
				EntityTypeID = QueryEntityTypeProvider.USERSTORY_TYPE_ID,
				ProjectID = 2,
				SquadID = 1
			});
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = 4,
				Name = "qwerty zzzz",
				EntityTypeID = QueryEntityTypeProvider.USERSTORY_TYPE_ID,
				ProjectID = 3,
				SquadID = 1
			});
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = 5,
				Name = "qwerty qqqq",
				EntityTypeID = QueryEntityTypeProvider.USERSTORY_TYPE_ID,
				ProjectID = 3,
			});



			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = 6,
				Name = "qwerty xxxx",
				EntityTypeID = QueryEntityTypeProvider.USERSTORY_TYPE_ID,
				ProjectID = 1,
				SquadID = 2
			});
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = 7,
				Name = "qwerty gggg",
				EntityTypeID = QueryEntityTypeProvider.USERSTORY_TYPE_ID,
				ProjectID = 1,
			});
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = 8,
				Name = "qwerty yyyy",
				EntityTypeID = QueryEntityTypeProvider.USERSTORY_TYPE_ID,
				ProjectID = 2,
				SquadID = 2
			});
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = 9,
				Name = "qwerty zzzz",
				EntityTypeID = QueryEntityTypeProvider.USERSTORY_TYPE_ID,
				ProjectID = 3,
				SquadID = 2
			});
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = 10,
				Name = "qwerty qqqq",
				EntityTypeID = QueryEntityTypeProvider.USERSTORY_TYPE_ID,
				ProjectID = 3,
			});
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "qwerty",
				ProjectIds = new[] { 1, 2 },
				TeamIds = new[] { 1 },
				TeamProjectRelations = new[]
				{
					new TeamProjectRelation
					{
						ProjectIds = new[]{3},
						TeamId = 1
					},
					new TeamProjectRelation
					{
						ProjectIds = new[]{3},
						TeamId = 2
					}
				},
				EntityTypeId = QueryEntityTypeProvider.USERSTORY_TYPE_ID,
				IncludeNoTeam = true
			});
			result.Total.Should(Be.EqualTo(6), "result.Total.Should(Be.EqualTo(6))");
			result.AssignableIds.Should(Be.EquivalentTo(new[] { 1, 2, 3, 4, 7, 9 }.Select(x => x.ToString())), "result.AssignableIds.Should(Be.EquivalentTo(new[] { 1, 2, 3, 4, 7, 9 }.Select(x => x.ToString())))");
		}

		[Test]
		public void SearchCommentInProjectsReachableThroughTeam()
		{
			var indexer = GetInstance<IEntityIndexer>();
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = 1,
				Name = "qwerty xxxx",
				EntityTypeID = QueryEntityTypeProvider.USERSTORY_TYPE_ID,
				ProjectID = 1,
				SquadID = 1
			});
			indexer.AddCommentIndex(new CommentDTO
				{
					ID = 1,
					GeneralID = 1,
					Description = "qwerty xxx"
				});
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = 2,
				Name = "qwerty gggg",
				EntityTypeID = QueryEntityTypeProvider.USERSTORY_TYPE_ID,
				ProjectID = 1,
			});
			indexer.AddCommentIndex(new CommentDTO
			{
				ID = 2,
				GeneralID = 2,
				Description = "qwerty ggg"
			});
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = 3,
				Name = "qwerty yyyy",
				EntityTypeID = QueryEntityTypeProvider.USERSTORY_TYPE_ID,
				ProjectID = 2,
				SquadID = 1
			});
			indexer.AddCommentIndex(new CommentDTO
			{
				ID = 3,
				GeneralID = 3,
				Description = "qwerty yyyy"
			});
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = 4,
				Name = "qwerty zzzz",
				EntityTypeID = QueryEntityTypeProvider.USERSTORY_TYPE_ID,
				ProjectID = 3,
				SquadID = 1
			});
			indexer.AddCommentIndex(new CommentDTO
			{
				ID = 4,
				GeneralID = 4,
				Description = "qwerty zzzz"
			});
			indexer.AddCommentIndex(new CommentDTO
			{
				ID = 7,
				GeneralID = 4,
				Description = "qwerty mmmm"
			});
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = 5,
				Name = "qwerty qqqq",
				EntityTypeID = QueryEntityTypeProvider.USERSTORY_TYPE_ID,
				ProjectID = 3,
			});
			indexer.AddCommentIndex(new CommentDTO
			{
				ID = 5,
				GeneralID = 5,
				Description = "qwerty qqqq"
			});
			indexer.AddCommentIndex(new CommentDTO
			{
				ID = 6,
				GeneralID = 5,
				Description = "qwerty bbbb"
			});
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "qwerty",
				ProjectIds = new[] { 1, 2 },
				TeamIds = new[] { 1 },
				TeamProjectRelations = new[]{new TeamProjectRelation
					{
						ProjectIds = new[]{3},
						TeamId = 1
					} },
				EntityTypeId = QueryEntityTypeProvider.COMMENT_TYPE_ID,
				IncludeNoTeam = true
			});
			result.Total.Should(Be.EqualTo(5), "result.Total.Should(Be.EqualTo(5))");
			result.AssignableIds.Should(Be.Empty, "result.AssignableIds.Should(Be.Empty)");
			result.CommentIds.Should(Be.EquivalentTo(new[] { 1, 2, 3, 4, 7 }.Select(x => x.ToString())), "result.CommentIds.Should(Be.EquivalentTo(new[] { 1, 2, 3, 4, 7 }.Select(x => x.ToString())))");
		}
		[Test]
		public void SearchUserStoryByNameWithDigitAfterDelete()
		{
			var indexer = GetInstance<IEntityIndexer>();
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = 1,
				Name = "US2.2",
				Description = string.Empty,
				EntityTypeID = 4,
				ParentProjectID = 1
			});
			indexer.OptimizeGeneralIndex();
			indexer.RemoveGeneralIndex(new GeneralDTO
				{
					ID = 1,
					EntityTypeID = 4
				});
			indexer.OptimizeGeneralIndex();
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "US2.2",
				ProjectIds = new[] { 1 }
			});
			result.Total.Should(Be.EqualTo(0), "result.Total.Should(Be.EqualTo(0))");
		}

		[Test]
		public void SearchUserStoryByNameWithDigits_()
		{
			var indexer = GetInstance<IEntityIndexer>();
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = 1,
				Name = "zagzag111",
				Description = string.Empty,
				EntityTypeID = 4,
				ParentProjectID = 1
			});
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = 2,
				Name = "zagzag",
				Description = string.Empty,
				EntityTypeID = 4,
				ParentProjectID = 1
			});
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "zagzag111",
				ProjectIds = new[] { 1 }
			});
			result.Total.Should(Be.EqualTo(1), "result.Total.Should(Be.EqualTo(1))");
			result.AssignableIds.Should(Be.EquivalentTo(new[] { "1" }), "result.AssignableIds.Should(Be.EquivalentTo(new[] { \"1\" }))");
		}

		[Test]
		public void SearchCommentByNameWithDigits()
		{
			var indexer = GetInstance<IEntityIndexer>();
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = 1,
				Name = "zagzag",
				Description = string.Empty,
				EntityTypeID = QueryEntityTypeProvider.USERSTORY_TYPE_ID,
				ParentProjectID = 1
			});
			indexer.AddCommentIndex(new CommentDTO{
				GeneralID = 1,
				CommentID = 1,
				Description = "zagzag111"
			});
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "zagzag111",
				ProjectIds = new[] { 1 }
			});
			result.Total.Should(Be.EqualTo(1), "result.Total.Should(Be.EqualTo(1))");
			result.CommentIds.Should(Be.EquivalentTo(new[] { "1" }), "result.CommentIds.Should(Be.EquivalentTo(new[] { \"1\" }))");
		}

		[Test]
		public void SearchUserStoryByNameWithSingleDigit()
		{
			var indexer = GetInstance<IEntityIndexer>();
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = 1,
				Name = "UserStory1",
				Description = string.Empty,
				EntityTypeID = 4,
				ParentProjectID = 1
			});
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = 2,
				Name = "zagzag",
				Description = string.Empty,
				EntityTypeID = 4,
				ParentProjectID = 1
			});
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "story1",
				ProjectIds = new[] { 1 }
			});
			result.Total.Should(Be.EqualTo(1), "result.Total.Should(Be.EqualTo(1))");
			result.AssignableIds.Should(Be.EquivalentTo(new[] { "1" }), "result.AssignableIds.Should(Be.EquivalentTo(new[] { \"1\" }))");
		}

		[Test]
		public void SearchUserStoryByNameWithDigitAndSpecSymbols()
		{
			var indexer = GetInstance<IEntityIndexer>();
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = 1,
				Name = "US1.2",
				Description = string.Empty,
				EntityTypeID = 4,
				ParentProjectID = 1
			});
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = 2,
				Name = "US2.1",
				Description = string.Empty,
				EntityTypeID = 4,
				ParentProjectID = 1
			});
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = 3,
				Name = "US2.2",
				Description = string.Empty,
				EntityTypeID = 4,
				ParentProjectID = 1
			});
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = 4,
				Name = "US2.3",
				Description = string.Empty,
				EntityTypeID = 4,
				ParentProjectID = 1
			});
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "US2.2",
				ProjectIds = new[] { 1 }
			});
			result.Total.Should(Be.EqualTo(1), "result.Total.Should(Be.EqualTo(1))");
			result.AssignableIds.Should(Be.EquivalentTo(new[] { "3" }), "result.AssignableIds.Should(Be.EquivalentTo(new[] { \"3\" }))");
		}

		[Test]
		public void SearchUserStoryByNameWithDigitAfterUpdate()
		{
			var indexer = GetInstance<IEntityIndexer>();
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = 1,
				Name = "US",
				Description = string.Empty,
				EntityTypeID = 4,
				ParentProjectID = 1
			});
			indexer.UpdateGeneralIndex(new GeneralDTO
				{
					ID = 1,
					Name = "US2.2",
					EntityTypeID = 4
				}, new[] {GeneralField.Name});
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "US2.2",
				ProjectIds = new[] { 1 }
			});
			result.Total.Should(Be.EqualTo(1), "result.Total.Should(Be.EqualTo(1))");
			result.AssignableIds.Should(Be.EquivalentTo(new[] { "1" }), "result.AssignableIds.Should(Be.EquivalentTo(new[] { \"1\" }))");
		}

		[Test]
		public void SearchCommentByNameWithDigitAfterDelete()
		{
			var indexer = GetInstance<IEntityIndexer>();
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = 1,
				Name = "US2.2",
				Description = string.Empty,
				EntityTypeID = 4,
				ParentProjectID = 1
			});
			indexer.AddCommentIndex(new CommentDTO
				{
				   ID = 1,
				   GeneralID = 1,
				   Description = "1234"
				});
			indexer.OptimizeGeneralIndex();
			indexer.OptimizeCommentIndex();
			indexer.RemoveCommentIndex(new CommentDTO
			{
				CommentID = 1,
				GeneralID = 1
			});
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "1234",
				ProjectIds = new[] { 1 },
				EntityTypeId = QueryEntityTypeProvider.COMMENT_TYPE_ID
			});
			result.Total.Should(Be.EqualTo(0), "result.Total.Should(Be.EqualTo(0))");
		}

		[Test]
		public void SearchUserStoryWithDigitsOnly()
		{
			var indexer = GetInstance<IEntityIndexer>();
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = 1,
				Name = "111",
				Description = string.Empty,
				EntityTypeID = QueryEntityTypeProvider.USERSTORY_TYPE_ID,
				ParentProjectID = 1
			});
			indexer.OptimizeGeneralIndex();
			indexer.RemoveGeneralIndex(new GeneralDTO
			{
				GeneralID = 1,
				EntityTypeID = QueryEntityTypeProvider.USERSTORY_TYPE_ID
			});
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "111",
				ProjectIds = new[] { 1 },
				EntityTypeId = QueryEntityTypeProvider.USERSTORY_TYPE_ID
			});
			result.Total.Should(Be.EqualTo(0), "result.Total.Should(Be.EqualTo(0))");
		}

		[Test]
		public void SearchUserStoryByComment()
		{
			var indexer = GetInstance<IEntityIndexer>();
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = 1,
				Name = "Us",
				Description = string.Empty,
				EntityTypeID = QueryEntityTypeProvider.USERSTORY_TYPE_ID,
				ParentProjectID = 1
			});
			indexer.AddCommentIndex(new CommentDTO
				{
					ID = 1,
					GeneralID = 1,
					Description = "aaaa"
				});
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "aaa",
				ProjectIds = new[] { 1 },
				EntityTypeId = QueryEntityTypeProvider.COMMENT_TYPE_ID
			});
			result.Total.Should(Be.EqualTo(1), "result.Total.Should(Be.EqualTo(1))");
			result.CommentIds.Should(Be.EqualTo(new[]{"1"}), "result.CommentIds.Should(Be.EqualTo(new[]{\"1\"}))");
			indexer.RemoveCommentIndex(new CommentDTO
				{
					ID = 1,
					GeneralID = 1
				});
			var result2 = queryRunner.Run(new QueryData
			{
				Query = "aaa",
				ProjectIds = new[] { 1 },
				EntityTypeId = QueryEntityTypeProvider.COMMENT_TYPE_ID
			});
			result2.Total.Should(Be.EqualTo(0), "result2.Total.Should(Be.EqualTo(0))");
		}
		[Test]
		public void SearchUserStoryWithDigitsWithSpacebar()
		{
			var indexer = GetInstance<IEntityIndexer>();
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = 1,
				Name = "Bug 1 2",
				Description = string.Empty,
				EntityTypeID = QueryEntityTypeProvider.USERSTORY_TYPE_ID,
				ParentProjectID = 1
			});
			indexer.OptimizeGeneralIndex();
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "Bug 1",
				ProjectIds = new[] { 1 },
				EntityTypeId = QueryEntityTypeProvider.USERSTORY_TYPE_ID
			});
			result.Total.Should(Be.EqualTo(1), "result.Total.Should(Be.EqualTo(1))");
		}

		[Test]
		public void SearchUserStoryWithDigitsWithSpacebar0()
		{
			var indexer = GetInstance<IEntityIndexer>();
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = 1,
				Name = "User23Story",
				Description = string.Empty,
				EntityTypeID = QueryEntityTypeProvider.USERSTORY_TYPE_ID,
				ParentProjectID = 1
			});
			indexer.OptimizeGeneralIndex();
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "User23Story",
				ProjectIds = new[] { 1 },
				EntityTypeId = QueryEntityTypeProvider.USERSTORY_TYPE_ID
			});
			result.Total.Should(Be.EqualTo(1), "result.Total.Should(Be.EqualTo(1))");
		}

		[Test]
		public void SearchUserStoryWithDigitsWithSpacebar1()
		{
			var indexer = GetInstance<IEntityIndexer>();
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = 1,
				Name = "User2 3 Story",
				Description = string.Empty,
				EntityTypeID = QueryEntityTypeProvider.USERSTORY_TYPE_ID,
				ParentProjectID = 1
			});
			indexer.OptimizeGeneralIndex();
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "user2",
				ProjectIds = new[] { 1 },
				EntityTypeId = QueryEntityTypeProvider.USERSTORY_TYPE_ID
			});
			result.Total.Should(Be.EqualTo(1), "result.Total.Should(Be.EqualTo(1))");
		}

		[Test]
		public void SearchUserStoryWithDigitsWithSpacebar2()
		{
			var indexer = GetInstance<IEntityIndexer>();
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = 1,
				Name = "User2 3 Story",
				Description = string.Empty,
				EntityTypeID = QueryEntityTypeProvider.USERSTORY_TYPE_ID,
				ParentProjectID = 1
			});
			indexer.OptimizeGeneralIndex();
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "3story",
				ProjectIds = new[] { 1 },
				EntityTypeId = QueryEntityTypeProvider.USERSTORY_TYPE_ID
			});
			result.Total.Should(Be.EqualTo(1), "result.Total.Should(Be.EqualTo(1))");
		}

		[Test]
		public void SearchUserStoryWithDigitsOnly0()
		{
			var indexer = GetInstance<IEntityIndexer>();
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = 1,
				Name = "11111",
				Description = string.Empty,
				EntityTypeID = QueryEntityTypeProvider.USERSTORY_TYPE_ID,
				ParentProjectID = 1
			});
			indexer.UpdateGeneralIndex(new GeneralDTO
				{
					ID = 1,
					Name = "11111",
					Description = "222",
					EntityTypeID = QueryEntityTypeProvider.USERSTORY_TYPE_ID
				}, new[] {GeneralField.Description});
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "222",
				ProjectIds = new[] { 1 },
				EntityTypeId = QueryEntityTypeProvider.USERSTORY_TYPE_ID
			});
			result.Total.Should(Be.EqualTo(1), "result.Total.Should(Be.EqualTo(1))");
		}

		[Test]
		public void SearchCommentAfterRemove()
		{
			var indexer = GetInstance<IEntityIndexer>();
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = 1,
				Name = "us0",
				Description = string.Empty,
				EntityTypeID = QueryEntityTypeProvider.USERSTORY_TYPE_ID,
				ParentProjectID = 1
			});
			indexer.AddCommentIndex(new CommentDTO
			{
				ID = 1,
				GeneralID = 1,
				Description = "test0",
			});
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = 2,
				Name = "us1",
				Description = string.Empty,
				EntityTypeID = QueryEntityTypeProvider.USERSTORY_TYPE_ID,
				ParentProjectID = 1
			});
			indexer.AddCommentIndex(new CommentDTO
			{
				ID = 2,
				GeneralID = 2,
				Description = "test1"
			});
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "test",
				ProjectIds = new[] { 1 },
				EntityTypeId = QueryEntityTypeProvider.COMMENT_TYPE_ID
			});
			result.Total.Should(Be.EqualTo(2), "result.Total.Should(Be.EqualTo(2))");
			result.CommentIds.Should(Be.EquivalentTo(new[] { "1", "2" }), "result.CommentIds.Should(Be.EquivalentTo(new[] { \"1\", \"2\" }))");
			indexer.RemoveCommentIndex(new CommentDTO
			{
				ID = 1,
				GeneralID = 1
			});
			var result2 = queryRunner.Run(new QueryData
			{
				Query = "test",
				ProjectIds = new[] { 1 },
				EntityTypeId = QueryEntityTypeProvider.COMMENT_TYPE_ID
			});
			result2.Total.Should(Be.EqualTo(1), "result2.Total.Should(Be.EqualTo(1))");
			result2.CommentIds.Should(Be.EquivalentTo(new[] { "2" }), "result2.CommentIds.Should(Be.EquivalentTo(new[] { \"2\" }))");
		}

		[Test]
		public void ShouldUpdateIndexOnEntityUpdate()
		{
			var indexer = GetInstance<IEntityIndexer>();
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = 1,
				Name = "testcase",
				Description = string.Empty,
				EntityTypeID = QueryEntityTypeProvider.TESTCASE_TYPE_ID,
				ParentProjectID = 1,
				CustomFieldsMetaInfo = new[] {new Field {FieldType = FieldTypeEnum.Text, Value = "cfvaluea"}}
			});
			indexer.UpdateGeneralIndex(new GeneralDTO
				{
					ID = 1,
					Name = "testcase",
					Description = string.Empty,
					EntityTypeID = QueryEntityTypeProvider.TESTCASE_TYPE_ID,
					EntityTypeName = "TestCase",
					ParentProjectID = 1,
					CustomFieldsMetaInfo = new[] {new Field {FieldType = FieldTypeEnum.Text, Value = "cfvalueb"}}
				}, new Collection<GeneralField> { GeneralField.CustomField1 });

			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "cfvalueb",
				ProjectIds = new[] { 1 },
				EntityTypeId = QueryEntityTypeProvider.TESTCASE_TYPE_ID
			});

			result.Total.Should(Be.EqualTo(1), "result.Total.Should(Be.EqualTo(1))");
			result.GeneralIds.Should(Be.EquivalentTo(new[] { "1" }), "result.GeneralIds.Should(Be.EquivalentTo(new[] { \"1\" }))");
		}

		[Test]
		public void SearchByCustomFieldWithDigits()
		{
			var indexer = GetInstance<IEntityIndexer>();
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = 1,
				Name = "Name",
				Description = string.Empty,
				EntityTypeID = QueryEntityTypeProvider.USERSTORY_TYPE_ID,
				CustomFieldsMetaInfo = new[] {new Field {FieldType = FieldTypeEnum.MultipleSelectionList, Value = "111,222"}},
				ParentProjectID = 1
			});
			
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "111",
				ProjectIds = new[] { 1 },
				EntityTypeId = QueryEntityTypeProvider.USERSTORY_TYPE_ID
			});
			result.Total.Should(Be.EqualTo(1), "result.Total.Should(Be.EqualTo(1))");
		}

		[Test]
		public void SearchCommentThroughUserStoryForProjectAndTeam()
		{
			var indexer = GetInstance<IEntityIndexer>();
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = 1,
				Name = "Name",
				Description = string.Empty,
				EntityTypeID = QueryEntityTypeProvider.USERSTORY_TYPE_ID,
				CustomFieldsMetaInfo = new[] { new Field { FieldType = FieldTypeEnum.MultipleSelectionList, Value = null } },
				ProjectID = 1,
				SquadID = 1
			});
			indexer.AddCommentIndex(new CommentDTO
				{
				   CommentID = 1,
				   GeneralID = 1,
				   Description = "zagzag"
				});
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "zag",
				ProjectIds = new[] { 1 },
				TeamIds = new[]{1},
				EntityTypeId = QueryEntityTypeProvider.COMMENT_TYPE_ID
			});
			result.Total.Should(Be.EqualTo(1), "result.Total.Should(Be.EqualTo(1))");
			result.CommentIds.Should(Be.EquivalentTo(new[]{"1"}), "result.CommentIds.Should(Be.EquivalentTo(new[]{\"1\"}))");
		}

		[Test]
		public void SearchCommentThroughUserStoryForNotMyProjectAndMyTeam()
		{
			var indexer = GetInstance<IEntityIndexer>();
			indexer.AddAssignableIndex(new AssignableDTO
			{
				ID = 1,
				Name = "Name",
				Description = string.Empty,
				EntityTypeID = QueryEntityTypeProvider.USERSTORY_TYPE_ID,
				ProjectID = 1,
				SquadID = 1
			});
			indexer.AddCommentIndex(new CommentDTO
			{
				CommentID = 1,
				GeneralID = 1,
				Description = "zagzag"
			});
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "zag",
				TeamIds = new[] { 1 },
				TeamProjectRelations = new[]
					{
						new TeamProjectRelation
							{
								TeamId = 1,
								ProjectIds = new[]{1}
							} 
					},
				EntityTypeId = QueryEntityTypeProvider.COMMENT_TYPE_ID
			});
			result.Total.Should(Be.EqualTo(1), "result.Total.Should(Be.EqualTo(1))");
			result.CommentIds.Should(Be.EquivalentTo(new[] { "1" }), "result.CommentIds.Should(Be.EquivalentTo(new[] { \"1\" }))");
		}

		[Test]
		public void SearchEpicByNameAndDescription()
		{
			var indexer = GetInstance<IEntityIndexer>();
			var id = 1;
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = id,
				Name = "zagzag",
				Description = "bla bla bla",
				EntityTypeID = 27,
				ParentProjectID = 1
			});
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "+zagzag +bla",
				ProjectIds = new[] { 1 }
			});
			result.Total.Should(Be.EqualTo(1), "result.Total.Should(Be.EqualTo(1))");
			result.AssignableIds.Should(Be.EquivalentTo(new[] { id.ToString() }), "result.AssignableIds.Should(Be.EquivalentTo(new[] { id.ToString() }))");
		}

		[Test]
		public void SearchReleaseByProjectId()
		{
			var indexer = GetInstance<IEntityIndexer>();
			var releaseId = 1;
			var projectId = 2;
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = releaseId,
				Name = "zagzag",
				Description = "bla bla bla",
				EntityTypeID = QueryEntityTypeProvider.RELEASE_TYPE_ID,
				ParentProjectID = 1
			});
			indexer.AddReleaseProjectIndex(new ReleaseProjectDTO
			{
				ID = 1,
				ReleaseID = releaseId,
				ProjectID = projectId
			});
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "+zagzag +bla",
				ProjectIds = new[] { projectId }
			});
			result.Total.Should(Be.EqualTo(1), "result.Total.Should(Be.EqualTo(1))");
			result.GeneralIds.Should(Be.EquivalentTo(new[] { releaseId.ToString() }), "result.GeneralIds.Should(Be.EquivalentTo(new[] { releaseId.ToString() }))");
		}

		[Test]
		public void SearchReleaseByProjectIdIfReleaseProjectChange()
		{
			var indexer = GetInstance<IEntityIndexer>();
			var releaseId = 1;
			var releaseMainProjectId = 1;
			var releaseOtherProjectId = 2;
			var newReleaseProjectId = 3;
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = releaseId,
				Name = "zagzag",
				Description = "bla bla bla",
				EntityTypeID = QueryEntityTypeProvider.RELEASE_TYPE_ID,
				ParentProjectID = releaseMainProjectId
			});
			indexer.AddReleaseProjectIndex(new ReleaseProjectDTO
			{
				ID = 1,
				ReleaseID = releaseId,
				ProjectID = releaseOtherProjectId,
			});
			indexer.UpdateGeneralIndex(new GeneralDTO
			{
				ID = releaseId,
				ParentProjectID = newReleaseProjectId,
				EntityTypeID = QueryEntityTypeProvider.RELEASE_TYPE_ID,
			}, new[] { GeneralField.ParentProjectID});
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "+zagzag +bla",
				ProjectIds = new[] { releaseOtherProjectId }
			});
			result.Total.Should(Be.EqualTo(1), "result.Total.Should(Be.EqualTo(1))");
			result.GeneralIds.Should(Be.EquivalentTo(new[] { releaseId.ToString() }), "result.GeneralIds.Should(Be.EquivalentTo(new[] { releaseId.ToString() }))");
		}

		[Test]
		public void SearchCommentsForCrossProjectReleases()
		{
			var indexer = GetInstance<IEntityIndexer>();
			var releaseId = 1;
			var releaseMainProjectId = 1;
			var releaseOtherProjectId = 2;
			var commentId = 3;
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = releaseId,
				Name = "zagzag",
				Description = "bla bla bla",
				EntityTypeID = QueryEntityTypeProvider.RELEASE_TYPE_ID,
				ParentProjectID = releaseMainProjectId
			});
			indexer.AddReleaseProjectIndex(new ReleaseProjectDTO
			{
				ID = releaseId,
				ReleaseID = releaseId,
				ProjectID = releaseOtherProjectId,
			});
			indexer.AddCommentIndex(new CommentDTO
			{
				CommentID = commentId,
				GeneralID = releaseId,
				Description = "qwerty ffff"
			});
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "+qwerty",
				ProjectIds = new[] { releaseOtherProjectId }
			});
			result.Total.Should(Be.EqualTo(1), "result.Total.Should(Be.EqualTo(1))");
			result.CommentIds.Should(Be.EquivalentTo(new[] { commentId.ToString() }), "result.CommentIds.Should(Be.EquivalentTo(new[] { commentId.ToString() }))");
		}

		[Test]
		public void SearchUserStoryByNameAndDescriptionWithHtmlFormatting()
		{
			var indexer = GetInstance<IEntityIndexer>();
			var id = 1;
			const string description = "&lt;table border=\"0\" style=\"width:100%\"&gt;&lt;tbody&gt;&lt;tr&gt;&lt;td style=\"width:50%\"&gt;&lt;strong&gt;Steps&lt;/strong&gt;&lt;/td&gt;&lt;td style=\"width:50%\"&gt;&lt;strong&gt;Success&lt;/strong&gt;&lt;/td&gt;&lt;/tr&gt;&lt;tr&gt;&lt;td style=\"vertical-align: top;\"&gt;1.   Log into the Box.&lt;br&gt;&lt;div&gt;         2.   Validate Database Connection set properly as per Technical Specification Document.&lt;br&gt;&lt;br&gt;        3.   Execute Database Script for PGL amd GPL&lt;br&gt;&lt;br&gt;Make sure the bothe script executes successfully&lt;/div&gt;&lt;p&gt;        4. Validate and make sure the Log file has no Errors.&lt;/p&gt;&lt;div&gt; &lt;/div&gt;&lt;/td&gt;&lt;td style=\"vertical-align: top;\"&gt;Expected Results&amp;#58;&lt;br&gt;&lt;br&gt;1.   Logged in to the Box Successfully.&lt;div&gt; &lt;/div&gt;&lt;div&gt;         2.   Data Connection values are properly set.&lt;/div&gt;&lt;p&gt;         3.   Job executed with no error.&lt;/p&gt;&lt;p&gt;         4.   No error in log files.&lt;/p&gt;&lt;div&gt; &lt;/div&gt;&lt;/td&gt;&lt;/tr&gt;&lt;/tbody&gt;&lt;/table&gt;";
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = id,
				Name = "zagzag",
				Description = description,
				EntityTypeID = 4,
				ParentProjectID = 1
			});
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "+executed +with +no +error",
				ProjectIds = new[] { 1 }
			});
			result.Total.Should(Be.EqualTo(1), "result.Total.Should(Be.EqualTo(1))");
			result.AssignableIds.Should(Be.EquivalentTo(new[] { id.ToString() }), "result.AssignableIds.Should(Be.EquivalentTo(new[] { id.ToString() }))");
		}

		[Test]
		public void SearchUserStoryByNameAndDescriptionWithHtmlFormattingNegativeCase()
		{
			var indexer = GetInstance<IEntityIndexer>();
			var id = 1;
			const string description = "&lt;table border=\"0\" style=\"width:100%\"&gt;&lt;tbody&gt;&lt;tr&gt;&lt;td style=\"width:50%\"&gt;&lt;strong&gt;Steps&lt;/strong&gt;&lt;/td&gt;&lt;td style=\"width:50%\"&gt;&lt;strong&gt;Success&lt;/strong&gt;&lt;/td&gt;&lt;/tr&gt;&lt;tr&gt;&lt;td style=\"vertical-align: top;\"&gt;1.   Log into the Box.&lt;br&gt;&lt;div&gt;         2.   Validate Database Connection set properly as per Technical Specification Document.&lt;br&gt;&lt;br&gt;        3.   Execute Database Script for PGL amd GPL&lt;br&gt;&lt;br&gt;Make sure the bothe script executes successfully&lt;/div&gt;&lt;p&gt;        4. Validate and make sure the Log file has no Errors.&lt;/p&gt;&lt;div&gt; &lt;/div&gt;&lt;/td&gt;&lt;td style=\"vertical-align: top;\"&gt;Expected Results&amp;#58;&lt;br&gt;&lt;br&gt;1.   Logged in to the Box Successfully.&lt;div&gt; &lt;/div&gt;&lt;div&gt;         2.   Data Connection values are properly set.&lt;/div&gt;&lt;p&gt;         3.   Job executed with no error.&lt;/p&gt;&lt;p&gt;         4.   No error in log files.&lt;/p&gt;&lt;div&gt; &lt;/div&gt;&lt;/td&gt;&lt;/tr&gt;&lt;/tbody&gt;&lt;/table&gt;";
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = id,
				Name = "zagzag",
				Description = description,
				EntityTypeID = 4,
				ParentProjectID = 1
			});
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "+table",
				ProjectIds = new[] { 1 }
			});
			result.Total.Should(Be.EqualTo(0), "result.Total.Should(Be.EqualTo(0))");
		}

		[Test]
		public void ShouldFindTestStep()
		{
			var indexer = GetInstance<IEntityIndexer>();
			const int testCaseTypeId = QueryEntityTypeProvider.TESTCASE_TYPE_ID;
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = 1,
				Name = "Searchable Test Case Name",
				EntityTypeID = testCaseTypeId,
				ParentProjectID = 1
			});
			indexer.AddTestStepIndex(new TestStepDTO
			{
				ID = 1,
				Description = "Searchable Test Step Description",
				Result = "Searchable Test Step Result",
				TestCaseID = 1
			});
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "Searchable Test Step",
				ProjectIds = new[] { 1 },
				EntityTypeId = testCaseTypeId
			});
			result.Total.Should(Be.EqualTo(1), "result.Total.Should(Be.EqualTo(1))");
			result.TestStepIds.Should(Be.EquivalentTo(new[] { 1 }.Select(x => x.ToString(CultureInfo.InvariantCulture))), "result.TestStepIds.Should(Be.EquivalentTo(new[] { 1 }.Select(x => x.ToString(CultureInfo.InvariantCulture))))");
		}

		[Test]
		public void ShouldFindTestStepAfterIndexIsUpdated()
		{
			var indexer = GetInstance<IEntityIndexer>();
			const int testCaseTypeId = QueryEntityTypeProvider.TESTCASE_TYPE_ID;
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = 1,
				Name = "Searchable Test Case Name",
				EntityTypeID = testCaseTypeId,
				ParentProjectID = 1
			});
			indexer.AddTestStepIndex(new TestStepDTO
			{
				ID = 1,
				Description = "Searchable Test Step Description",
				Result = "Searchable Test Step Result",
				TestCaseID = 1
			});
			indexer.UpdateTestStepIndex(new TestStepDTO
			{
				ID = 1,
				Result = "Updated Searchable Test Step Result",
				TestCaseID = 1
			}, new[] { TestStepField.Result }, Maybe.Nothing);
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "Updated",
				ProjectIds = new[] { 1 },
				EntityTypeId = testCaseTypeId
			});
			result.Total.Should(Be.EqualTo(1), "result.Total.Should(Be.EqualTo(1))");
			result.TestStepIds.Should(Be.EquivalentTo(new[] { 1 }.Select(x => x.ToString(CultureInfo.InvariantCulture))), "result.TestStepIds.Should(Be.EquivalentTo(new[] { 1 }.Select(x => x.ToString(CultureInfo.InvariantCulture))))");
		}

		[Test]
		public void ShouldNotFindTestStepAfterIndexIsRemoved()
		{
			var indexer = GetInstance<IEntityIndexer>();
			const int testCaseTypeId = QueryEntityTypeProvider.TESTCASE_TYPE_ID;
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = 1,
				Name = "Searchable Test Case Name",
				EntityTypeID = testCaseTypeId,
				ParentProjectID = 1
			});
			indexer.AddTestStepIndex(new TestStepDTO
			{
				ID = 1,
				Description = "Searchable Test Step Description",
				Result = "Searchable Test Step Result",
				TestCaseID = 1
			});
			var queryRunner = GetInstance<QueryRunner>();
			var queryData = new QueryData
			{
				Query = "Searchable Test Step",
				ProjectIds = new[] {1},
				EntityTypeId = testCaseTypeId
			};
			var result = queryRunner.Run(queryData);
			result.Total.Should(Be.EqualTo(1), "result.Total.Should(Be.EqualTo(1))");
			result.TestStepIds.Should(Be.EquivalentTo(new[] { 1 }.Select(x => x.ToString(CultureInfo.InvariantCulture))), "result.TestStepIds.Should(Be.EquivalentTo(new[] { 1 }.Select(x => x.ToString(CultureInfo.InvariantCulture))))");
			indexer.RemoveTestStepIndex(new TestStepDTO
			{
				ID = 1,
				TestCaseID = 1
			});
			var resultAfterRemove = queryRunner.Run(queryData);
			resultAfterRemove.Total.Should(Be.EqualTo(0), "resultAfterRemove.Total.Should(Be.EqualTo(0))");
		}

		[Test]
		public void ShouldFilterTestStepsByTestCaseProject()
		{
			var indexer = GetInstance<IEntityIndexer>();
			const int testCaseTypeId = QueryEntityTypeProvider.TESTCASE_TYPE_ID;
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = 1,
				Name = "Searchable Test Case Name",
				EntityTypeID = testCaseTypeId,
				ParentProjectID = 1
			});
			indexer.AddGeneralIndex(new GeneralDTO
			{
				ID = 2,
				Name = "Searchable Test Case 2 Name",
				EntityTypeID = testCaseTypeId,
				ParentProjectID = 2
			});
			indexer.AddTestStepIndex(new TestStepDTO
			{
				ID = 1,
				Description = "Searchable Test Step 1 Description",
				Result = "Searchable Test Step 1 Result",
				TestCaseID = 1
			});
			indexer.AddTestStepIndex(new TestStepDTO
			{
				ID = 2,
				Description = "Searchable Test Step 2 Description",
				Result = "Searchable Test Step 2 Result",
				TestCaseID = 2
			});
			var queryRunner = GetInstance<QueryRunner>();
			var result = queryRunner.Run(new QueryData
			{
				Query = "Searchable Test Step",
				ProjectIds = new[] { 2 },
				EntityTypeId = testCaseTypeId
			});
			result.Total.Should(Be.EqualTo(1), "result.Total.Should(Be.EqualTo(1))");
			result.TestStepIds.Should(Be.EquivalentTo(new[] { 2 }.Select(x => x.ToString(CultureInfo.InvariantCulture))), "result.TestStepIds.Should(Be.EquivalentTo(new[] { 2 }.Select(x => x.ToString(CultureInfo.InvariantCulture))))");
		}
	}
}
