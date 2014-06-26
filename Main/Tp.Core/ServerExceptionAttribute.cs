// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;

namespace Tp.Core
{
	[AttributeUsage(AttributeTargets.Class)]
	public class ServerExceptionAttribute : Attribute
	{
		public ServerExceptionAttribute(ServerErrorCode exceptionCode)
		{
			ExceptionCode = exceptionCode;
		}

		public ServerErrorCode ExceptionCode { get; private set; }
	}
}