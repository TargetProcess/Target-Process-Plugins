// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

namespace Tp.BugTracking.ConnectionValidators
{
	public interface IDataHolder<T>
	{
		T Data { get; }
	}
}