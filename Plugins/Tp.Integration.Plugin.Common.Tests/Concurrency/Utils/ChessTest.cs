// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Reflection;
using Tp.Integration.Plugin.Common.Tests.Common;

namespace Tp.Integration.Plugin.Common.Tests.Concurrency.Utils
{
	public class ChessTest : SqlPersisterSpecBase
	{
		private static SqlPersisterSpecBase _instance;
		private static MethodInfo _methodInfo;

		public static bool Startup(string[] args)
		{
			return Chess.Run(() =>
			              	{
			              		var fullTypeName = args[0];
			              		var methodName = args[1];
			              		var type = Type.GetType(fullTypeName);
			              		_instance = (SqlPersisterSpecBase) Activator.CreateInstance(type);
			              		_methodInfo = type.GetMethod(methodName);
			              	});
		}

		public static bool Run()
		{
			return Chess.Run(() =>
			              	{
			              		_instance.Init();
			              		_methodInfo.Invoke(_instance, new object[0]);
								_instance.Destroy();
			              	});
		}

		public static bool Shutdown()
		{
			return true;
		}
	}
}