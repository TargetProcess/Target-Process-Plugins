// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Runtime.Serialization;

namespace Tp.SourceControl.RevisionStorage
{
	[DataContract]
	public class TargetProcessRevisionId
	{
		[DataMember]
		public int? Value { get; set; }
	}
}