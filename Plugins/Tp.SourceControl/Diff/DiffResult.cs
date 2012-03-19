// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Tp.SourceControl.Diff
{
	[DataContract]
	public class DiffResult
	{
		private readonly List<DiffLineData> _leftPan = new List<DiffLineData>();
		private readonly List<DiffLineData> _rightPan = new List<DiffLineData>();

		[DataMember]
		public string LeftPanRevisionId { get; set; }

		[DataMember]
		public List<DiffLineData> LeftPan
		{
			get { return _leftPan; }
		}

		[DataMember]
		public string RightPanRevisionId { get; set; }

		[DataMember]
		public List<DiffLineData> RightPan
		{
			get { return _rightPan; }
		}
	}
}