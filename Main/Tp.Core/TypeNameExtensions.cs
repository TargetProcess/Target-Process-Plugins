// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using System.Text.RegularExpressions;

namespace Tp.Core
{
	public static class TypeNameExtensions
	{
		public static string GetTypeNameWithoutVersion(this Type type)
		{
			return GetTypeNameWithoutVersion(type.AssemblyQualifiedName);
		}

		public static string GetTypeNameWithoutVersion(this string typename)
		{
			typename = Regex.Replace(typename, @", Version=\d+.\d+.\d+.\d+", String.Empty);
			typename = Regex.Replace(typename, @", Culture=\w+", String.Empty);
			return Regex.Replace(typename, @", PublicKeyToken=\w+", String.Empty);
		}
	}
}