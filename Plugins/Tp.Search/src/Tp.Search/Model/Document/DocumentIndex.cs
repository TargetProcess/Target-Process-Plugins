using System;
using System.Collections.Generic;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Search.Model.Optimization;
using Tp.Search.Model.Query;
using hOOt;

namespace Tp.Search.Model.Document
{
    class DocumentIndex : IDocumentIndex
    {
        private readonly IDocumentIndex _charactersIndex;
        private readonly IDocumentIndex _numberIndex;

        public DocumentIndex(DocumentIndexType charactersIndexType, DocumentIndexType digitsIndexType, IPluginContext context,
            Action shuttedDown, DocumentIndexSetup documentIndexSetup, IActivityLoggerFactory loggerFactory,
            DocumentIndexOptimizeHintFactory optimizeHintFactory)
        {
            _charactersIndex = new DocumentIndexTyped(charactersIndexType, context, shuttedDown, documentIndexSetup, loggerFactory,
                optimizeHintFactory);
            _numberIndex = new DocumentIndexTyped(digitsIndexType, context, shuttedDown, documentIndexSetup, loggerFactory,
                optimizeHintFactory);
        }

        public IndexResult Index(hOOt.Document document, bool deleteOld, DocumentIndexOptimizeSetup setup)
        {
            var characterIndexResult = _charactersIndex.Index(document, deleteOld, setup);
            var numberIndexResult = _numberIndex.Index(document.DocNumber, document.Text, setup);
            return Merge(characterIndexResult, numberIndexResult);
        }

        public IndexResult Index(int recordNumber, string text, DocumentIndexOptimizeSetup setup)
        {
            var characterIndexResult = _charactersIndex.Index(recordNumber, text, setup);
            var numberIndexResult = _numberIndex.Index(recordNumber, text, setup);
            return Merge(characterIndexResult, numberIndexResult);
        }

        public IndexResult Rebuild(hOOt.Document document, bool deleteOld, DocumentIndexOptimizeSetup setup)
        {
            var characterIndexResult = _charactersIndex.Rebuild(document, deleteOld, setup);
            var numberIndexResult = _numberIndex.Update(document.DocNumber, document.Text, setup);
            return Merge(characterIndexResult, numberIndexResult);
        }

        public IndexResult Update(string fileName, string text, DocumentIndexOptimizeSetup setup)
        {
            var characterIndexResult = _charactersIndex.Update(fileName, text, setup);
            var numberIndexResult = _numberIndex.Update(characterIndexResult.DocNumber, text, setup);
            return Merge(characterIndexResult, numberIndexResult);
        }

        public IndexResult Update(int recordNumber, string text, DocumentIndexOptimizeSetup setup)
        {
            var charachterResult = _charactersIndex.Update(recordNumber, text, setup);
            var numberResult = _numberIndex.Update(recordNumber, text, setup);
            return Merge(charachterResult, numberResult);
        }

        public void Optimize(DocumentIndexOptimizeSetup setup)
        {
            _charactersIndex.Optimize(setup);
            _numberIndex.Optimize(setup);
        }

        public bool Shutdown(DocumentIndexShutdownSetup setup)
        {
            if (!ShouldShutdown(setup))
            {
                return false;
            }
            bool characterResult = _charactersIndex.Shutdown(setup);
            bool numberResult = _numberIndex.Shutdown(setup);
            return characterResult && numberResult;
        }

        public bool ShouldShutdown(DocumentIndexShutdownSetup setup)
        {
            return _charactersIndex.ShouldShutdown(setup) && _numberIndex.ShouldShutdown(setup);
        }

        public QueryPlan BuildExecutionPlan(ParsedQuery parsedQuery, bool freeCache)
        {
            QueryPlan characterPlan = _charactersIndex.BuildExecutionPlan(new ParsedQuery(words: parsedQuery.Words), freeCache);
            if (string.IsNullOrEmpty(parsedQuery.Numbers))
            {
                return characterPlan;
            }
            QueryPlan numberPlan = _numberIndex.BuildExecutionPlan(new ParsedQuery(numbers: parsedQuery.Numbers), freeCache);
            if (string.IsNullOrEmpty(parsedQuery.Words))
            {
                return numberPlan;
            }
            return QueryPlan.And(characterPlan, numberPlan);
        }

        public DocumentIndexType Type
        {
            get { return _charactersIndex.Type; }
        }

        public bool IsAlive
        {
            get { return _charactersIndex.IsAlive && _numberIndex.IsAlive; }
        }

        public bool IsOptimized
        {
            get { return _charactersIndex.IsOptimized && _numberIndex.IsOptimized; }
        }

        public List<TDocument> Find<TDocument>(QueryPlanResult plan, int page, int pageSize, int skip, out int total)
            where TDocument : hOOt.Document
        {
            return _charactersIndex.Find<TDocument>(plan, page, pageSize, skip, out total);
        }

        public T FindDocumentByName<T>(string name) where T : hOOt.Document, new()
        {
            return _charactersIndex.FindDocumentByName<T>(name);
        }

        public IndexData GetExistingIndexByNumber(int number)
        {
            return _charactersIndex.GetExistingIndexByNumber(number);
        }

        public void SaveDocument(hOOt.Document document, bool deleteOld)
        {
            _charactersIndex.SaveDocument(document, deleteOld);
        }

        public T GetLastDocument<T>() where T : hOOt.Document
        {
            return _charactersIndex.GetLastDocument<T>();
        }

        private static IndexResult Merge(IndexResult left, IndexResult right)
        {
            if (left.DocNumber != right.DocNumber)
            {
                throw new ApplicationException("Inconsistent index results");
            }
            var wordsAdded = new Dictionary<string, int>(left.WordsAdded);
            foreach (var w in right.WordsAdded)
            {
                wordsAdded.Add(w.Key, w.Value);
            }
            var wordsRemoved = new List<string>(left.WordsRemoved);
            wordsRemoved.AddRange(right.WordsRemoved);
            return new IndexResult
            {
                DocNumber = left.DocNumber,
                WordsAdded = wordsAdded,
                WordsRemoved = wordsRemoved
            };
        }
    }
}
