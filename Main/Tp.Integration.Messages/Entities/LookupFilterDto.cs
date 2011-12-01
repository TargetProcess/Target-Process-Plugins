using System;

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

		public int? ID
		{
			get { return _id; }
			set { _id = value; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public string State
		{
			get { return _state; }
			set { _state = value; }
		}

		public int[] EntityTypeIds
		{
			get { return _entityTypeIds; }
			set { _entityTypeIds = value; }
		}

		public int[] ProjectIds
		{
			get { return _projectIds; }
			set { _projectIds = value; }
		}

		public int? ReleaseIterationId
		{
			get { return _releaseIterationId; }
			set { _releaseIterationId = value; }
		}

		public int? ExcludeEntityId
		{
			get { return _excludeEntityId; }
			set { _excludeEntityId = value; }
		}
	}
}