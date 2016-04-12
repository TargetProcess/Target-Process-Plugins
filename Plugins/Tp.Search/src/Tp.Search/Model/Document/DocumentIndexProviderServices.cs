using System.Collections.Generic;
using System.Linq;
using Tp.Core;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Search.Model.Document
{
	static class DocumentIndexProviderServices
	{
		public static void ShutdownDocumentIndexesIfRunning(this IDocumentIndexProvider documentIndexProvider, IPluginContext context, DocumentIndexShutdownSetup setup, IActivityLogger logger)
		{
			foreach (IDocumentIndex documentIndex in GetDocumentIndexes(documentIndexProvider, context, runningOnly:true))
			{
				if (documentIndex.Shutdown(setup))
				{
					logger.DebugFormat("{0} was shutted down", documentIndex.Type.TypeToken);
				}
			}
		}

		public static void OptimizeDocumentIndexesIfRunning(this IDocumentIndexProvider documentIndexProvider, IPluginContext context, IActivityLogger logger)
		{
			foreach (IDocumentIndex documentIndex in GetDocumentIndexes(documentIndexProvider, context, runningOnly:true).Where(d => !d.IsOptimized))
			{
				documentIndex.Optimize(DocumentIndexOptimizeSetup.ImmediateOptimize);
			}
		}

		public static void ShutdownDocumentIndexes(this IDocumentIndexProvider documentIndexProvider, IPluginContext context, DocumentIndexShutdownSetup setup, IActivityLogger logger)
		{
			foreach (IDocumentIndex documentIndex in GetDocumentIndexes(documentIndexProvider, context, runningOnly:false))
			{
				var success = documentIndex.Shutdown(setup);
				logger.DebugFormat("{0} was {1} shutted down", documentIndex.Type.TypeToken, success ? string.Empty : "not");
			}
		}

		private static IEnumerable<IDocumentIndex> GetDocumentIndexes(IDocumentIndexProvider documentIndexProvider, IPluginContext context, bool runningOnly)
		{
			IEnumerable<IDocumentIndex> documentIndexes = documentIndexProvider.DocumentIndexTypes.Select(t => t.TypeToken)
				.Distinct()
				.Select(t => runningOnly ? documentIndexProvider.GetDocumentIndex(context, t) : Maybe.Return(documentIndexProvider.GetOrCreateDocumentIndex(context,t)))
				.Choose();
			return documentIndexes;
		}
	}
}
