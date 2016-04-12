// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;

namespace Tp.Integration.Plugin.Common.Activity
{
	public class ActivityDto
	{
		public IEnumerable<ActivityLogRecord> Records { get; set; }

		public ActivityType Type { get; set; }
	}
}