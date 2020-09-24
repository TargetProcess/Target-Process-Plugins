using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.Entities.PermissionGroups
{
    [Serializable]
    [DataContract]
    public class AclEntityPermissionGroupsDTO : DataTransferObject, IEntityPermissionGroupsDTO
    {
        [DataMember]
        [XmlElement(Order = 0)]
        public int EntityID { get; set; }

        [DataMember]
        [XmlElement(Order = 1)]
        public string ResourceType { get; set; }

        [DataMember]
        [XmlElement(Order = 2)]
        public string[] Groups { get; set; }

        [DataMember]
        [XmlElement(Order = 3)]
        public string Error { get; set; }

        [DataMember]
        [XmlElement(Order = 4)]
        public int? ResourceTypeId { get; set; }

        public enum AclEntityPermissionsGroups
        {
            EntityID,
            ResourceType,
            Groups,
            Error,
            ResourceTypeId
        }
    }
}
