using System.Collections.Generic;
using Tp.Search.Model.Entity;

namespace Tp.Search.Model.Query
{
	struct QueryRunResult
	{
		public IEnumerable<EntityDocument> Entities { get; set; }
		public IEnumerable<hOOt.Document> Comments { get; set; }
		public IEnumerable<hOOt.Document> TestSteps { get; set; }
		public int EntitiesTotalCount { get; set; }
		public int CommentsTotalCount { get; set; }
		public int TestStepsTotalCount { get; set; }
		public int LastIndexedEntityId { get; set; }
		public int LastIndexedCommentId { get; set; }
		public int LastIndexedTestStepId { get; set; }
	}
}