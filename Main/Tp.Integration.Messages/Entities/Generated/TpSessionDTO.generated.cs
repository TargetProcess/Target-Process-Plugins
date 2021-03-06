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
	// Autogenerated from TpSession.hbm.xml properties: TpSessionID: Int32?, IsCompressed: Boolean?, ObjKey: String, Buffer: byte[], ModifyDate: DateTime?, OwnerID: int?, UserID: int?
	public partial interface ITpSessionDTO : IDataTransferObject
	{
		Boolean? IsCompressed { get; set; }
		String ObjKey { get; set; }
		byte[] Buffer { get; set; }
		DateTime? ModifyDate { get; set; }
		int? OwnerID { get; set; }
		int? UserID { get; set; }
	}

	[Serializable]
	[DataContract]
	public partial class TpSessionDTO : DataTransferObject, ITpSessionDTO
	{
		[PrimaryKey]
		public override int? ID
		{
			get { return TpSessionID; }
			set
			{
				if (value == int.MinValue)
					value = null;

				TpSessionID = value;
			}
		}
		[PrimaryKey]
		[DataMember]
		[XmlElement(Order=3)]
		public Int32? TpSessionID { get; set; }

		[ForeignKey]
		[DataMember]
		[XmlElement(Order=4)]
		public int? UserID { get; set; }

		
		[DataMember]
		[XmlElement(Order=5)]
		public Boolean? IsCompressed { get; set; }

		
		[DataMember]
		[XmlElement(Order=6)]
		public String ObjKey { get; set; }

		
		[DataMember]
		[XmlElement(Order=7)]
		public byte[] Buffer { get; set; }

		
		[DataMember]
		[XmlElement(Order=8)]
		public DateTime? ModifyDate { get; set; }

		[ForeignKey]
		[DataMember]
		[XmlElement(Order=9)]
		public int? OwnerID { get; set; }
	}

	public enum TpSessionField
	{
		UserID,
		IsCompressed,
		ObjKey,
		Buffer,
		ModifyDate,
		OwnerID,
	}
}
