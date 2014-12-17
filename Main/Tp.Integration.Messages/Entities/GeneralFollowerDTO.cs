using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Tp.Integration.Common
{
	[Serializable]
	[DataContract]
	public partial class GeneralFollowerDTO : DataTransferObject
	{
		[PrimaryKey]
		public override int? ID
		{
			get
			{
				return GeneralFollowerID;
			}
			set
			{
				if (value == int.MinValue)
					value = null;

				GeneralFollowerID = value;
			}
		}

		[DataMember]
		[XmlElement(Order = 1)]
		public int? GeneralFollowerID { get; set; }

		[DataMember]
		[XmlElement(Order = 2)]
		public int? UserID { get; set; }

		[DataMember]
		[XmlElement(Order = 3)]
		public int? GeneralID { get; set; }
	}

	public enum GeneralFollowerField
	{
		UserID,
		GeneralID
	}
}