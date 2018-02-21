using System;
using System.Runtime.Serialization;

namespace Tp.Integration.Messages.PluginLifecycle
{
    [Serializable, DataContract]
    public class PluginProfile
    {
        public PluginProfile()
            : this(string.Empty)
        {
        }

        public PluginProfile(ProfileName name)
        {
            Name = name;
        }

        public ProfileName Name { get; set; }

        [DataMember(Name = "Name")]
        private string NameValue
        {
            get { return Name.Value; }
            set { Name = new ProfileName(value); }
        }
    }
}
