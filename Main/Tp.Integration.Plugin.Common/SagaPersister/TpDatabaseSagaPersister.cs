// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using NServiceBus.Saga;
using StructureMap;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Storage;

namespace Tp.Integration.Plugin.Common.SagaPersister
{
    /// <summary>
    /// Persists sagas in plugin profile storage
    /// </summary>
    internal class TpDatabaseSagaPersister : ISagaPersister
    {
        private static IStorage<ISagaEntity> GetSagaStorage(Guid sagaId)
        {
            return ObjectFactory.GetInstance<IStorageRepository>().Get<ISagaEntity>(sagaId.ToString());
        }

        public void Save(ISagaEntity saga)
        {
            GetSagaStorage(saga.Id).Add(saga);
        }

        public void Update(ISagaEntity saga)
        {
            GetSagaStorage(saga.Id).Update(saga, x => x.Id == saga.Id);
        }

        public T Get<T>(Guid sagaId) where T : ISagaEntity
        {
            foreach (var tpSagaEntity in GetSagaStorage(sagaId))
            {
                if (!(tpSagaEntity is T)) continue;

                if (tpSagaEntity.Id == sagaId)
                    return (T) tpSagaEntity;
            }
            return default(T);
        }

        public T Get<T>(string property, object value) where T : ISagaEntity
        {
            if (property == "Id" && IsGuid(value))
            {
                return Get<T>(new Guid(value.ToString()));
            }

            foreach (var tpSagaEntity in ObjectFactory.GetInstance<IStorageRepository>().Get<ISagaEntity>())
            {
                if (!(tpSagaEntity is T)) continue;

                var prop = tpSagaEntity.GetType().GetProperty(property);
                if (prop != null)
                    if (prop.GetValue(tpSagaEntity, null).Equals(value))
                        return (T) tpSagaEntity;
            }

            return default(T);
        }

        private static bool IsGuid(object value)
        {
            try
            {
                new Guid(value.ToString());
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public void Complete(ISagaEntity saga)
        {
            GetSagaStorage(saga.Id).Clear();
        }
    }
}
