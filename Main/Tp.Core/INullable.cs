// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
namespace Tp.Core
{
	/// <summary>
	/// Base interface for reference type which has null object case
	/// </summary>
	public interface INullable
	{
		bool IsNull { get; }
	}
}