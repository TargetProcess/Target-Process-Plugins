using System;
using System.Collections.Generic;
using System.Threading;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Search.Model.Exceptions;
using Tp.Search.Model.Query;
using hOOt;

namespace Tp.Search.Model.Document
{
    class DocumentIndexMonitor : IDocumentIndex, ILock
    {
        private readonly Func<IDocumentIndex> _documentIndexFactory;
        private readonly IActivityLogger _logger;
        private IDocumentIndex _documentIndex;
        private readonly object _gate;
        private readonly DocumentIndexType _documentIndexType;
        private int _version;

        public DocumentIndexMonitor(DocumentIndexType documentIndexType, Func<IDocumentIndex> documentIndexFactory, IActivityLogger logger)
        {
            _documentIndexFactory = documentIndexFactory;
            _logger = logger;
            _gate = new object();
            _documentIndexType = documentIndexType;
            BornOrRessurectIfDead();
        }

        private class LockRegion : IDisposable
        {
            private readonly DocumentIndexMonitor _monitor;

            public LockRegion(DocumentIndexMonitor monitor)
            {
                _monitor = monitor;
                Monitor.Enter(_monitor._gate);
            }

            public void Dispose()
            {
                Monitor.Exit(_monitor._gate);
            }
        }

        public LockToken Lock()
        {
            return new LockToken
            {
                Token = new LockRegion(this),
                TypeToken = Type.TypeToken,
                Version = _version
            };
        }

        public void DetachIndex()
        {
            _logger.DebugFormat("[shutdown {0}] index was detached", _documentIndexType.TypeToken);
            _documentIndex = null;
        }

        public List<TDocument> Find<TDocument>(QueryPlanResult plan, int page, int pageSize, int skip, out int total)
            where TDocument : hOOt.Document
        {
            lock (_gate)
            {
                BornOrRessurectIfDead();
                if (plan.GetVersion(Type.TypeToken) != _version)
                {
                    throw new DocumentIndexConcurrentAccessException();
                }
                return _documentIndex.Find<TDocument>(plan, page, pageSize, skip, out total);
            }
        }

        public T FindDocumentByName<T>(string name) where T : hOOt.Document, new()
        {
            lock (_gate)
            {
                BornOrRessurectIfDead();
                return _documentIndex.FindDocumentByName<T>(name);
            }
        }

        public IndexData GetExistingIndexByNumber(int number)
        {
            lock (_gate)
            {
                BornOrRessurectIfDead();
                return _documentIndex.GetExistingIndexByNumber(number);
            }
        }

        public IndexResult Index(hOOt.Document document, bool deleteOld, DocumentIndexOptimizeSetup setup)
        {
            lock (_gate)
            {
                BornOrRessurectIfDead();
                UpdateVersion();
                return _documentIndex.Index(document, deleteOld, setup);
            }
        }

        public IndexResult Index(int recordNumber, string text, DocumentIndexOptimizeSetup setup)
        {
            lock (_gate)
            {
                BornOrRessurectIfDead();
                UpdateVersion();
                return _documentIndex.Index(recordNumber, text, setup);
            }
        }

        public void SaveDocument(hOOt.Document document, bool deleteOld)
        {
            lock (_gate)
            {
                BornOrRessurectIfDead();
                UpdateVersion();
                _documentIndex.SaveDocument(document, deleteOld);
            }
        }

        public IndexResult Rebuild(hOOt.Document document, bool deleteOld, DocumentIndexOptimizeSetup setup)
        {
            lock (_gate)
            {
                BornOrRessurectIfDead();
                UpdateVersion();
                return _documentIndex.Rebuild(document, deleteOld, setup);
            }
        }

        public IndexResult Update(string fileName, string text, DocumentIndexOptimizeSetup setup)
        {
            lock (_gate)
            {
                BornOrRessurectIfDead();
                UpdateVersion();
                return _documentIndex.Update(fileName, text, setup);
            }
        }

        public IndexResult Update(int recordNumber, string text, DocumentIndexOptimizeSetup setup)
        {
            lock (_gate)
            {
                BornOrRessurectIfDead();
                UpdateVersion();
                return _documentIndex.Update(recordNumber, text, setup);
            }
        }

        public void Optimize(DocumentIndexOptimizeSetup setup)
        {
            lock (_gate)
            {
                BornOrRessurectIfDead();
                UpdateVersion();
                _documentIndex.Optimize(setup);
            }
        }

        public bool Shutdown(DocumentIndexShutdownSetup setup)
        {
            lock (_gate)
            {
                if (_documentIndex == null)
                {
                    return false;
                }
                UpdateVersion();
                return _documentIndex.Shutdown(setup);
            }
        }

        public bool ShouldShutdown(DocumentIndexShutdownSetup setup)
        {
            lock (_gate)
            {
                if (_documentIndex == null)
                {
                    return false;
                }
                UpdateVersion();
                return _documentIndex.ShouldShutdown(setup);
            }
        }

        public QueryPlan BuildExecutionPlan(ParsedQuery parsedQuery, bool freeCache)
        {
            lock (_gate)
            {
                BornOrRessurectIfDead();
                QueryPlan queryPlan = _documentIndex.BuildExecutionPlan(parsedQuery, freeCache);
                queryPlan.AddLock(this);
                return queryPlan;
            }
        }

        public T GetLastDocument<T>() where T : hOOt.Document
        {
            lock (_gate)
            {
                BornOrRessurectIfDead();
                return _documentIndex.GetLastDocument<T>();
            }
        }

        public bool IsAlive
        {
            get { return true; }
        }

        public bool IsOptimized
        {
            get { return _documentIndex != null ? _documentIndex.IsOptimized : true; }
        }

        public DocumentIndexTypeToken Token
        {
            get { return Type.TypeToken; }
        }

        public DocumentIndexType Type
        {
            get { return _documentIndexType; }
        }

        private void BornOrRessurectIfDead()
        {
            if (_documentIndex == null || !_documentIndex.IsAlive)
            {
                _logger.DebugFormat("[{0}] Create index because {1}", _documentIndexType.TypeToken,
                    _documentIndex == null ? "there is no current index" : "current index is not alive");
                _documentIndex = _documentIndexFactory();
            }
        }

        private void UpdateVersion()
        {
            ++_version;
        }
    }
}
