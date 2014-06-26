using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using Tp.Integration.Common;

namespace Tp.Integration.Common
{
	[Serializable]
	[DataContract]
	public partial class ProcessAdminDTO : DataTransferObject
	{
		[PrimaryKey]
		public override int? ID
		{
			get { return ProcessAdminID; }
			set
			{
				if (value == int.MinValue)
					value = null;

				ProcessAdminID = value;
			}
		}

		[PrimaryKey]
		[DataMember]
		[XmlElement(Order = 1)]
		public int? ProcessAdminID { get; set; }

		[ForeignKey]
		[DataMember]
		[XmlElement(Order = 2)]
		public Int32? UserID { get; set; }

		[ForeignKey]
		[DataMember]
		[XmlElement(Order = 3)]
		public Int32? ProcessID { get; set; }
	}

	public enum ProcessAdminField
	{
		UserID,
		ProcessID
	}
}
