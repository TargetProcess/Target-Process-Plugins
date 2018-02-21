using System;
using NServiceBus.Saga;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Search.Bus.Utils
{
    class SagaServices
    {
        private readonly ISagaPersister _sagaPersister;
        private readonly IActivityLogger _logger;
        private readonly IStorageRepository _storageRepository;

        public SagaServices(ISagaPersister sagaPersister, IActivityLogger logger, IStorageRepository storageRepository)
        {
            _sagaPersister = sagaPersister;
            _logger = logger;
            _storageRepository = storageRepository;
        }

        public bool TryCompleteInprogressSaga<TSaga>(Guid sagaId)
        {
            try
            {
                foreach (ISagaEntity candidate in _storageRepository.Get<ISagaEntity>())
                {
                    if (!(candidate is TSaga))
                    {
                        continue;
                    }
                    if (candidate.Id != sagaId)
                    {
                        _sagaPersister.Complete(candidate);
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.ErrorFormat("Failed to complete Running Saga When start to rebuild indexed, error: ", e.Message);
            }
            return false;
        }
    }
}
