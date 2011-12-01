// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Common;

namespace Tp.Bugzilla
{
	public static class UserExtension
	{
		public static bool IsActiveNotDeletedUser(this UserDTO user)
		{
			return user.IsActive == true && !user.DeleteDate.HasValue;
		}
	}
}