// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Runtime.Serialization;

namespace Tp.Tfs.WorkItemsIntegration
{
	[Serializable]
	[DataContract]
	public class WorkItemId
	{
		[DataMember]
		public string Id { get; set; }

		public override bool Equals(object obj)
		{
			return Equals(obj as WorkItemId);
		}

		public bool Equals(WorkItemId other)
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
			return Int32.Parse(Id);
		}
	}
}
