using System.Collections.Generic;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Plugins.Toolkit.Repositories
{
    public abstract class Repository<T>
    {
        protected Repository(IStorageRepository storageRepository)
        {
            StorageRepository = storageRepository;
        }

        public IEnumerable<T> GetAll()
        {
            return StorageRepository.Get<T>();
        }

        public virtual void Add(T entity)
        {
            GetStorage(entity).Add(entity);
        }

        public void Delete(T dto)
        {
            GetStorage(dto).Clear();
        }

        public void Update(T dto)
        {
            Delete(dto);
            Add(dto);
        }

        protected IStorageRepository StorageRepository { get; }
        protected abstract IStorage<T> GetStorage(T item);
    }
}
