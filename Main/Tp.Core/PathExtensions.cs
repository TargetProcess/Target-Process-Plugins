// 
// Copyright (c) 2005-2015 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.IO;

namespace Tp.Core
{
	public static class PathExtensions
	{
		// Windows API definitions
		public const int MAX_PATH = 260;  // From WinDef.h 

		public static string Combine(this string source, string path)
		{
			return Path.Combine(source, path);
		}

		public static string GetFileName(this string source)
		{
			return Path.GetFileName(source);
		}

		public static string GetFileNameWithoutExtension(this string source)
		{
			return Path.GetFileNameWithoutExtension(source);
		}

		public static string GetDirectoryName(this string source)
		{
			return Path.GetDirectoryName(source);
		}

		public static string GetExtension(this string source)
		{
			return Path.GetExtension(source);
		}
	}
}