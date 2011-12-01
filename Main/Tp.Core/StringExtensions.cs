// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Linq;
using System.Text;

namespace System
{
	public static class StringExtensions
	{
		public static string Fmt(this string format, params object[] args)
		{
			return String.Format(format, args);
		}

		public static bool IsNullOrEmpty(this string format)
		{
			return String.IsNullOrEmpty(format);
		}

		public static bool IsNullOrWhitespace(this string value)
		{
			return value==null || value.Trim().Length == 0;
		}

		public static StringBuilder AppendLine(this StringBuilder stringBuilder, string format, params object[] args)
		{
			return stringBuilder.AppendFormat(format, args).AppendLine();
		}

		public static T ParseEnum<T>(this string name)
			where T : struct
		{
			var type = typeof (T);
			if (!type.IsEnum)
				throw new ArgumentException("Type argument must be enum");
			try
			{
				return (T)Enum.Parse(type, name, true);
			}
			catch (Exception e)
			{
				throw new InvalidCastException("Could not parse value '{0}' for enum '{1}'".Fmt(name, typeof(T)), e);
			}
		}

		public static bool TryParseEnum<T>(this string name, out T result)
			where T : struct
		{
			result = default(T);
			var type = typeof (T);
			if (!type.IsEnum || !Enum.GetNames(type).Contains(name, StringComparer.OrdinalIgnoreCase))
				return false;

			result = (T) Enum.Parse(type, name, true);

			return true;
		}
	}
}