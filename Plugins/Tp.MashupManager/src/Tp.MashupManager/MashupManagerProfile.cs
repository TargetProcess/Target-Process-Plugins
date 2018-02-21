using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Tp.Integration.Plugin.Common;
using Tp.MashupManager.MashupLibrary.Repository.Config;

namespace Tp.MashupManager
{
    [Serializable, Profile, DataContract]
    public class MashupManagerProfile
    {
        public MashupManagerProfile()
        {
            MashupNames = new MashupNames();
        }

        [DataMember]
        public MashupNames MashupNames { get; set; }

        [DataMember]
        public LibraryRepositoryConfig[] LibraryRepositoryConfigs { get; set; }
    }

    public static class MashupManagerProfileExtensions
    {
        public static bool ContainsMashupName(this MashupManagerProfile mashupManagerProfile, string mashupName)
        {
            return mashupManagerProfile.MashupNames.Any(name => name.EqualsIgnoreCase(mashupName));
        }

        public static void AddMashups(this MashupManagerProfile mashupManagerProfile, IEnumerable<string> mashupNames)
        {
            mashupNames.ForEach(mashupManagerProfile.AddMashup);
        }

        public static void AddMashup(this MashupManagerProfile mashupManagerProfile, string mashupName)
        {
            mashupManagerProfile.MashupNames.Add(mashupName);
        }

        public static void RemoveMashup(this MashupManagerProfile mashupManagerProfile, string mashupName)
        {
            mashupManagerProfile.MashupNames.Remove(mashupName);
        }
    }

    public class MashupNames : List<string>
    {
    }
}
