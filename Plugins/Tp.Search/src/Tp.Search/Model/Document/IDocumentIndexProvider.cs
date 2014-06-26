using System.Collections.Generic;
using Tp.Core;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Search.Model.Document
{
	interface IDocumentIndexProvider
	{
		IEnumerable<DocumentIndexType> DocumentIndexTypes { get; }
		IDocumentIndex GetOrCreateDocumentIndex(IPluginContext context, DocumentIndexTypeToken indexTypeToken);
		Maybe<IDocumentIndex> GetDocumentIndex(IPluginContext context, DocumentIndexTypeToken indexTypeToken);
		IEnumerable<IDocumentIndex> GetOrCreateDocumentIndexes(IPluginContext context, params DocumentIndexTypeToken[] indexTypeTokens);
	}
}
