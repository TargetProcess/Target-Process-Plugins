//  
// Copyright (c) 2005-2009 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;

namespace Tp.Core
{
	public class InvalidDateRangeException : InvalidOperationException
	{
		public InvalidDateRangeException(string message)
			: base(message)
		{
		}
	}
}