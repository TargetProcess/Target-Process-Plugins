// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

namespace Tp.Bugzilla.BugzillaQueries
{
	public class BugzillaAssigneeAction : IBugzillaAction
	{
		private readonly string _bugzillaBugId;
		private readonly string _userEmail;

		public BugzillaAssigneeAction(string bugzillaBugId, string userEmail)
		{
			_bugzillaBugId = bugzillaBugId;
			_userEmail = userEmail;
		}
		
		public string Value()
		{
			return string.Format("cmd=assign_user&bugid={0}&user={1}", _bugzillaBugId, _userEmail);
		}

		public string GetOperationDescription()
		{
			return string.Format("Assign user '{0}' to bug with id '{1}'", _userEmail, _bugzillaBugId);
		}
	}
}