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
	// Autogenerated from PluginProfileMessage.hbm.xml properties: PluginProfileMessageID: Int32?, CreateDate: DateTime?, Successfull: Boolean?, Message: string, PluginProfileID: Int32?
	public partial interface IPluginProfileMessageDTO : IDataTransferObject
	{
		DateTime? CreateDate { get; set; }
		Boolean? Successfull { get; set; }
		string Message { get; set; }
		Int32? PluginProfileID { get; set; }
	}

	[Serializable]
	[DataContract]
	public partial class PluginProfileMessageDTO : DataTransferObject, IPluginProfileMessageDTO
	{
		[PrimaryKey]
		public override int? ID
		{
			get { return PluginProfileMessageID; }
			set
			{
				if (value == int.MinValue)
					value = null;

				PluginProfileMessageID = value;
			}
		}
		[PrimaryKey]
		[DataMember]
		[XmlElement(Order=3)]
		public Int32? PluginProfileMessageID { get; set; }

		
		[DataMember]
		[XmlElement(Order=4)]
		public DateTime? CreateDate { get; set; }

		
		[DataMember]
		[XmlElement(Order=5)]
		public Boolean? Successfull { get; set; }

		
		[DataMember]
		[XmlElement(Order=6)]
		public string Message { get; set; }

		
		[DataMember]
		[XmlElement(Order=7)]
		public Int32? PluginProfileID { get; set; }
	}

	public enum PluginProfileMessageField
	{
		CreateDate,
		Successfull,
		Message,
		PluginProfileID,
	}
}
