//
// THIS FILE IS AUTOGENERATED! ANY MANUAL MODIFICATIONS WILL BE LOST!
//

using System;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using Tp.Integration.Common;
using Tp.Integration.Messages.Entities;

namespace Tp.Integration.Common
{
	// Autogenerated from License.hbm.xml properties: LicenseID: Int32?, LicenseKey: String, LastUpdateDate: DateTime?, Content: byte[]
	public partial interface ILicenseDTO : IDataTransferObject
	{
		String LicenseKey { get; set; }
		DateTime? LastUpdateDate { get; set; }
		byte[] Content { get; set; }
	}

	[Serializable]
	[DataContract]
	public partial class LicenseDTO : DataTransferObject, ILicenseDTO
	{
		[PrimaryKey]
		public override int? ID
		{
			get { return LicenseID; }
			set
			{
				if (value == int.MinValue)
					value = null;

				LicenseID = value;
			}
		}
		[PrimaryKey]
		[DataMember]
		[XmlElement(Order=3)]
		public Int32? LicenseID { get; set; }

		
		[DataMember]
		[XmlElement(Order=4)]
		public String LicenseKey { get; set; }

		
		[DataMember]
		[XmlElement(Order=5)]
		public DateTime? LastUpdateDate { get; set; }

		
		[DataMember]
		[XmlElement(Order=6)]
		public byte[] Content { get; set; }
	}

	public enum LicenseField
	{
		LicenseKey,
		LastUpdateDate,
		Content,
	}
}
