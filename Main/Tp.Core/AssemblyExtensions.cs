// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.IO;
using System.Reflection;

namespace Tp.Core
{
	public static class AssemblyExtensions
	{
		public static DateTime GetUtcBuildDate(this Assembly assembly)
		{
			var PeHeaderOffset = 60;
			var LinkerTimestampOffset = 8;

			var b = new byte[2048];
			using (var s = new FileStream(assembly.Location, FileMode.Open, FileAccess.Read))
			{
				s.Read(b, 0, 2048);
			}
			var i = BitConverter.ToInt32(b, PeHeaderOffset);
			var seconds = BitConverter.ToInt32(b, i + LinkerTimestampOffset);
			return new DateTime(1970, 1, 1).AddSeconds(seconds);
		}
	}
}