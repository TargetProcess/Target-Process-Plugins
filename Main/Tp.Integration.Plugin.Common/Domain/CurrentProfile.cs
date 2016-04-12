using Tp.Integration.Messages;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.FileStorage;
using Tp.Integration.Plugin.Common.Storage.Persisters;

namespace Tp.Integration.Plugin.Common.Domain
{
	class CurrentProfile : IProfile
	{
		private readonly IPluginCurrentObjectContext _currentObjectContext;

		public CurrentProfile(IPluginCurrentObjectContext currentObjectContext)
		{
			_currentObjectContext = currentObjectContext;
		}

		public bool IsNull
		{
			get { return _currentObjectContext.Profile.IsNull; }
		}

		public IStorage<T> Get<T>()
		{
			return _currentObjectContext.Profile.Get<T>();
		}

		public IStorage<T> Get<T>(params StorageName[] storageNames)
		{
			return _currentObjectContext.Profile.Get<T>(storageNames);
		}

		public T GetProfile<T>()
		{
			return _currentObjectContext.Profile.GetProfile<T>();
		}

		public ProfileName Name
		{
			get { return _currentObjectContext.Profile.Name; }
		}

		object IProfile.Settings
		{
			get { return _currentObjectContext.Profile.Settings; }
			set { _currentObjectContext.Profile.Settings = value; }
		}

		public IActivityLog Log
		{
			get { return _currentObjectContext.Profile.Log; }
		}

		public IProfileFileStorage FileStorage
		{
			get { return _currentObjectContext.Profile.FileStorage; }
		}

		public void Save()
		{
			_currentObjectContext.Profile.Save();
		}

		public void MarkAsInitialized()
		{
			_currentObjectContext.Profile.MarkAsInitialized();
		}

		public void MarkAsNotInitialized()
		{
			_currentObjectContext.Profile.MarkAsNotInitialized();
		}

		object IProfileReadonly.Settings
		{
			get { return _currentObjectContext.Profile; }
		}

		public bool Initialized
		{
			get { return _currentObjectContext.Profile.Initialized; }
		}
	}
}
