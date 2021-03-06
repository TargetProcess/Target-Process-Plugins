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
	// Autogenerated from Workflow.hbm.xml properties: WorkflowID: Int32?, Name: String, ProcessID: int?, ProcessName: string, EntityTypeID: int?, EntityTypeName: string, ParentWorkflowID: int?
	public partial interface IWorkflowDTO : IDataTransferObject
	{
		String Name { get; set; }
		int? ProcessID { get; set; }
		string ProcessName { get; set; }
		int? EntityTypeID { get; set; }
		string EntityTypeName { get; set; }
		int? ParentWorkflowID { get; set; }
	}

	[Serializable]
	[DataContract]
	public partial class WorkflowDTO : DataTransferObject, IWorkflowDTO
	{
		[PrimaryKey]
		public override int? ID
		{
			get { return WorkflowID; }
			set
			{
				if (value == int.MinValue)
					value = null;

				WorkflowID = value;
			}
		}
		[PrimaryKey]
		[DataMember]
		[XmlElement(Order=1)]
		public Int32? WorkflowID { get; set; }

		
		[DataMember]
		[XmlElement(Order=2)]
		public String Name { get; set; }

		[ForeignKey]
		[DataMember]
		[XmlElement(Order=3)]
		public int? ProcessID { get; set; }

		[RelationName]
		[DataMember]
		[XmlElement(Order=4)]
		public string ProcessName { get; set; }

		[ForeignKey]
		[DataMember]
		[XmlElement(Order=5)]
		public int? EntityTypeID { get; set; }

		[RelationName]
		[DataMember]
		[XmlElement(Order=6)]
		public string EntityTypeName { get; set; }

		[ForeignKey]
		[DataMember]
		[XmlElement(Order=7)]
		public int? ParentWorkflowID { get; set; }
	}

	public enum WorkflowField
	{
		Name,
		ProcessID,
		ProcessName,
		EntityTypeID,
		EntityTypeName,
		ParentWorkflowID,
	}
}
