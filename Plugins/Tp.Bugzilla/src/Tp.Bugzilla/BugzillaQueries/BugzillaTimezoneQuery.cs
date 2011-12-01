// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

namespace Tp.Bugzilla.BugzillaQueries
{
	public class BugzillaTimezoneQuery : IBugzillaQuery
	{
		public string Value()
		{
			return "cmd=get_timezone";
		}

		public string GetOperationDescription()
		{
			return "Get bugzilla timezone";
		}
	}
}