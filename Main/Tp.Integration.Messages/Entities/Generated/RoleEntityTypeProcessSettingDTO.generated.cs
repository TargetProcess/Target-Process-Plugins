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
	// Autogenerated from RoleEntityTypeProcessSetting.hbm.xml properties: RoleEntityTypeProcessSettingID: Int32?, CanBeAssigned: Boolean?, EntityTypeID: int?, EntityTypeName: string, RoleID: int?, RoleName: string, ProcessID: int?, ProcessName: string
	public partial interface IRoleEntityTypeProcessSettingDTO : IDataTransferObject
	{
		Boolean? CanBeAssigned { get; set; }
		int? EntityTypeID { get; set; }
		string EntityTypeName { get; set; }
		int? RoleID { get; set; }
		string RoleName { get; set; }
		int? ProcessID { get; set; }
		string ProcessName { get; set; }
	}

	[Serializable]
	[DataContract]
	public partial class RoleEntityTypeProcessSettingDTO : DataTransferObject, IRoleEntityTypeProcessSettingDTO
	{
		[PrimaryKey]
		public override int? ID
		{
			get { return RoleEntityTypeProcessSettingID; }
			set
			{
				if (value == int.MinValue)
					value = null;

				RoleEntityTypeProcessSettingID = value;
			}
		}
		[PrimaryKey]
		[DataMember]
		[XmlElement(Order=1)]
		public Int32? RoleEntityTypeProcessSettingID { get; set; }

		
		[DataMember]
		[XmlElement(Order=2)]
		public Boolean? CanBeAssigned { get; set; }

		[ForeignKey]
		[DataMember]
		[XmlElement(Order=3)]
		public int? ProcessID { get; set; }

		[ForeignKey]
		[DataMember]
		[XmlElement(Order=4)]
		public int? EntityTypeID { get; set; }

		[ForeignKey]
		[DataMember]
		[XmlElement(Order=5)]
		public int? RoleID { get; set; }

		[RelationName]
		[DataMember]
		[XmlElement(Order=6)]
		public string EntityTypeName { get; set; }

		[RelationName]
		[DataMember]
		[XmlElement(Order=7)]
		public string RoleName { get; set; }

		[RelationName]
		[DataMember]
		[XmlElement(Order=8)]
		public string ProcessName { get; set; }
	}

	public enum RoleEntityTypeProcessSettingField
	{
		CanBeAssigned,
		ProcessID,
		EntityTypeID,
		RoleID,
		EntityTypeName,
		RoleName,
		ProcessName,
	}
}
