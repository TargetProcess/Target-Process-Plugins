//  
// Copyright (c) 2005-2009 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Reflection;

namespace Tp.Core.Extensions
{
	public static class Objects
	{
		/// <summary>
		/// Creates a shallow copy of specified object (ONLY PROPERTIES ARE SUPPORTED)
		/// </summary>
		/// <remarks><b>Only properties supported</b></remarks>
		/// <param name="original">Specified object to clone</param>
		/// <returns>Object clone</returns>
		public static object CloneObject(this object original)
		{
			Type type = original.GetType();
			PropertyInfo[] properties = type.GetProperties();

			Object clone = type.InvokeMember("", BindingFlags.CreateInstance, null, original, null);

			foreach (PropertyInfo pi in properties)
			{
				if (pi.CanWrite)
				{
					pi.SetValue(clone, pi.GetValue(original, null), null);
				}
			}

			return clone;
		}
	}
}