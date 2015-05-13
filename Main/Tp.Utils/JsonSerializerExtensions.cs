// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System.IO;
using Newtonsoft.Json;

namespace Tp.Utils
{
	public static class JsonSerializerExtensions
	{
		public static string Serialize<T>(this JsonSerializer serializer, T obj)
		{
			using (TextWriter writer = new StringWriter())
			{
				serializer.Serialize(writer, obj);
				return writer.ToString();
			}
		}

		public static T Deserialize<T>(this JsonSerializer serializer, string str)
		{
			using (var reader = new StringReader(str))
			{
				return serializer.Deserialize<T>(new JsonTextReader(reader));
			}
		}
	}
}