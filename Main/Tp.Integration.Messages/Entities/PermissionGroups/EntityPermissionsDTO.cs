using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.Entities.PermissionGroups
{
    [Serializable]
    [DataContract]
    public class EntityPermissionGroupsDTO : DataTransferObject
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
    }

    public enum EntityPermissionGroupsField { EntityID, ResourceType, Groups, Error }
}
