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
	// Autogenerated from TpProjectProfile.hbm.xml properties: ProjectProfileID: Int32?, PropertyName: String, PropertyValue: string, ProjectID: int?, ProjectName: string
	public partial interface ITpProjectProfileDTO : IDataTransferObject
	{
		String PropertyName { get; set; }
		string PropertyValue { get; set; }
		int? ProjectID { get; set; }
		string ProjectName { get; set; }
	}

	[Serializable]
	[DataContract]
	public partial class TpProjectProfileDTO : DataTransferObject, ITpProjectProfileDTO
	{
		[PrimaryKey]
		public override int? ID
		{
			get { return ProjectProfileID; }
			set
			{
				if (value == int.MinValue)
					value = null;

				ProjectProfileID = value;
			}
		}
		[PrimaryKey]
		[DataMember]
		[XmlElement(Order=3)]
		public Int32? ProjectProfileID { get; set; }

		
		[DataMember]
		[XmlElement(Order=4)]
		public String PropertyName { get; set; }

		
		[DataMember]
		[XmlElement(Order=5)]
		public string PropertyValue { get; set; }

		[ForeignKey]
		[DataMember]
		[XmlElement(Order=6)]
		public int? ProjectID { get; set; }

		[RelationName]
		[DataMember]
		[XmlElement(Order=7)]
		public string ProjectName { get; set; }
	}

	public enum TpProjectProfileField
	{
		PropertyName,
		PropertyValue,
		ProjectID,
		ProjectName,
	}
}
