using System;
using System.Linq;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Queries;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Search.Model.Entity;

namespace Tp.Search.Bus.Workflow
{
    internal abstract class IndexAlgorithm<TDto, TSagaData, TQuery> where TDto : DataTransferObject
        where TQuery : QueryBase
    {
        private const int DataWindowSize = 1000;
        private readonly IEntityIndexer _entityIndexer;
        private readonly Func<TSagaData> _data;
        private readonly IEntityTypeProvider _entityTypesProvider;
        private readonly Action<TSagaData> _onComplete;
        private readonly Action<TQuery> _sendQuery;
        private readonly IActivityLogger _logger;
        private readonly string _entityName;

        protected IndexAlgorithm(IEntityIndexer entityIndexer, Func<TSagaData> data, IEntityTypeProvider entityTypesProvider,
            Action<TSagaData> onComplete, Action<TQuery> sendQuery, IActivityLogger logger, string entityName)
        {
            _entityIndexer = entityIndexer;
            _data = data;
            _entityTypesProvider = entityTypesProvider;
            _onComplete = onComplete;
            _sendQuery = sendQuery;
            _logger = logger;
            _entityName = entityName;
        }

        protected TSagaData Data
        {
            get { return _data(); }
        }

        protected IEntityIndexer EntityIndexer
        {
            get { return _entityIndexer; }
        }

        protected IEntityTypeProvider EntityTypesProvider
        {
            get { return _entityTypesProvider; }
        }

        protected abstract void IndexEntity(TDto dto);
        protected abstract void OptimizeIndex();
        protected abstract void IncrementCounters(int count);
        protected abstract int GetCurrentDataWindowSize();
        protected abstract void ResetCurrentDataWindowSize();
        protected abstract int GetTotalRetrievedEntitiesCount();
        protected abstract void ResetTotalRetrievedEntitiesCount();

        public void Start()
        {
            ResetTotalRetrievedEntitiesCount();
            _sendQuery(BuildQuery(0, DataWindowSize));
        }

        public void Handle(QueryResult<TDto> message)
        {
            IncrementCounters(message.Dtos.Length + message.FailedDtosCount);
            if (message.FailedDtosCount > 0)
            {
                _logger.WarnFormat("Failed to index {0} {1} entities", message.FailedDtosCount, _entityName);
            }
            foreach (TDto dto in message.Dtos)
            {
                IndexEntity(dto);
            }
            if (message.Dtos.Empty() && message.FailedDtosCount == 0)
            {
                OptimizeIndex();
                _onComplete(Data);
            }
            else if (GetCurrentDataWindowSize() == message.QueryResultCount)
            {
                _logger.Info("{0} {1} entities were indexed".Fmt(GetTotalRetrievedEntitiesCount(), _entityName));
                OptimizeIndex();
                _sendQuery(BuildQuery(GetTotalRetrievedEntitiesCount(), DataWindowSize));
                ResetCurrentDataWindowSize();
            }
        }

        protected abstract TQuery CreateQuery();

        private TQuery BuildQuery(int skip, int take)
        {
            var q = CreateQuery();
            q.Skip = skip;
            q.Take = take;
            q.IgnoreMessageSizeOverrunFailure = true;
            return q;
        }
    }
}
