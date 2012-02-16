// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Tp.Integration.Messages;
using Tp.Integration.Plugin.Common.Events;
using Tp.Integration.Plugin.Common.Events.Aggregator;
using Tp.Integration.Plugin.Common.Storage.Repositories;

namespace Tp.Integration.Plugin.Common.Domain
{
	[DebuggerDisplay("AccountName =  {_accountName.Value}, ProfilesCount = {_profiles.Count}")]
	internal class ProfileCollection : IProfileCollection, IProfileCollectionReadonly
	{
		private readonly AccountName _accountName;
		private readonly Dictionary<ProfileName, ProfileDomainObject> _profiles;
		
		public ProfileCollection(AccountName accountName, IEnumerable<ProfileDomainObject> profiles)
		{
			_accountName = accountName;
			_profiles = profiles.ToDictionary(p => p.Name);
			ProfileRepository = null;
			EventAggregator = null;
		}

		public ProfileCollection(ProfileCollection other)
		{
			_accountName = other._accountName;
			_profiles = other.Profiles.Values.Select(p => new ProfileDomainObject(p)).ToDictionary(p => p.Name);
			ProfileRepository = other.ProfileRepository;
			EventAggregator = other.EventAggregator;
		}

		public IProfile this[ProfileName profileName]
		{
			get
			{
				return (profileName.IsEmpty || !Profiles.ContainsKey(profileName))
				       	? ProfileSafeNull.Instance
				       	: Profiles[profileName];
			}
		}

		IProfileReadonly IProfileCollectionReadonly.this[ProfileName profileName]
		{
			get { return this[profileName]; }
		}

		public IProfile Add(ProfileCreationArgs profileCreationArgs)
		{
			ProfileDomainObject newProfile = ProfileRepository.Add(profileCreationArgs, _accountName);
			Profiles.Add(newProfile.Name, newProfile);
			RaiseProfileCollectionChanged();
			return newProfile;
		}

		public void Remove(IProfile profile)
		{
			ProfileRepository.Delete(profile.Name, _accountName);
			Profiles.Remove(profile.Name);
			RaiseProfileCollectionChanged();
		}

		private void RaiseProfileCollectionChanged()
		{
			EventAggregator.Get<Event<ProfileCollectionChanged>>().Raise(new ProfileCollectionChanged(this, _accountName));
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<IProfile> GetEnumerator()
		{
			return Profiles.Values.GetEnumerator();
		}

		IEnumerator<IProfileReadonly> IEnumerable<IProfileReadonly>.GetEnumerator()
		{
			return Profiles.Values.GetEnumerator();
		}

		public IEventAggregator EventAggregator
		{
			private get; set;
		}

		public IProfileRepository ProfileRepository
		{
			private get; set;
		}

		private IDictionary<ProfileName, ProfileDomainObject> Profiles
		{
			get { return _profiles; }
		}
	}
}