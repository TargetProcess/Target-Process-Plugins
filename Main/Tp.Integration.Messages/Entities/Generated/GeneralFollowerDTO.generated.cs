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
	// Autogenerated from GeneralFollower.hbm.xml properties: GeneralFollowerID: Int32?, GeneralID: int?, GeneralName: string, UserID: int?
	public partial interface IGeneralFollowerDTO : IDataTransferObject
	{
		int? GeneralID { get; set; }
		string GeneralName { get; set; }
		int? UserID { get; set; }
	}

	[Serializable]
	[DataContract]
	public partial class GeneralFollowerDTO : DataTransferObject, IGeneralFollowerDTO
	{
		[PrimaryKey]
		public override int? ID
		{
			get { return GeneralFollowerID; }
			set
			{
				if (value == int.MinValue)
					value = null;

				GeneralFollowerID = value;
			}
		}
		[PrimaryKey]
		[DataMember]
		[XmlElement(Order=1)]
		public Int32? GeneralFollowerID { get; set; }

		[ForeignKey]
		[DataMember]
		[XmlElement(Order=2)]
		public int? UserID { get; set; }

		[ForeignKey]
		[DataMember]
		[XmlElement(Order=3)]
		public int? GeneralID { get; set; }

		[RelationName]
		[DataMember]
		[XmlElement(Order=4)]
		public string GeneralName { get; set; }
	}

	public enum GeneralFollowerField
	{
		UserID,
		GeneralID,
		GeneralName,
	}
}