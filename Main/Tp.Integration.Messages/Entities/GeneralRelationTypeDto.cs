using System;
using System.Xml.Serialization;

namespace Tp.Integration.Common
{
	[Serializable]
	public class GeneralRelationTypeDTO : DataTransferObject
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

		[XmlElement(Order = 2)]
		public string Name { get; set; }

		[XmlElement(Order = 1)]
		public int? GeneralRelationTypeID { get; set; }
	}

	public enum GeneralRelationTypeField
	{
		Name
	}
}