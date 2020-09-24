using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Tp.Integration.Common
{
    // TODO: Remove after completition of US#183198
    [Serializable]
    [DataContract]
    public class AclEntityPermissionGroupsTempDTO : DataTransferObject, IAclEntityPermissionGroupsTempDTO
    {
        [DataMember]
        [XmlElement(Order = 0)]
        public int EntityID { get; set; }

        [DataMember]
        [XmlElement(Order = 1)]
        public int EntityTypeId { get; set; }

        public enum AclEntityPermissionGroupsTemp { EntityID, EntityTypeId }
    }

    public interface IAclEntityPermissionGroupsTempDTO : IDataTransferObject{}
}
