// 
// Copyright (c) 2005-2014 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Globalization;
using System.IO;

namespace Tp.MashupManager
{
	public static class PathExtensions
	{
		public static string GetRelativePathTo(this string path, string baseDirectoryPath)
		{
			if (!baseDirectoryPath.EndsWith(Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture)))
			{
				baseDirectoryPath += Path.DirectorySeparatorChar;
			}

			return Uri.UnescapeDataString(
				new Uri(baseDirectoryPath).MakeRelativeUri(new Uri(path))
					.ToString().Replace('/', Path.DirectorySeparatorChar)
				);
		}
	}
}