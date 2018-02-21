using System.Collections.Generic;
using Tp.Search.Model.Entity;

namespace Tp.Search.Model.Query
{
    interface IQueryResultFactory
    {
        QueryEntityTypeProvider.SearchResult CreateQueryResult(IEnumerable<EntityDocument> entityDocuments);
    }
}
