// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Linq;

namespace Tp.Utils
{
	public static class ArrayUtils
	{
		public static int[] Add(int[] a, int[] b)
		{
			return a.Select((x, i) => x + b[i]).ToArray();
		}

		public static int[] Subtract(int[] a, int[] b)
		{
			return a.Select((x, i) => x - b[i]).ToArray();
		}
	}
}