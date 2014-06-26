using System.Collections.Generic;
using Tp.Search.Model.Query;
using hOOt;

namespace Tp.Search.Model.Document
{
	interface IDocumentIndex
	{
		List<TDocument> Find<TDocument>(QueryPlanResult plan, int page, int pageSize, int skip, out int total) where TDocument : hOOt.Document;
		T FindDocumentByName<T>(string name) where T : hOOt.Document, new();
		T GetLastDocument<T>() where T : hOOt.Document;
		void SaveDocument(hOOt.Document document, bool deleteOld);

		IndexResult Index(hOOt.Document document, bool deleteOld, DocumentIndexOptimizeSetup setup);
		IndexResult Rebuild(hOOt.Document document, bool deleteOld, DocumentIndexOptimizeSetup setup);
		IndexResult Index(int recordNumber, string text, DocumentIndexOptimizeSetup setup);
		IndexResult Update(int recordNumber, string text, DocumentIndexOptimizeSetup setup);
		IndexResult Update(string fileName, string text, DocumentIndexOptimizeSetup setup);
		void Optimize(DocumentIndexOptimizeSetup setup);

		bool Shutdown(DocumentIndexShutdownSetup setup);
		bool ShouldShutdown(DocumentIndexShutdownSetup setup);

		QueryPlan BuildExecutionPlan(ParsedQuery parsedQuery, bool freeCache);

		DocumentIndexType Type { get; }
		bool IsAlive { get; }
		bool IsOptimized { get; }
	}
}