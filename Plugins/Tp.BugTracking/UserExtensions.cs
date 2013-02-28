// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Common;

namespace Tp.BugTracking
{
	public static class UserExtensions
	{
		public static bool IsNotDeletedUser(this UserDTO user)
		{
			return !user.DeleteDate.HasValue;
		}
	}
}