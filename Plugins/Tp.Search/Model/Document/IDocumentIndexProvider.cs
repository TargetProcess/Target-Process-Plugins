using System.Collections.Generic;
using Tp.Core;
using Tp.Integration.Messages;

namespace Tp.Search.Model.Document
{
	interface IDocumentIndexProvider
	{
		IEnumerable<DocumentIndexTypeToken> DocumentTypes { get; }
		IDocumentIndex GetOrCreateDocumentIndex(AccountName accountName, DocumentIndexTypeToken indexTypeToken);
		Maybe<IDocumentIndex> GetDocumentIndex(AccountName accountName, DocumentIndexTypeToken indexTypeToken);
		IEnumerable<IDocumentIndex> GetOrCreateDocumentIndexes(AccountName accountName, params DocumentIndexTypeToken[] indexTypeTokens);
	}
}