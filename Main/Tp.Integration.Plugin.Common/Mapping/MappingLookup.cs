// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Runtime.Serialization;

namespace Tp.Integration.Plugin.Common.Mapping
{
	[DataContract]
	public class MappingLookup
	{
		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Name { get; set; }

		public override bool Equals(object obj)
		{
			return Equals(obj as MappingLookup);
		}

		public bool Equals(MappingLookup other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}

			return other.Id == Id;
		}

		public override int GetHashCode()
		{
			return Id;
		}
	}
}