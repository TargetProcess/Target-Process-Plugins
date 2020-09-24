using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.Entities.PermissionGroups
{
    [Serializable]
    [DataContract]
    public class AclUserPermissionGroupsDTO : DataTransferObject, IUserPermissionGroupsDTO
    {
        [DataMember]
        [XmlElement(Order = 0)]
        public int UserId { get; set; }

        [DataMember]
        [XmlElement(Order = 1)]
        public string[] Groups { get; set; }

        [DataMember]
        [XmlElement(Order = 2)]
        public string Error { get; set; }

        public enum AclUserPermissionGroups { UserId, Groups, Error }
    }
}
