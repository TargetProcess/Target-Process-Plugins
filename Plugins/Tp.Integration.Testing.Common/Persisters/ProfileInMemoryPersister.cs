using System;
using System.Collections.Generic;
using System.Linq;
using Tp.Integration.Messages;
using Tp.Integration.Plugin.Common.Storage.Persisters;

namespace Tp.Integration.Testing.Common.Persisters
{
    public class ProfileInMemoryPersister : IProfilePersister
    {
        private readonly Dictionary<AccountName, List<Profile>> _profiles = new Dictionary<AccountName, List<Profile>>();
        private int _id;
        private readonly Func<int> Id;

        public ProfileInMemoryPersister()
        {
            Id = () => _id++;
        }

        public void Delete(ProfileName profileName, AccountName accountName)
        {
            _profiles[accountName].RemoveAll(x => x.Name == profileName);
        }

        public Profile Add(ProfileName profileName, bool initialized, AccountName accountName)
        {
            var profile = new Profile { Name = profileName.Value, Id = Id(), Initialized = initialized };
            if (_profiles.ContainsKey(accountName))
            {
                _profiles[accountName].Add(profile);
            }
            else
            {
                _profiles.Add(accountName, new List<Profile> { profile });
            }
            return profile;
        }

        public Profile Update(ProfileName profileName, bool initialized, AccountName accountName)
        {
            var index = _profiles[accountName].FindIndex(x => x.Name == profileName.Value);
            _profiles[accountName][index].Initialized = initialized;
            _profiles[accountName][index].Name = profileName.Value;
            return _profiles[accountName][index];
        }

        public IEnumerable<Profile> GetAll(AccountName accountName)
        {
            if (_profiles.ContainsKey(accountName))
            {
                return _profiles[accountName];
            }

            return Enumerable.Empty<Profile>();
        }
    }
}
