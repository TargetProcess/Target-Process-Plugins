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
	// Autogenerated from SquadProject.hbm.xml properties: SquadProjectID: Int32?, StartDate: DateTime?, EndDate: DateTime?, IsProjectAccessed: Boolean?, IsFullProjectAccess: Boolean?, SquadID: int?, SquadName: string, ProjectID: int?, ProjectName: string
	public partial interface ISquadProjectDTO : IDataTransferObject
	{
		DateTime? StartDate { get; set; }
		DateTime? EndDate { get; set; }
		Boolean? IsProjectAccessed { get; set; }
		Boolean? IsFullProjectAccess { get; set; }
		int? SquadID { get; set; }
		string SquadName { get; set; }
		int? ProjectID { get; set; }
		string ProjectName { get; set; }
	}

	[Serializable]
	[DataContract]
	public partial class SquadProjectDTO : DataTransferObject, ISquadProjectDTO
	{
		[PrimaryKey]
		public override int? ID
		{
			get { return SquadProjectID; }
			set
			{
				if (value == int.MinValue)
					value = null;

				SquadProjectID = value;
			}
		}
		[PrimaryKey]
		[DataMember]
		[XmlElement(Order=1)]
		public Int32? SquadProjectID { get; set; }

		
		[DataMember]
		[XmlElement(Order=2)]
		public DateTime? StartDate { get; set; }

		
		[DataMember]
		[XmlElement(Order=3)]
		public DateTime? EndDate { get; set; }

		
		[DataMember]
		[XmlElement(Order=4)]
		public Boolean? IsProjectAccessed { get; set; }

		
		[DataMember]
		[XmlElement(Order=5)]
		public Boolean? IsFullProjectAccess { get; set; }

		[ForeignKey]
		[DataMember]
		[XmlElement(Order=6)]
		public int? SquadID { get; set; }

		[RelationName]
		[DataMember]
		[XmlElement(Order=7)]
		public string SquadName { get; set; }

		[ForeignKey]
		[DataMember]
		[XmlElement(Order=8)]
		public int? ProjectID { get; set; }

		[RelationName]
		[DataMember]
		[XmlElement(Order=9)]
		public string ProjectName { get; set; }
	}

	public enum SquadProjectField
	{
		StartDate,
		EndDate,
		IsProjectAccessed,
		IsFullProjectAccess,
		SquadID,
		SquadName,
		ProjectID,
		ProjectName,
	}
}
