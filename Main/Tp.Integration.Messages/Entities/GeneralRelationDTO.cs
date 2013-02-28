using System;
using System.Xml.Serialization;

namespace Tp.Integration.Common
{
	[Serializable]
	public class GeneralRelationDTO : DataTransferObject
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

		[XmlElement(Order = 1)]
		public int? GeneralRelationID { get; set; }

		[XmlElement(Order = 2)]
		public int? MasterID { get; set; }

		[XmlElement(Order = 3)]
		public int? SlaveID { get; set; }

		[XmlElement(Order = 4)]
		public int? GeneralRelationTypeID { get; set; }
	}

	public enum GeneralRelationField
	{
		MasterID,
		SlaveID,
		GeneralReltionTypeID,
	}
}