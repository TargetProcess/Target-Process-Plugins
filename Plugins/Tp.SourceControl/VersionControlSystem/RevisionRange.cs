//  
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Runtime.Serialization;

namespace Tp.SourceControl.VersionControlSystem
{
	[Serializable]
	[DataContract]
	public class RevisionRange
	{
		public RevisionRange() {}

		public RevisionRange(RevisionId fromChangeset, RevisionId toChangeset)
		{
			FromChangeset = fromChangeset;
			ToChangeset = toChangeset;
		}

		[DataMember]
		public RevisionId FromChangeset { get; private set; }

		[DataMember]
		public RevisionId ToChangeset { get; private set; }

		public override string ToString()
		{
			return string.Format("{0} - {1}", FromChangeset, ToChangeset);
		}
	}
}