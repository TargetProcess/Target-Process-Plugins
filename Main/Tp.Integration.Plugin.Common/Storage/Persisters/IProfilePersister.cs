using System.Collections.Generic;
using Tp.Integration.Messages;

namespace Tp.Integration.Plugin.Common.Storage.Persisters
{
    public interface IProfilePersister
    {
        void Delete(ProfileName profileName, AccountName accountName);
        IEnumerable<Profile> GetAll(AccountName accountName);
        Profile Add(ProfileName profileName, bool initialized, AccountName accountName);
        Profile Update(ProfileName profileName, bool initialized, AccountName accountName);
    }
}
