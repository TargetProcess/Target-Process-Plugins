// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

namespace Tp.Bugzilla.BugFieldConverters
{
	public interface IBugConverter
	{
		void Apply(BugzillaBug bugzillaBug, ConvertedBug convertedBug);
	}
}
