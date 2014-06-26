using Tp.Core;

namespace Tp.Search.Model.Query
{
	struct QueryPlanFull
	{
		public ParsedQuery Query { get; set; }
		public Maybe<QueryPlan> EntityPlan { get; set; }
		public Maybe<QueryPlan> CommentPlan { get; set; }
	}
}