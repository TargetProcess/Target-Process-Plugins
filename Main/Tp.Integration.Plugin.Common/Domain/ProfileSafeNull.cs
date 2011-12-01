using Tp.Core;
using Tp.Integration.Messages;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Storage.Persisters;

namespace Tp.Integration.Plugin.Common.Domain
{
	class ProfileSafeNull : SafeNull<ProfileSafeNull, IProfile>, IProfile
	{
		public IStorage<T> Get<T>()
		{
			return new StorageSafeNull<T>();
		}

		public IStorage<T> Get<T>(params StorageName[] storageNames)
		{
			return Get<T>();
		}

		public T GetProfile<T>()
		{
			return default(T);
		}

		public ProfileName Name
		{
			get { return new ProfileName(); }
		}

		public bool Initialized
		{
			get { return false; }
		}

		public object Settings
		{
			get { return new object(); }
			set { }
		}

		public IActivityLog Log
		{
			get { return ActivityLogSafeNull.Instance; }
		}

		public void Save()
		{
		}

		public void MarkAsInitialized()
		{
		}

		public void MarkAsNotInitialized()
		{
		}
	}
}