// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Globalization;

namespace Tp.Components
{
	public class Formatter
	{
		/// <summary>
		/// 	Formats decimal string. Cut off "00"
		/// </summary>
		/// <param name = "input">Input string (decimal value)</param>
		/// <returns></returns>
		public static string FormatDecimal(object input)
		{
			if (input == null)
				return "#N/A";

			string output = PerformFormat(input);

			if (output.Length == 1)
				return output;

			string separator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

			while ((output.EndsWith("0") || output.EndsWith(separator)) && output.Length > 1 && output.Contains(separator))
			{
				output = output.EndsWith("0") ? output.Substring(0, output.Length - 1) : output.Substring(0, output.Length - separator.Length);
			}

			return output;
		}

		public static string FormatValue(object value)
		{
			if (value == null)
				return string.Empty;

			if (value is DateTime)
				return ((DateTime) value).ToString("dd-MMM-yyyy");

			if (value is decimal || value is float || value is int || value is Single)
				return FormatDecimal(value);

			return value.ToString();
		}

		private static string PerformFormat(object input)
		{
			// Fix #398. Bug on saving feature on saving with initial Estimate 1000
			// remove group separators
			var groupSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
			return input.ToString().Replace(groupSeparator, "");
		}
	}
}