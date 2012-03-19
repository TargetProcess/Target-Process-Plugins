using System;
using System.Xml.Serialization;

namespace Tp.Integration.Common
{
	[Serializable]
	public class LookupFilterDto
	{
		private Int32? _id;
		private string _name;
		private string _state;
		private int[] _entityTypeIds;
		private int[] _projectIds;
		private int? _releaseIterationId;
		private int? _excludeEntityId;

		[XmlElement(Order = 3)]public int? ID
		{
			get { return _id; }
			set { _id = value; }
		}

		[XmlElement(Order = 4)]public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		[XmlElement(Order = 5)]public string State
		{
			get { return _state; }
			set { _state = value; }
		}

		[XmlElement(Order = 6)]public int[] EntityTypeIds
		{
			get { return _entityTypeIds; }
			set { _entityTypeIds = value; }
		}

		[XmlElement(Order = 7)]public int[] ProjectIds
		{
			get { return _projectIds; }
			set { _projectIds = value; }
		}

		[XmlElement(Order = 8)]public int? ReleaseIterationId
		{
			get { return _releaseIterationId; }
			set { _releaseIterationId = value; }
		}

		[XmlElement(Order = 9)]public int? ExcludeEntityId
		{
			get { return _excludeEntityId; }
			set { _excludeEntityId = value; }
		}
	}
}
