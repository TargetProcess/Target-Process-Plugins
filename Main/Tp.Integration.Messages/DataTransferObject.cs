using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Tp.Integration.Common
{
	/// <summary>
	/// It is a basic class for all DTOs in system
	/// </summary>
	[Serializable]
	[DataContract]
	public class DataTransferObject : IDataTransferObject
	{
		/// <summary>
		/// Gets or sets the ID.
		/// </summary>
		/// <value>The ID.</value>
		[XmlElement(Order = 0)]
		[DataMember(IsRequired = true)]
		public virtual int? ID { get; set; }

		public virtual T Accept<T>(IDTOVisitor<T> visitor)
		{
			return visitor.VisitDataTransferObject(this);
		}

		public override string ToString()
		{
			return string.Format("{0} ID : {1}", GetType(), ID);
		}
	}
}
