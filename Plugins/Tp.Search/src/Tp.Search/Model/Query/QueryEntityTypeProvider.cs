// 
// Copyright (c) 2005-2014 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Linq;
using Tp.Core;
using Tp.Search.Model.Document;
using Tp.Search.Model.Entity;

namespace Tp.Search.Model.Query
{
	/// <summary>
	/// Contains the values of entity type id
	/// </summary>
	class QueryEntityTypeProvider : IQueryResultFactory, IEntityTypeProvider
	{
		public class SearchResult
		{
			public SearchResult()
			{
				GeneralIds = new List<string>();
				AssignableIds = new List<string>();
				ImpedimentIds = new List<string>();
			}
			public List<string> GeneralIds { get; private set; }
			public List<string> AssignableIds { get; private set; }
			public List<string> ImpedimentIds { get; private set; }
		}

		private readonly List<int> _entityTypeIds;
		private readonly Dictionary<string, int> _generalTypeIds;
		private readonly Dictionary<string, int> _assignableTypeIds;
		private readonly Dictionary<int?, string> _entityTypeNames;
		private readonly List<string> _noSquadEntityTypeNames;
		private readonly string _impedimentType;

		private readonly IDocumentIdFactory _documentIdFactory;

		public QueryEntityTypeProvider(IDocumentIdFactory documentIdFactory)
		{
			_entityTypeIds = new List<int>();
			_generalTypeIds = new Dictionary<string, int>();
			_assignableTypeIds = new Dictionary<string, int>();
			_documentIdFactory = documentIdFactory;
			_impedimentType = documentIdFactory.CreateEntityTypeId(IMPEDIMENT_TYPE_ID);
			_entityTypeIds.Add(RELEASE_TYPE_ID);
			_entityTypeIds.Add(ITERATION_TYPE_ID);
			_entityTypeIds.Add(TESTPLAN_TYPE_ID);
			_entityTypeIds.Add(USERSTORY_TYPE_ID);
			_entityTypeIds.Add(TASK_TYPE_ID);
			_entityTypeIds.Add(BUG_TYPE_ID);
			_entityTypeIds.Add(FEATURE_TYPE_ID);
			_entityTypeIds.Add(EPIC_TYPE_ID);
			_entityTypeIds.Add(TESTCASE_TYPE_ID);
			_entityTypeIds.Add(TESTPLANRUN_TYPE_ID);
			_entityTypeIds.Add(REQUEST_TYPE_ID);
			_entityTypeIds.Add(IMPEDIMENT_TYPE_ID);
			_entityTypeIds.Add(COMMENT_TYPE_ID);

			_assignableTypeIds.Add(documentIdFactory.CreateEntityTypeId(USERSTORY_TYPE_ID), USERSTORY_TYPE_ID);
			_assignableTypeIds.Add(documentIdFactory.CreateEntityTypeId(TASK_TYPE_ID), TASK_TYPE_ID);
			_assignableTypeIds.Add(documentIdFactory.CreateEntityTypeId(BUG_TYPE_ID), BUG_TYPE_ID);
			_assignableTypeIds.Add(documentIdFactory.CreateEntityTypeId(FEATURE_TYPE_ID), FEATURE_TYPE_ID);
			_assignableTypeIds.Add(documentIdFactory.CreateEntityTypeId(EPIC_TYPE_ID), EPIC_TYPE_ID);
			_assignableTypeIds.Add(documentIdFactory.CreateEntityTypeId(TESTPLAN_TYPE_ID), TESTPLAN_TYPE_ID);
			_assignableTypeIds.Add(documentIdFactory.CreateEntityTypeId(TESTPLANRUN_TYPE_ID), TESTPLANRUN_TYPE_ID);
			_assignableTypeIds.Add(documentIdFactory.CreateEntityTypeId(REQUEST_TYPE_ID), REQUEST_TYPE_ID);

			_generalTypeIds.Add(documentIdFactory.CreateEntityTypeId(TESTCASE_TYPE_ID), TESTCASE_TYPE_ID);
			_generalTypeIds.Add(documentIdFactory.CreateEntityTypeId(RELEASE_TYPE_ID), RELEASE_TYPE_ID);
			_generalTypeIds.Add(documentIdFactory.CreateEntityTypeId(ITERATION_TYPE_ID), ITERATION_TYPE_ID);
			_generalTypeIds.Add(documentIdFactory.CreateEntityTypeId(IMPEDIMENT_TYPE_ID), IMPEDIMENT_TYPE_ID);

			_entityTypeNames = new Dictionary<int?, string>
				{
					{8, "Bug".ToLower()},
					{19, "Comment".ToLower()},
					{9, "Feature".ToLower()},
					{3, "Iteration".ToLower()},
					{2, "Release".ToLower()},
					{17, "Request".ToLower()},
					{5, "Task".ToLower()},
					{12, "TestCase".ToLower()},
					{13, "TestPlan".ToLower()},
					{14, "TestPlanRun".ToLower()},
					{4, "UserStory".ToLower()},
					{16, "Impediment".ToLower()},
					{27, "Epic".ToLower()}
				};

			_noSquadEntityTypeNames = new List<string>
				{
					"TestCase".ToLower(),
					"Release".ToLower(),
					"Iteration".ToLower(),
					"Impediment".ToLower(),
				};
		}

