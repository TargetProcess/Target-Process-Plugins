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
	// Autogenerated from FreezedTestCaseInfo.hbm.xml properties: FreezedTestCaseInfoID: Int32?, Name: string, Description: string, NumericPriority: Double?, OriginTestCaseID: int?, OriginTestCaseName: string
	public partial interface IFreezedTestCaseInfoDTO : IDataTransferObject
	{
		string Name { get; set; }
		string Description { get; set; }
		Double? NumericPriority { get; set; }
		int? OriginTestCaseID { get; set; }
		string OriginTestCaseName { get; set; }
	}

	[Serializable]
	[DataContract]
	public partial class FreezedTestCaseInfoDTO : DataTransferObject, IFreezedTestCaseInfoDTO
	{
		[PrimaryKey]
		public override int? ID
		{
			get { return FreezedTestCaseInfoID; }
			set
			{
				if (value == int.MinValue)
					value = null;

				FreezedTestCaseInfoID = value;
			}
		}
		[PrimaryKey]
		[DataMember]
		[XmlElement(Order=1)]
		public Int32? FreezedTestCaseInfoID { get; set; }

		
		[DataMember]
		[XmlElement(Order=2)]
		public string Name { get; set; }

		
		[DataMember]
		[XmlElement(Order=3)]
		public string Description { get; set; }

		
		[DataMember]
		[XmlElement(Order=4)]
		public Double? NumericPriority { get; set; }

		[ForeignKey]
		[DataMember]
		[XmlElement(Order=5)]
		public int? OriginTestCaseID { get; set; }

		[RelationName]
		[DataMember]
		[XmlElement(Order=6)]
		public string OriginTestCaseName { get; set; }
	}

	public enum FreezedTestCaseInfoField
	{
		Name,
		Description,
		NumericPriority,
		OriginTestCaseID,
		OriginTestCaseName,
	}
}
