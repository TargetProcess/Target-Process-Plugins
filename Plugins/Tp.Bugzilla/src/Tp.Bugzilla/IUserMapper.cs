// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

namespace Tp.Bugzilla
{
	public interface IUserMapper
	{
		int? GetTpIdBy(string bugzillaEmail);

		string GetBugzillaEmailBy(int? tpUserId);
	}
}