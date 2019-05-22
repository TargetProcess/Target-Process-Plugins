using System.Linq;
using Tp.Integration.Common;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Plugins.Toolkit.Repositories
{
    public class DataRepository<T> : Repository<T>, IRepository<T> where T : IDataTransferObject
    {
        public DataRepository(IStorageRepository storageRepository) : base(storageRepository)
        {
        }

        public T Find(int? id)
        {
            return StorageRepository.Get<T>(id.ToString()).FirstOrDefault();
        }

        protected override IStorage<T> GetStorage(T item)
        {
            return StorageRepository.Get<T>(item.ID.ToString());
        }
    }
}
