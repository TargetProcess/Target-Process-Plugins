// 
// Copyright (c) 2005-2008 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
namespace Tp.Integration.Common
{
	/// <summary>
	/// Describes how the request was added
	/// </summary>
	public enum RequestSourceEnum
	{
		/// <summary>
		/// Undefined
		/// </summary>
		None = 0,
		/// <summary>
		/// By email
		/// </summary>
		Email = 1,
		/// <summary>
		/// By phone
		/// </summary>
		Phone = 2,
		/// <summary>
		/// Internally
		/// </summary>
		Internal = 3,
		/// <summary>
		/// Externally
		/// </summary>
		External = 4
	}
}