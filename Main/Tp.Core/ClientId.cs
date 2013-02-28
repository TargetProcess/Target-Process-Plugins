// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Runtime.Remoting.Messaging;

namespace Tp.Core
{
	public static class ClientId
	{
		private const string KEY = "clientId";

		public static void Set(string value)
		{
			CallContext.LogicalSetData(KEY, value);
		}

		public static string Get()
		{
			return CallContext.LogicalGetData(KEY) as string;
		}
	}
}