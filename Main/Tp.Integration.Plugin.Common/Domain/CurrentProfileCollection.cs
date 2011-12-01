using System.Collections;
using System.Collections.Generic;
using Tp.Integration.Messages;

namespace Tp.Integration.Plugin.Common.Domain
{
	class CurrentProfileCollection : IProfileCollection, IProfileCollectionReadonly
	{
		private readonly IPluginCurrentObjectContext _currentObjectContext;

		public CurrentProfileCollection(IPluginCurrentObjectContext currentObjectContext)
		{
			_currentObjectContext = currentObjectContext;
		}

		IEnumerator<IProfileReadonly> IEnumerable<IProfileReadonly>.GetEnumerator()
		{
			IPluginCurrentObjectContextReadonly c = _currentObjectContext;
			return c.ProfileCollection.GetEnumerator();
		}

		public IEnumerator<IProfile> GetEnumerator()
		{
			return _currentObjectContext.ProfileCollection.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IProfile this[ProfileName profileName]
		{
			get { return _currentObjectContext.ProfileCollection[profileName]; }
		}

		public void Remove(IProfile profile)
		{
			_currentObjectContext.ProfileCollection.Remove(profile);
		}

		public IProfile Add(ProfileCreationArgs profileCreationArgs)
		{
			return _currentObjectContext.ProfileCollection.Add(profileCreationArgs);
		}

		IProfileReadonly IProfileCollectionReadonly.this[ProfileName profileName]
		{
			get
			{
				IPluginCurrentObjectContextReadonly c = _currentObjectContext;
				return c.ProfileCollection[profileName];
			}
		}
	}
}
