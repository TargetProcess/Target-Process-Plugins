using System;
using System.Runtime.Serialization;

namespace Tp.Integration.Messages.PluginLifecycle
{
    [DataContract, Serializable]
    public class MashupMetaInfo
    {
        public MashupMetaInfo()
        {
            IsEnabled = true;
        }

        [DataMember]
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Set to original package name when mashup was installed from library.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public string PackageName { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public ulong CreationDate { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public MashupUserInfo CreatedBy { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public ulong LastModificationDate { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public MashupUserInfo LastModifiedBy { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Publisher { get; set; }
    }

    [DataContract, Serializable]
    public class MashupUserInfo
    {
        [DataMember(EmitDefaultValue = false)]
        public int Id { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Name { get; set; }
    }
}
