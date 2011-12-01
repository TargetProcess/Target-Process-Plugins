// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

namespace Tp.Core.Csv
{
	public static class CsvExtensions
	{
		public static string CsvEncode(this string source)
		{
			return source.Replace(@"""", @"""""");
		}
	}
}