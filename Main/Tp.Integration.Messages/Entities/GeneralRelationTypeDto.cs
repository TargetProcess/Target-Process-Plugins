using System;
using System.Xml.Serialization;using System.Runtime.Serialization;

namespace Tp.Integration.Common
{
	[Serializable][DataContract]
	public partial class GeneralRelationTypeDTO : DataTransferObject
	{
		[PrimaryKey]
		public override int? ID
		{
			get { return GeneralRelationTypeID; }
			set
			{
				if (value == int.MinValue)
					value = null;

				GeneralRelationTypeID = value;
			}
		}

		[DataMember][XmlElement(Order = 2)]
		public string Name { get; set; }

		[DataMember][XmlElement(Order = 1)]
		public int? GeneralRelationTypeID { get; set; }
	}

	public enum GeneralRelationTypeField
	{
		Name
	}
}