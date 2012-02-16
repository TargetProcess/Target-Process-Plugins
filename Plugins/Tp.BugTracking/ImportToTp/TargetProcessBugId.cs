// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Runtime.Serialization;

namespace Tp.BugTracking.ImportToTp
{
	[DataContract]
	public class TargetProcessBugId
	{
		[DataMember]
		public int Value { get; set; }
	}
}