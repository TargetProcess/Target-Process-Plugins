// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;

namespace Tp.SourceControl.Testing.Repository
{
	public interface IVcsCredentials
	{
		Uri Uri { get; }
		string Login { get; }
		string Password { get; }
	}
}