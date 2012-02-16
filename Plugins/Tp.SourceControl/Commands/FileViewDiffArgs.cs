// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Runtime.Serialization;

namespace Tp.SourceControl.Commands
{
	[DataContract]
	public class FileViewDiffArgs
	{
		[DataMember]
		public int? TpRevisionId { get; set; }

		[DataMember]
		public string Path { get; set; }
	}
}