		/// <summary>
		/// Bug Type ID
		/// </summary>
		public const int BUG_TYPE_ID = 8;

		/// <summary>
		/// Feature Type ID
		/// </summary>
		public const int FEATURE_TYPE_ID = 9;

		/// <summary>
		/// Impediment Type ID
		/// </summary>
		public const int IMPEDIMENT_TYPE_ID = 16;

		/// <summary>
		/// Iteration Type ID
		/// </summary>
		public const int ITERATION_TYPE_ID = 3;

		/// <summary>
		/// Release Type ID
		/// </summary>
		public const int RELEASE_TYPE_ID = 2;

		/// <summary>
		/// Request Type ID
		/// </summary>
		public const int REQUEST_TYPE_ID = 17;

		/// <summary>
		/// Task Type ID
		/// </summary>
		public const int TASK_TYPE_ID = 5;

		/// <summary>
		/// Test Case Type ID
		/// </summary>
		public const int TESTCASE_TYPE_ID = 12;

		/// <summary>
		/// Test Plan Type ID
		/// </summary>
		public const int TESTPLAN_TYPE_ID = 13;

		/// <summary>
		/// Test Plan Run Type ID
		/// </summary>
		public const int TESTPLANRUN_TYPE_ID = 14;

		/// <summary>
		/// User Story Type ID
		/// </summary>
		public const int USERSTORY_TYPE_ID = 4;

		/// <summary>
		/// Comment Type I
		/// </summary>
		public const int COMMENT_TYPE_ID = 19;

		/// <summary>
		/// Epic Type ID
		/// </summary>
		public const int EPIC_TYPE_ID = 27;

		public Maybe<string> GetEntityTypeName(int? entityTypeId)
		{
			return _entityTypeNames.GetValue(entityTypeId);
		}

		public bool IsQueryable(int? entityTypeId)
		{
			return EntityTypeIds.Contains(entityTypeId.GetValueOrDefault());
		}

		public bool IsAssignable(int? entityTypeId)
		{
			return _assignableTypeIds.ContainsKey(_documentIdFactory.CreateEntityTypeId(entityTypeId.GetValueOrDefault()));
		}

		public bool IsTestCase(int? entityTypeId)
		{
			return entityTypeId == TESTCASE_TYPE_ID;
		}

		public bool IsImpediment(int? entityTypeId)
		{
			return entityTypeId == IMPEDIMENT_TYPE_ID;
		}

		public IEnumerable<string> NoSquadEntityTypeNames { get { return _noSquadEntityTypeNames; } }

		public int?[] EntityTypeIds
		{
			get { return _entityTypeIds.Select(x => (int?)x).ToArray(); }
		}

		public SearchResult CreateQueryResult(IEnumerable<EntityDocument> entityDocuments)
		{
			var result = new SearchResult();
			foreach (EntityDocument doc in entityDocuments)
			{
				if (IsImpediment(doc))
				{
					result.ImpedimentIds.Add(doc.FileName);
					continue;
				}
				if (IsGeneral(doc))
				{
					result.GeneralIds.Add(doc.FileName);
					continue;
				}
				if (IsAssignable(doc))
				{
					result.AssignableIds.Add(doc.FileName);
				}
			}
			return result;
		}

		private bool IsGeneral(EntityDocument entityDocument)
		{
			return _generalTypeIds.ContainsKey(entityDocument.EntityTypeId);
		}

		private bool IsAssignable(EntityDocument entityDocument)
		{
			return _assignableTypeIds.ContainsKey(entityDocument.EntityTypeId);
		}
		
		private bool IsImpediment(EntityDocument entityDocument)
		{
			return entityDocument.EntityTypeId == _impedimentType;
		}
	}
}