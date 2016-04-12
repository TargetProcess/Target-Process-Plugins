using System.Collections.Generic;
using Tp.Core;
using Tp.Search.Model.Document;

namespace Tp.Search.Model.Query
{
	interface IProjectContextQueryPlanBuilder
	{
		Maybe<QueryPlan> BuildProjectContextPlan(IEnumerable<int> projectIds, bool includeNoProject, DocumentIndexTypeToken projectIndexTypeToken);
	}
}