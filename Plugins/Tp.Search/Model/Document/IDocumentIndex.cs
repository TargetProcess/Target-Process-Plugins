using System;
using System.Collections.Generic;
using hOOt;

namespace Tp.Search.Model.Document
{
	interface IDocumentIndex
	{
		List<TDocument> Find<TDocument>(Lazy<WAHBitArray> plan, int page, int pageSize, int skip, out int total) where TDocument : hOOt.Document;
		T FindDocumentByName<T>(string name) where T : hOOt.Document, new();
		IndexResult Index(hOOt.Document document, bool deleteOld);
		IndexResult Index(int recordNumber, string text);
		void SaveDocument(hOOt.Document document, bool deleteOld);
		IndexResult Rebuild(hOOt.Document document, bool deleteOld = false);
		IndexResult Update(string fileName, string text);
		IndexResult Update(int recordNumber, string text);
		void Optimize(bool freeMemory);
		bool Shutdown(DocumentIndexShutdownSetup setup);
		Lazy<WAHBitArray> BuildExecutionPlan(string query, bool freeCache);
		DocumentIndexType Type { get; }
		T GetLastDocument<T>() where T : hOOt.Document;
	}
}