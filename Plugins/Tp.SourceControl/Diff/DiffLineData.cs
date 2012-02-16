// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Runtime.Serialization;

namespace Tp.SourceControl.Diff
{
	[DataContract]
	public class DiffLineData
	{
		public DiffLineData(string line, DiffActionType action, int lineNumber)
		{
			Line = line.Replace("\t", "    ");
			Action = action;
			LineNumber = lineNumber;
		}

		[DataMember]
		public int LineNumber { get; set; }

		[DataMember]
		public string Line { get; set; }

		[DataMember]
		public DiffActionType Action { get; set; }
	}
}