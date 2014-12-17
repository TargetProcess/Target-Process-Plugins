using System;
using System.Xml.Serialization;using System.Runtime.Serialization;

namespace Tp.Integration.Common
{
	[Serializable][DataContract]
	public partial class GeneralRelationDTO : DataTransferObject
	{
		[PrimaryKey]
		public override int? ID
		{
			get
			{
				return GeneralRelationID;
			}
			set
			{
				if (value == int.MinValue)
					value = null;

				GeneralRelationID = value;
			}
		}

		[DataMember][XmlElement(Order = 1)]
		public int? GeneralRelationID { get; set; }

		[DataMember][XmlElement(Order = 2)]
		public int? MasterID { get; set; }

		[DataMember][XmlElement(Order = 3)]
		public int? SlaveID { get; set; }

		[DataMember][XmlElement(Order = 4)]
		public int? GeneralRelationTypeID { get; set; }
	}

	public enum GeneralRelationField
	{
		MasterID,
		SlaveID,
		GeneralReltionTypeID,
	}
}