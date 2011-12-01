using Tp.Integration.Plugin.Common.Storage.Persisters;

namespace Tp.Integration.Plugin.Common.Domain
{
	class StorageRepositoryDelegate : IStorageRepository
	{
		protected StorageRepositoryDelegate()
		{
		}

		protected StorageRepositoryDelegate(StorageRepositoryDelegate other)
		{
			StorageRepository = other.StorageRepository;
		}

		public IStorageRepository StorageRepository
		{
			protected get; set;
		}

		public IStorage<T> Get<T>()
		{
			return StorageRepository.Get<T>();
		}

		public IStorage<T> Get<T>(params StorageName[] storageNames)
		{
			return StorageRepository.Get<T>(storageNames);
		}

		public T GetProfile<T>()
		{
			return StorageRepository.GetProfile<T>();
		}

		public bool IsNull
		{
			get { return false; }
		}
	}
}