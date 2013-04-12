using System.Collections.Generic;
using System.Linq;
using Tp.Core;
using Tp.Integration.Messages;

namespace Tp.Search.Model.Document
{
	static class DocumentIndexProviderServices
	{
		public static void ShutdownDocumentIndexesIfRunning(this IDocumentIndexProvider documentIndexProvider, AccountName accountName, DocumentIndexShutdownSetup setup)
		{
			IEnumerable<IDocumentIndex> documentIndexes = documentIndexProvider.DocumentTypes.Select(t => documentIndexProvider.GetDocumentIndex(accountName, t)).Choose();
			foreach (IDocumentIndex documentIndex in documentIndexes)
			{
				documentIndex.Shutdown(setup);
			}
		}

		public static void ShutdownDocumentIndexes(this IDocumentIndexProvider documentIndexProvider, AccountName accountName, DocumentIndexShutdownSetup setup)
		{
			IEnumerable<IDocumentIndex> documentIndexes = documentIndexProvider.DocumentTypes.Select(t => documentIndexProvider.GetOrCreateDocumentIndex(accountName, t));
			foreach (IDocumentIndex documentIndex in documentIndexes)
			{
				documentIndex.Shutdown(setup);
			}
		}
	}
}