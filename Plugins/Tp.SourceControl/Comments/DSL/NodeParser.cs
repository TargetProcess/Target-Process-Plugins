// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Globalization;

namespace Tp.SourceControl.Comments.DSL
{
	public static class NodeParser
	{
		public static int GetEntityId(object value)
		{
			return Convert.ToInt32(value, CultureInfo.InvariantCulture);
		}

		public static decimal GetTime(object value)
		{
			return Convert.ToDecimal(value.ToString().Replace(",", "."), CultureInfo.InvariantCulture);
		}

		public static string GetStatus(object value)
		{
			return Convert.ToString(value).Trim().Trim(',');
		}

		public static string GetCommentSegment(object value)
		{
			return Convert.ToString(value).Trim();
		}
	}
}