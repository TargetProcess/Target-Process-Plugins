// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using Tp.Integration.Common;

namespace Tp.BugTracking.BugFieldConverters
{
	[Serializable]
	public class ConvertedBug
	{
		public ConvertedBug()
		{
			BugDto = new BugDTO();
			ChangedFields = new List<Enum>();
		}

		public BugDTO BugDto { get; set; }
		public List<Enum> ChangedFields { get; set; }
	}
}