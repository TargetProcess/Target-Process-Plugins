using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Tp.Integration.Common
{
	[Serializable]
	[DataContract]
	public partial class EmailAttachmentDTO : DataTransferObject
	{
		[DataMember]
		[XmlElement(Order = 3)]
		public byte[] Buffer { get; set; }

		[DataMember]
		[XmlElement(Order = 4)]
		public string Name { get; set; }
	}
}
