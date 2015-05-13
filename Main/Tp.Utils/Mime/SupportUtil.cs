// 
// Copyright (c) 2005-2008 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using System.Collections;
using System.Reflection;
using System.Text;

namespace Tp.Utils.Mime
{
	public class SupportUtil
	{
		/// <summary>
		/// Converts a string to an array of bytes
		/// </summary>
		/// <param name="sourceString">The string to be converted</param>
		/// <returns>The new array of bytes</returns>
		public static byte[] ToByteArray(String sourceString)
		{
			return Encoding.UTF8.GetBytes(sourceString);
		}

		/// <summary>
		/// Converts a string to an array of bytes
		/// </summary>
		/// <param name="sourceString">The string to be converted</param>
		/// <returns>The new array of bytes</returns>
		public static byte[] ToByteArrayWithDefaultCodePage(String sourceString)
		{
			return Encoding.Default.GetBytes(sourceString);
		}

		/// <summary>
		/// Converts a array of object-type instances to a byte-type array.
		/// </summary>
		/// <param name="tempObjectArray">Array to convert.</param>
		/// <returns>An array of byte type elements.</returns>
		public static byte[] ToByteArray(Object[] tempObjectArray)
		{
			byte[] byteArray = null;
			if (tempObjectArray != null)
			{
				byteArray = new byte[tempObjectArray.Length];
				for (int index = 0; index < tempObjectArray.Length; index++)
				{
					byteArray[index] = (byte) tempObjectArray[index];
				}
			}
			return byteArray;
		}

		/// <summary>
		/// Obtains an array containing all the elements of the collection.
		/// </summary>
		/// <param name="c"></param>
		/// <param name="objects">The array into which the elements of the collection will be stored.</param>
		/// <returns>The array containing all the elements of the collection.</returns>
		public static Object[] ToArray(ICollection c, Object[] objects)
		{
			int index = 0;

			Type type = objects.GetType().GetElementType();
			var objs = (object[]) Array.CreateInstance(type, c.Count);

			IEnumerator e = c.GetEnumerator();

			while (e.MoveNext())
			{
				objs[index++] = e.Current;
			}

			//If objects is smaller than c then do not return the new array in the parameter
			if (objects.Length >= c.Count)
			{
				objs.CopyTo(objects, 0);
			}

			return objs;
		}

		public static Assembly GetAssembly(string assemblyName)
		{
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				if (assembly.GetName().Name == assemblyName)
				{
					return assembly;
				}
			}
			return Assembly.Load(assemblyName);
		}
	}
}