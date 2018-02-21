using System;
using System.Collections.Generic;

namespace Tp.Search.Model.Document.IndexAttribute
{
    interface IDocumentFieldsProvider
    {
        IEnumerable<Enum> DocumentFields { get; }
    }
}
