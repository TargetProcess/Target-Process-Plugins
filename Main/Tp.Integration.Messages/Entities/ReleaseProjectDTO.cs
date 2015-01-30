using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Tp.Integration.Common
{
	[Serializable]
	[DataContract]
	public partial class ReleaseProjectDTO : DataTransferObject
	{
		[PrimaryKey]
		public override int? ID
		{
			get { return ReleaseProjectID; }
			set
			{
				if (value == int.MinValue)
					value = null;

				ReleaseProjectID = value;
			}
		}

		[PrimaryKey]
		[DataMember]
		[XmlElement(Order = 1)]
		public int? ReleaseProjectID { get; set; }
		
		[ForeignKey]
		[DataMember]
		[XmlElement(Order = 2)]
		public Int32? ProjectID { get; set; }

		[ForeignKey]
		[DataMember]
		[XmlElement(Order = 3)]
		public Int32? ReleaseID { get; set; }
		
		[RelationName]
		[DataMember]
		[XmlElement(Order = 4)]
		public virtual string ProjectName { get; set; }

		[RelationName]
		[DataMember]
		[XmlElement(Order = 5)]
		public virtual string ReleaseName { get; set; }
	}

	public enum ReleaseProjectField
	{
		ProjectID,
		ReleaseID,
		ProjectName,
		ReleaseName,
	}
}