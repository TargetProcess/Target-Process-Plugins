using Tp.Core;
using Tp.Search.Bus.Data;
using Tp.Search.Model.Document;

namespace Tp.Search.Model.Query
{
    interface IContextQueryPlanBuilder
    {
        Maybe<QueryPlan> Build(QueryData data, DocumentIndexTypeToken projectContextType, DocumentIndexTypeToken squadContextType,
            DocumentIndexTypeToken entityType);

        bool ShouldBuild(QueryData data);
    }
}
