using System;
using System.Xml.Serialization;using System.Runtime.Serialization;

namespace Tp.Integration.Common
{
	[Serializable][DataContract]
	public class LookupFilterDto
	{
		private Int32? _id;
		private string _name;
		private string _state;
		private int[] _entityTypeIds;
		private int[] _projectIds;
		private int? _releaseIterationId;
		private int? _excludeEntityId;

		[DataMember][XmlElement(Order = 3)]public int? ID
		{
			get { return _id; }
			set { _id = value; }
		}

		[DataMember][XmlElement(Order = 4)]public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		[DataMember][XmlElement(Order = 5)]public string State
		{
			get { return _state; }
			set { _state = value; }
		}

		[DataMember][XmlElement(Order = 6)]public int[] EntityTypeIds
		{
			get { return _entityTypeIds; }
			set { _entityTypeIds = value; }
		}

		[DataMember][XmlElement(Order = 7)]public int[] ProjectIds
		{
			get { return _projectIds; }
			set { _projectIds = value; }
		}

		[DataMember][XmlElement(Order = 8)]public int? ReleaseIterationId
		{
			get { return _releaseIterationId; }
			set { _releaseIterationId = value; }
		}

		[DataMember][XmlElement(Order = 9)]public int? ExcludeEntityId
		{
			get { return _excludeEntityId; }
			set { _excludeEntityId = value; }
		}
	}
}
