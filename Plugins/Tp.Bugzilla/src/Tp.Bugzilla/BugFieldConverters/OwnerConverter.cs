// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

namespace Tp.Bugzilla.BugFieldConverters
{
	public class OwnerConverter : IBugConverter
	{
		private readonly IUserMapper _userMapper;

		public OwnerConverter(IUserMapper userMapper)
		{
			_userMapper = userMapper;
		}

		public void Apply(BugzillaBug bugzillaBug, ConvertedBug convertedBug)
		{
			convertedBug.BugDto.OwnerID = _userMapper.GetTpIdBy(bugzillaBug.reporter);
		}
	}
}